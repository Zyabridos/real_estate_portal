/* eslint-disable no-console */

// TODO: consider moving to types folder
type AppEnv = {
  apiBaseUrl: string;

  sanity: {
    projectId: string | null;
    dataset: string | null;
    apiVersion: string;
    useCdn: boolean;
  };
};

function readString(key: string): string | null {
  const v = (import.meta.env[key] as string | undefined) ?? null;
  return v && v.trim().length > 0 ? v.trim() : null;
}

function readBool(key: string, fallback: boolean): boolean {
  const v = (import.meta.env[key] as string | undefined) ?? null;
  if (v === null) return fallback;
  return v.toLowerCase() === "true";
}

export const env: AppEnv = {
  apiBaseUrl: readString("VITE_API_PROXY_TARGET") ?? "http://localhost:5000",

  sanity: {
    projectId: readString("VITE_SANITY_PROJECT_ID"),
    dataset: readString("VITE_SANITY_DATASET"),
    apiVersion: readString("VITE_SANITY_API_VERSION") ?? "2025-01-01",
    useCdn: readBool("VITE_SANITY_USE_CDN", true),
  },
};

if (import.meta.env.DEV) {
  const missing: string[] = [];
  if (!env.sanity.projectId) missing.push("VITE_SANITY_PROJECT_ID");
  if (!env.sanity.dataset) missing.push("VITE_SANITY_DATASET");

  if (missing.length > 0) {
    console.warn(
      `[Sanity] Missing env variables: ${missing.join(", ")}. Blog is scaffolded only.`
    );
  }
}
