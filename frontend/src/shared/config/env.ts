// TODO: move somewhere more centilized
export type AppEnv = {
  apiBaseUrl: string;
  sanity: {
    projectId: string;
    dataset: string;
    apiVersion: string;
    useCdn: boolean;
  };
  i18nDefaultLanguage: string;
};

export const env: AppEnv = {
  apiBaseUrl: readEnvString("VITE_API_PREFIX") ?? "/api",

  sanity: {
    projectId: requireEnv("VITE_SANITY_PROJECT_ID"),
    dataset: requireEnv("VITE_SANITY_DATASET"),
    apiVersion: readEnvString("VITE_SANITY_API_VERSION") ?? "2025-01-01",
    useCdn: readEnvBool("VITE_SANITY_USE_CDN", true),
  },

  i18nDefaultLanguage: readEnvString("VITE_I18N_DEFAULT_LANGUAGE") ?? "en",
};

function readEnvString(key: string): string | null {
  const v = (import.meta.env[key] as string | undefined) ?? null;
  return v && v.trim().length > 0 ? v.trim() : null;
}

function readEnvBool(key: string, fallback: boolean): boolean {
  const v = (import.meta.env[key] as string | undefined) ?? null;
  if (v === null) return fallback;
  return v.toLowerCase() === "true";
}

function requireEnv(key: string): string {
  const v = readEnvString(key);
  if (v) return v;

  throw new Error(`Missing required env variable: ${key}`);
}
