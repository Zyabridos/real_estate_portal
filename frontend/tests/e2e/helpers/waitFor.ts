import { expect, type Page } from "@playwright/test";
import { testIds } from "../__fixtures__/testIds";

export async function waitForPropertiesLoaded(page: Page): Promise<void> {
  await expect(page.getByTestId(testIds.properties.page)).toBeVisible();
  // loading mught not show up, so
  try {
    await expect(page.getByTestId(testIds.properties.loading)).toBeHidden({ timeout: 15000 });
  } catch {
    // ok
  }
  // either list, either empty
  await expect(
    page.getByTestId(testIds.properties.list).or(page.getByTestId(testIds.properties.empty))
  ).toBeVisible({ timeout: 15000 });
}
