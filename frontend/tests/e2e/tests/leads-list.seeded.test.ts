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

      test("default view is grouped and shows 3 seeded groups with 1 row each", async ({ page, request }) => {
        const { emailOnly, phoneOnly, both } = await getSeedLeads(request);

        const title1 = await getPropertyTitle(request, emailOnly.propertyId);
        const title2 = await getPropertyTitle(request, phoneOnly.propertyId);
        const title3 = await getPropertyTitle(request, both.propertyId);

        await page.goto(routes.leads.list());
        await expect(page.getByTestId(testIds.leadsList.page)).toBeVisible();

        await expect(page.getByTestId(testIds.leadsList.viewGrouped)).toHaveAttribute("aria-pressed", "true");
        await expect(page.getByTestId(testIds.leadsList.tableGrouped)).toBeVisible();

        const group1 = page.getByTestId(testIds.leadsList.groupHeader(emailOnly.propertyId));
        const group2 = page.getByTestId(testIds.leadsList.groupHeader(phoneOnly.propertyId));
        const group3 = page.getByTestId(testIds.leadsList.groupHeader(both.propertyId));

        await expect(group1).toBeVisible();
        await expect(group2).toBeVisible();
        await expect(group3).toBeVisible();

        await expect(group1).toContainText(title1);
        await expect(group2).toContainText(title2);
        await expect(group3).toContainText(title3);

        await expect(group1).toContainText("Property ID: 1");
        await expect(group2).toContainText("Property ID: 2");
        await expect(group3).toContainText("Property ID: 3");

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
