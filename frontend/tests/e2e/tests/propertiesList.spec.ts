import { test, expect } from "@playwright/test";
import { routes } from "../__fixtures__/routes";
import { testIds } from "../__fixtures__/testIds";
import { PropertyType } from "../types/properties";
import { waitForPropertiesLoaded } from "../helpers/waitFor";

// TODO: add tests that text is rendered at least on english version of webpage
const DEFAULT_PAGE = 1;
const PAGE_SIZE_LARGE = 50;
const PAGE_SIZE_SMALL = 5;

test.describe("Properties catalog", () => {
  test("properties catalog renders list", async ({ page }) => {
    await page.goto(routes.properties.list());
    await waitForPropertiesLoaded(page);

    await expect(page.getByTestId(testIds.properties.list)).toBeVisible();
    await expect(page.getByTestId(testIds.properties.card).first()).toBeVisible();
  });

  test("properties filter by type updates URL and results", async ({ page }) => {
    await page.goto(routes.properties.list(`?page=${DEFAULT_PAGE}&pageSize=${PAGE_SIZE_LARGE}`));
    await waitForPropertiesLoaded(page);

    const beforeCount = await page.getByTestId(testIds.properties.card).count();

    await page.getByTestId(testIds.filters.type).selectOption(PropertyType.Apartment);
    await page.getByTestId(testIds.filters.apply).click();

    await expect(page).toHaveURL(new RegExp(`type=${PropertyType.Apartment}`));
    await expect(page).toHaveURL(new RegExp(`page=${DEFAULT_PAGE}`));

    await waitForPropertiesLoaded(page);

    const afterCount = await page.getByTestId(testIds.properties.card).count();
    expect(afterCount).toBeGreaterThan(0);
    expect(afterCount).toBeLessThanOrEqual(beforeCount);

    const metaText = await page.getByTestId(testIds.properties.cardMeta).first().textContent();
    expect(metaText ?? "").toContain(PropertyType.Apartment);
  });

  test("properties paging next updates results", async ({ page }) => {
    await page.goto(routes.properties.list(`?page=${DEFAULT_PAGE}&pageSize=${PAGE_SIZE_SMALL}`));
    await waitForPropertiesLoaded(page);

    const firstMetaBefore = await page.getByTestId(testIds.properties.cardMeta).first().textContent();

    await expect(page.getByTestId(testIds.pagination.next)).toBeEnabled();
    await page.getByTestId(testIds.pagination.next).click();

    await expect(page).toHaveURL(/page=2/);
    await waitForPropertiesLoaded(page);

    const firstMetaAfter = await page.getByTestId(testIds.properties.cardMeta).first().textContent();
    expect(firstMetaAfter).not.toEqual(firstMetaBefore);
  });
});
