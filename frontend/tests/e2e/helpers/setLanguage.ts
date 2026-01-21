import type { Page } from "@playwright/test";
import type { Lang } from "../__fixtures__/leadValidationMessages";

async function setLanguage(page: Page, lang: Lang) {
  await page.addInitScript(([lng]) => {
    window.localStorage.setItem("i18nextLng", lng);
  }, [lang]);
}

export default setLanguage;
