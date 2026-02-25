import { test, expect } from "@playwright/test";
import setLanguage from "../helpers/setLanguage";
import { routes } from "../__fixtures__/routes";
import { apiRoutes } from "../__fixtures__/apiRoutes";
import { testIds } from "../__fixtures__/testIds";
import { getSeedAgencyIds } from "../helpers/seedEnv";
import { agenciesSeed } from "../__fixtures__/agenciesSeed";

type Lang = "en" | "ru" | "no";
const languages: Lang[] = ["en", "ru", "no"];

test.describe("Agencies: list page (seeded DB)", () => {
  test.describe.configure({ mode: "serial" });

  languages.forEach((lang) => {
    test.describe(`${lang.toUpperCase()} | /agencies`, () => {
      test.beforeEach(async ({ page }) => {
        await setLanguage(page, lang);
        await page.route(apiRoutes.agencies.pattern(), async (route) => route.continue());
      });

      test("renders seeded agency cards and opens details by clicking card", async ({ page }) => {
        const { agency1Id } = getSeedAgencyIds();
        await page.goto(routes.agencies.list());
        await expect(page.getByTestId(testIds.agencies.listPage)).toBeVisible();

        const card = page.getByTestId(testIds.agencies.card(agency1Id));
        await expect(card).toBeVisible({ timeout: 15000 });

        await card.getByTestId(testIds.agencies.viewDetails).click();

        await expect(page).toHaveURL(new RegExp(`/agencies/${agency1Id}$`));
        await expect(page.getByTestId(testIds.agencies.detailsPage)).toBeVisible({ timeout: 15000 });
        await expect(page.getByTestId(testIds.agencies.pageTitle)).toContainText(agenciesSeed.agency1.name);
      });
    });
  });
});
