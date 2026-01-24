export type AppEnv = {
  apiBaseUrl: string;

  sanity: {
    projectId: string | null;
    dataset: string | null;
    apiVersion: string;
    useCdn: boolean;
  };
};
const isDocker = (import.meta.env.VITE_DOCKER as string | undefined) === "1"
  || (import.meta.env.DOCKER as string | undefined) === "1"
  || !!import.meta.env.VITE_API_PROXY_TARGET;

export const env: AppEnv = {
  apiBaseUrl: readEnvString("VITE_API_BASE_URL") ?? (isDocker ? "/api" : "http://localhost:5000"),

  sanity: {
    projectId: readEnvString("VITE_SANITY_PROJECT_ID"),
    dataset: readEnvString("VITE_SANITY_DATASET"),
    apiVersion: readEnvString("VITE_SANITY_API_VERSION") ?? "2025-01-01",
    useCdn: readEnvBool("VITE_SANITY_USE_CDN", true),
  },
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

if (import.meta.env.DEV) {
  const missing: string[] = [];
  if (!env.sanity.projectId) missing.push("VITE_SANITY_PROJECT_ID");
  if (!env.sanity.dataset) missing.push("VITE_SANITY_DATASET");

  if (missing.length > 0) {
    console.warn(
      `[Sanity] Missing env variables: ${missing.join(", ")}. Blog is scaffolded only.`,
    );
  }
}
