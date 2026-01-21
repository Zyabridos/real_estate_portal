import getEnvString from "../helpers/env";

const normalizeBase = (base: string): string => {
  const trimmed = base.trim();
  if (!trimmed) return "/api";
  if (!trimmed.startsWith("/")) return `/${trimmed}`;
  return trimmed.endsWith("/") ? trimmed.slice(0, -1) : trimmed;
};

const apiBase = normalizeBase(getEnvString("E2E_API_BASE", "/api"));

export const apiRoutes = {
  leads: {
    path: () => `${apiBase}/leads`,
    pattern: () => `**${apiBase}/leads`,
  },
} as const;
