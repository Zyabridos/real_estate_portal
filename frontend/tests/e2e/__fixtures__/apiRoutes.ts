export const apiRoutes = {
  health: {
    path: () => "/api/health/readiness",
  },
  agencies: {
    path: () => "/api/agencies",
    byId: (id: number) => `/api/agencies/${id}`,
    pattern: () => /\/api\/agencies(\/[^/?]+)?(\?.*)?$/,
  },
  leads: {
    path: () => "/api/leads",
    byId: (id: number) => `/api/leads/${id}`,
    pattern: () => /\/api\/leads(\/[^/?]+)?(\?.*)?$/,
  },
  properties: {
    path: () => "/api/properties",
    byId: (id: number) => `/api/properties/${id}`,
    pattern: () => /\/api\/properties(\/[^/?]+)?(\?.*)?$/,
  },
};
