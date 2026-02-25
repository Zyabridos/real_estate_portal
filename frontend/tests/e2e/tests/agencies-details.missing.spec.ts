import { test, expect } from "@playwright/test";

import setLanguage from "../helpers/setLanguage";

import { languages } from "../__fixtures__/languages";
import { routes } from "../__fixtures__/routes";
import { apiRoutes } from "../__fixtures__/apiRoutes";
import { testIds } from "../__fixtures__/testIds";
import { testData } from "../__fixtures__/testData";

const agenciesMissing = testData.agencies.missing;

const isAgenciesDetailsGet = (url: string, method: string): boolean => {
  if (method !== "GET") return false;
  return /\/api\/agencies\/[^/?]+(\?.*)?$/.test(url);
};

test.describe("Agencies: details missing states", () => {
  test.describe.configure({ mode: "serial" });

  languages.forEach((lang) => {
    test.describe(`${lang.toUpperCase()} | missing states`, () => {
      test.beforeEach(async ({ page }) => {
        await setLanguage(page, lang);
        await page.route(apiRoutes.agencies.pattern(), async (route) => route.continue());
      });

      test("invalid id (whitespace) shows friendly state (not message from backand)", async ({ page }) => {
        let detailsCalls = 0;

        page.on("request", (req) => {
          if (isAgenciesDetailsGet(req.url(), req.method())) detailsCalls += 1;
        });

        await page.goto(routes.agencies.details(agenciesMissing.ids.whitespaceEncoded));

        await expect(page.getByTestId(testIds.agencies.detailsPage)).toBeVisible();
        await expect(page.getByTestId(testIds.agencies.errorState)).toBeVisible();

        await expect(page.getByTestId(testIds.states.errorTitle)).toBeVisible();
        await expect(page.getByTestId(testIds.states.errorMessage)).toBeVisible();

        // no refresh button inside state
        await expect(page.getByTestId(testIds.states.retryButton)).toHaveCount(0);

        expect(detailsCalls).toBe(0);
      });

      test("400 BadRequest (non-guid id) shows friendly message (does not leak backend detail)", async ({ page }) => {
        let detailsCalls = 0;

        page.on("request", (req) => {
          const isDetails = isAgenciesDetailsGet(req.url(), req.method());
          const isThisId = req.url().includes(apiRoutes.agencies.byId(agenciesMissing.ids.nonGuid));
          if (isDetails && isThisId) detailsCalls += 1;
        });

        await page.goto(routes.agencies.details(agenciesMissing.ids.nonGuid));

        await expect(page.getByTestId(testIds.agencies.detailsPage)).toBeVisible();
        await expect(page.getByTestId(testIds.agencies.errorState)).toBeVisible();

        await expect(page.getByTestId(testIds.states.errorTitle)).toBeVisible();

        const msg = page.getByTestId(testIds.states.errorMessage);
        await expect(msg).toBeVisible();

        for (const phrase of agenciesMissing.backendLeakPhrases) {
          await expect(msg).not.toContainText(phrase);
        }

        await expect(page.getByTestId(testIds.agencies.errorState)).toContainText(agenciesMissing.ids.nonGuid);

        await expect(page.getByTestId(testIds.states.retryButton)).toHaveCount(0);

        expect(detailsCalls).toBeGreaterThan(0);
      });

      test("404 NotFound shows notFound state and retry triggers re-fetch", async ({ page }) => {
        const missingId = agenciesMissing.ids.missingGuid;

        let detailsCalls = 0;

        page.on("request", (req) => {
          const isDetails = isAgenciesDetailsGet(req.url(), req.method());
          const isThisId = req.url().includes(apiRoutes.agencies.byId(missingId));
          if (isDetails && isThisId) detailsCalls += 1;
        });

        await page.goto(routes.agencies.details(missingId));

        await expect(page.getByTestId(testIds.agencies.detailsPage)).toBeVisible();
        await expect(page.getByTestId(testIds.agencies.errorState)).toBeVisible();

        const retry = page.getByTestId(testIds.states.retryButton);
        await expect(retry).toBeVisible();

        await expect.poll(() => detailsCalls, { timeout: 15000 }).toBeGreaterThan(0);
        const before = detailsCalls;

        await retry.click();
        await expect.poll(() => detailsCalls, { timeout: 15000 }).toBeGreaterThan(before);

        await expect(page).toHaveURL(new RegExp(`${routes.agencies.details(missingId)}$`));
      });
    });
  });
});
