import getEnvString from "../helpers/env";

const normalizeBase = (base: string): string => {
  const trimmed = base.trim();
  if (!trimmed) return "/api";
  if (!trimmed.startsWith("/")) return `/${trimmed}`;
  return trimmed.endsWith("/") ? trimmed.slice(0, -1) : trimmed;
};

const apiBase = normalizeBase(getEnvString("E2E_API_BASE", "/api"));

export const apiRoutes = {
  health: {
    path: () => "/api/health",
  },
  agencies: {
    path: () => "/api/agencies",
    byId: (id: string) => `/api/agencies/${id}`,
    pattern: () => /\/api\/agencies(\/[^/?]+)?(\?.*)?$/,
  },
  leads: {
    path: () => "/api/leads",
    byId: (id: string) => `/api/leads/${id}`,
    pattern: () => /\/api\/leads(\/[^/?]+)?(\?.*)?$/,
  },
  properties: {
    path: () => "/api/properties",
    byId: (id: string) => `/api/properties/${id}`,
    pattern: () => /\/api\/properties(\/[^/?]+)?(\?.*)?$/,
  },
};
