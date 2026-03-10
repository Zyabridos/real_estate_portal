import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import ru from "./locales/ru";
import en from "./locales/en";
import no from "./locales/no";

const DEFAULT_LANGUAGE = import.meta.env.VITE_I18N_DEFAULT_LANGUAGE ?? "en";

export const i18nInitialized = i18n.use(LanguageDetector).init({
  fallbackLng: DEFAULT_LANGUAGE,
  resources: { en, ru, no },
  supportedLngs: ["en", "ru", "no"],
  nonExplicitSupportedLngs: true,
  load: "languageOnly",
  ns: [
    "common",
    "navigation",
    "states",
    "home",
    "notFound",
    "errors",
    "agencies",
    "brokers",
    "properties",
    "leads",
    "blog",
  ],
  defaultNS: "common",
  interpolation: { escapeValue: false },
  pluralSeparator: "_",
  detection: {
    order: ["localStorage"],
    caches: ["localStorage"],
  },
});

export default i18n;

export const supportedLanguages = ["en", "ru", "no"] as const;
export type SupportedLanguage = (typeof supportedLanguages)[number];
