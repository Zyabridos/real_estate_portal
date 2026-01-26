import { test, expect } from "@playwright/test";
import { routes } from "../__fixtures__/routes";
import { apiRoutes } from "../__fixtures__/apiRoutes";
import { testIds } from "../__fixtures__/testIds";
import { leadsSeed } from "../__fixtures__/leadsSeed";
import setLanguage from "../helpers/setLanguage";

import { getSeedLeads } from "../helpers/getSeedLeads";
import { getPropertyTitle } from "../helpers/getPropertyTitle";
import { updateLeadMessage } from "../helpers/updateLeadMessage";

type Lang = "en" | "ru" | "no";
const languages: Lang[] = ["en", "ru", "no"];

test.describe("Leads: list page (seeded DB)", () => {
  test.describe.configure({ mode: "serial" });

  languages.forEach((lang) => {
    test.describe(`${lang.toUpperCase()} | /leads`, () => {
      test.beforeEach(async ({ page }) => {
        await setLanguage(page, lang);

        await page.route(apiRoutes.leads.pattern(), async (route) => route.continue());
        await page.route(apiRoutes.properties.pattern(), async (route) => route.continue());
      });

      test("default view is grouped and shows one group for seeded propertyId + 3 rows", async ({ page, request }) => {
        const { propertyId, emailOnly, phoneOnly, both } = await getSeedLeads(request);
        const title = await getPropertyTitle(request, propertyId);

        await page.goto(routes.leads.list());
        await expect(page.getByTestId(testIds.leadsList.page)).toBeVisible();

        // grouped is default
        await expect(page.getByTestId(testIds.leadsList.viewGrouped)).toHaveAttribute("aria-pressed", "true");
        await expect(page.getByTestId(testIds.leadsList.tableGrouped)).toBeVisible();

        // group header exists for PROPERTY_ID and contains property title
        const groupHeader = page.getByTestId(testIds.leadsList.groupHeader(propertyId));
        await expect(groupHeader).toBeVisible();
        await expect(groupHeader).toContainText(title);
        await expect(groupHeader).toContainText("(3)");

        // rows exist by ids
        await expect(page.getByTestId(testIds.leadsList.row(emailOnly.id))).toBeVisible();
        await expect(page.getByTestId(testIds.leadsList.row(phoneOnly.id))).toBeVisible();
        await expect(page.getByTestId(testIds.leadsList.row(both.id))).toBeVisible();
      });

      test("comment opens modal and shows message (fetched from real API)", async ({ page, request }) => {
        const { both } = await getSeedLeads(request);

        await page.goto(routes.leads.list());
        await page.getByTestId(testIds.leadsList.viewList).click();

        await page.getByTestId(testIds.leadsList.commentBtn(both.id)).click();

        await expect(page.getByTestId(testIds.leadMessageModal.root)).toBeVisible();
        await expect(page.getByTestId(testIds.leadMessageModal.content)).toContainText(leadsSeed.both.message);

        await page.getByTestId(testIds.leadMessageModal.close).click();
        await expect(page.getByTestId(testIds.leadMessageModal.root)).toBeHidden();
      });

      test("sorting: sortBy=Status&desc does not break (no ErrorState)", async ({ page }) => {
        await page.goto("/leads?view=list&sortBy=Status&sortDirection=desc");

        await expect(page.getByTestId(testIds.leadsList.page)).toBeVisible();
        await expect(page.getByTestId(testIds.leadsList.error)).toHaveCount(0); // ErrorState not shown
        await expect(page.getByTestId(testIds.leadsList.tableList)).toBeVisible();
      });

      test("sorting: UpdatedAt desc works (update one lead => it becomes first)", async ({ page, request }) => {
        const { emailOnly } = await getSeedLeads(request);
        const updatedMessage = `Updated by e2e at ${new Date().toISOString()}`;
        await updateLeadMessage(request, emailOnly.id, updatedMessage);

        await page.goto("/leads?view=list&sortBy=UpdatedAt&sortDirection=desc");
        await expect(page.getByTestId(testIds.leadsList.tableList)).toBeVisible();

        const firstRow = page
          .getByTestId(testIds.leadsList.tableList)
          .locator("tbody tr")
          .first();

        const dtid = await firstRow.getAttribute("data-testid");
        expect(dtid).toBe(`lead-row-${emailOnly.id}`);

        await page.getByTestId(testIds.leadsList.commentBtn(emailOnly.id)).click();
        await expect(page.getByTestId(testIds.leadMessageModal.content)).toContainText(updatedMessage);
      });
    });
  });
});
