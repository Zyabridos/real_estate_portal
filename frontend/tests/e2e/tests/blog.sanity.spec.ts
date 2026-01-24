import { test, expect } from "@playwright/test";
import { routes } from "../__fixtures__/routes";
import { testIds } from "../__fixtures__/testIds";
import { testData } from "../__fixtures__/testData";
import { getQueryParam } from "../helpers/url";

test.describe("Blog (Sanity): list + filter + details", () => {
  test("when open /blog articles, list is rendered", async ({ page }) => {
    await page.goto(routes.blog.list());

    await expect(page.getByTestId(testIds.blog.categorySelect)).toBeVisible();
    await expect(page.getByTestId(testIds.blog.list)).toBeVisible();

    const list = page.getByTestId(testIds.blog.list);
    const cards = list.locator("article");
    await expect(cards.first()).toBeVisible();
    expect(await cards.count()).toBeGreaterThan(0);
  });

  test("category filter updates URL and shows filtered list", async ({ page }) => {
    await page.goto(routes.blog.list());

    const categorySelect = page.getByTestId(testIds.blog.categorySelect);
    await expect(categorySelect).toBeVisible();

    await categorySelect.selectOption(testData.blog.categories.buyingGuide.slug);

    await expect(page).toHaveURL(/\/blog\?category=buying-guide/);
    const categoryFromUrl = getQueryParam(page.url(), "category");
    expect(categoryFromUrl).toBe(testData.blog.categories.buyingGuide.slug);

    // verify list is still visible and has at least one item
    const list = page.getByTestId(testIds.blog.list);
    await expect(list).toBeVisible();

    const cards = list.locator("article");
    expect(await cards.count()).toBeGreaterThan(0);

    // check the tag text is present on first few cards
    const take = Math.min(await cards.count(), 5);
    for (let i = 0; i < take; i += 1) {
      await expect(cards.nth(i)).toContainText(testData.blog.categories.buyingGuide.title);
    }
  });

  test("click Buying Guide article navigates to /blog/:slug and shows title and content", async ({ page }) => {
    await page.goto(routes.blog.list());

    const list = page.getByTestId(testIds.blog.list);
    await expect(list).toBeVisible();

    const buyingGuideCard = list.locator("article", {
      has: page.locator(`text=${testData.blog.categories.buyingGuide.title}`),
    }).first();
    await expect(buyingGuideCard).toBeVisible();

    const link = buyingGuideCard.locator('a[aria-label="Open article"]').first();
    await expect(link).toBeVisible();

    const href = await link.getAttribute("href");
    expect(href).toBeTruthy();
    expect(href).toMatch(/^\/blog\/.+/);

    const before = page.url();
    await link.click(); // TODO: do it more alike user action

    await expect.poll(() => page.url()).not.toBe(before);
    await expect(page).toHaveURL(new RegExp(`${href}$`));

    await expect(page.getByTestId(testIds.blog.detailsTitle)).toBeVisible();
    await expect(page.getByTestId(testIds.blog.detailsContent)).toBeVisible();
  });

  test("deep link /blog/:slug loads from Sanity and content is visible", async ({ page }) => {
    const slug = testData.blog.article.knownSlug;

    await page.goto(routes.blog.details(slug));

    await expect(page.getByTestId(testIds.blog.detailsTitle)).toBeVisible();
    await expect(page.getByTestId(testIds.blog.detailsContent)).toBeVisible();
  });

  test("category filter persists on reload (URL is source of truth)", async ({ page }) => {
    const slug = testData.blog.categories.buyingGuide.slug;

    await page.goto(routes.blog.list(`category=${slug}`));

    await expect(page).toHaveURL(new RegExp(`/blog\\?category=${slug}`));

    const categorySelect = page.getByTestId(testIds.blog.categorySelect);
    await expect(categorySelect).toBeVisible();
    await expect(categorySelect).toHaveValue(slug);

    await expect(page.getByTestId(testIds.blog.list)).toBeVisible();
  });

  test("when reset category filte, URL is cleaned, but list still visible", async ({ page }) => {
    await page.goto(routes.blog.list());

    const categorySelect = page.getByTestId(testIds.blog.categorySelect);
    await expect(categorySelect).toBeVisible();

    // set category
    await categorySelect.selectOption(testData.blog.categories.buyingGuide.slug);
    await expect(page).toHaveURL(/\/blog\?category=buying-guide/);

    // reset to "All"
    await categorySelect.selectOption({ label: testData.blog.labels.allOption });

    // URL should have category removed
    await expect(page).toHaveURL(/\/blog$/);

    await expect(page.getByTestId(testIds.blog.list)).toBeVisible();
  });
});
