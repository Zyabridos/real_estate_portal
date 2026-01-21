import { test, expect } from "@playwright/test";
import { routes } from "../__fixtures__/routes";
import { apiRoutes } from "../__fixtures__/apiRoutes";
import { testIds } from "../__fixtures__/testIds";
import { testData } from "../__fixtures__/testData";
import { leadValidationMessages, type Lang } from "../__fixtures__/leadValidationMessages";
import setLanguage from "../helpers/setLanguage";

const languages: Lang[] = ["en", "ru", "no"];

test.describe("Leads: negative cases (client validation)", () => {
  test.describe.configure({ mode: "serial" });

  languages.forEach((lang) => {
    test.describe(`${lang.toUpperCase()} | Leads validation`, () => {
      test.beforeEach(async ({ page }) => {
        await setLanguage(page, lang);

        // allow requests through, but do not "garbage" test DB
        await page.route(apiRoutes.leads.pattern(), async (route) => {
          await route.continue();
        });
      });

      test.describe("Invalid email / invalid phone", () => {
        test("invalid email shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);
          await page.getByTestId(testIds.leads.email).fill(testData.leads.email.invalid.format);
          await page.getByTestId(testIds.leads.email).blur();

          await expect(page.getByTestId(testIds.leads.emailError)).toHaveText(m.emailInvalid);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("invalid phone shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);
          await page.getByTestId(testIds.leads.phoneNumber).fill(testData.leads.phoneNumber.invalid.plusNotAtStart);
          await page.getByTestId(testIds.leads.phoneNumber).blur();

          await expect(page.getByTestId(testIds.leads.phoneNumberError)).toHaveText(m.phoneInvalid);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("phone separators > 3 shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);
          await page.getByTestId(testIds.leads.phoneNumber).fill(testData.leads.phoneNumber.invalid.separatorsOverMax);
          await page.getByTestId(testIds.leads.phoneNumber).blur();

          await expect(page.getByTestId(testIds.leads.phoneNumberError)).toHaveText(m.phoneSeparatorsMax);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("too short email shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);

          // satisfy "contact" with non-empty email, but invalid-short
          await page.getByTestId(testIds.leads.email).fill(testData.leads.email.invalid.tooShort);
          await page.getByTestId(testIds.leads.email).blur();

          await expect(page.getByTestId(testIds.leads.emailError)).toHaveText(m.emailMin);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("too long email (>100) shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);

          await page.getByTestId(testIds.leads.email).fill(testData.leads.email.invalid.tooLong);
          await page.getByTestId(testIds.leads.email).blur();

          await expect(page.getByTestId(testIds.leads.emailError)).toHaveText(m.emailMax);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("too short phone (<7) shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);

          // satisfy "contact" with phone (but too short)
          await page.getByTestId(testIds.leads.phoneNumber).fill(testData.leads.phoneNumber.invalid.tooShort);
          await page.getByTestId(testIds.leads.phoneNumber).blur();

          await expect(page.getByTestId(testIds.leads.phoneNumberError)).toHaveText(m.phoneMin);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

        test("too long phone (>20) shows error and disables submit button", async ({ page }) => {
          const m = leadValidationMessages[lang];
          const propertyId = testData.properties.id;

          let postCount = 0;
          page.on("request", (req) => {
            if (req.url().includes(apiRoutes.leads.path()) && req.method() === "POST") postCount += 1;
          });

          await page.goto(routes.properties.details(propertyId));
          await page.getByTestId(testIds.properties.createLeadButton).click();
          await expect(page.getByTestId(testIds.leads.page)).toBeVisible();

          await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);

          await page.getByTestId(testIds.leads.phoneNumber).fill(testData.leads.phoneNumber.invalid.tooLong);
          await page.getByTestId(testIds.leads.phoneNumber).blur();

          await expect(page.getByTestId(testIds.leads.phoneNumberError)).toHaveText(m.phoneMax);
          await expect(page.getByTestId(testIds.leads.submit)).toBeDisabled();

          expect(postCount).toBe(0);
          await expect(page.getByTestId(testIds.leads.validationBanner)).toHaveText(m.fixErrors);
        });

      });
    });
  });
});
