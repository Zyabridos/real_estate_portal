export const apiRoutes = {
  health: {
    path: () => "/api/health/readiness",
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
