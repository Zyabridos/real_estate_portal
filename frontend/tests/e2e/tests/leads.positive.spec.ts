import { test, expect } from "@playwright/test";
import { routes } from "../__fixtures__/routes";
import { testIds } from "../__fixtures__/testIds";
import { testData } from "../__fixtures__/testData";
import { apiRoutes } from "../__fixtures__/apiRoutes";

test.describe("Leads: positive cases", () => {
  test("request sent, success message shown ", async ({ page }) => {
    const propertyId = testData.properties.id;

    let interceptedPayload: any | null = null;

    // allow requests through, but do not "garbage" test DB
    await page.route(apiRoutes.leads.pattern(), async (route) => {
      const req = route.request();
      if (req.method() !== "POST") {
        await route.continue();
        return;
      }

      try {
        interceptedPayload = req.postDataJSON();
      } catch {
        const raw = req.postData() ?? "{}";
        interceptedPayload = JSON.parse(raw);
      }

      await route.fulfill({
        status: 201,
        contentType: "application/json",
        body: JSON.stringify({
          id: "11111111-1111-1111-1111-111111111111",
        }),
      });
    });

    await page.goto(routes.properties.details(propertyId));

    await expect(page.getByTestId(testIds.properties.detailsPage)).toBeVisible();
    await expect(page.getByTestId(testIds.properties.detailsTitle)).toBeVisible();

    await page.getByTestId(testIds.properties.createLeadButton).click();

    await expect(page.getByTestId(testIds.leads.page)).toBeVisible();
    await expect(page.getByTestId(testIds.leads.form)).toBeVisible();

    await page.getByTestId(testIds.leads.fullName).fill(testData.leads.fullName.valid);
    await page.getByTestId(testIds.leads.email).fill(testData.leads.email.valid);
    await page.getByTestId(testIds.leads.phoneNumber).fill(testData.leads.phoneNumber.valid);
    await page.getByTestId(testIds.leads.message).fill(testData.leads.message.valid);

    const reqPromise = page.waitForRequest(
      (r) => r.url().includes(apiRoutes.leads.path()) && r.method() === "POST"
    );

    await expect(page.getByTestId(testIds.leads.submit)).toBeEnabled();
    await page.getByTestId(testIds.leads.submit).click();

    await reqPromise;

    expect(interceptedPayload).toBeTruthy();
    expect(interceptedPayload).toMatchObject({
      propertyId,
      fullName: testData.leads.fullName.valid,
      email: testData.leads.email.valid,
      phoneNumber: testData.leads.phoneNumber.valid,
      message: testData.leads.message.valid,
    });

    // UI shows success
    await expect(page.getByTestId(testIds.leads.success)).toBeVisible();
  });
});
