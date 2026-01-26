const apiRoutes = {
  health: (): string => ["health"].join("/"),
  properties: {
    list: (): string => ["properties"].join("/"),
    getById: (id: string): string => ["properties", id].join("/"),
  },
  brokers: {
    list: (): string => ["brokers"].join("/"),
    getById: (id: string): string => ["brokers", id].join("/"),
  },
  leads: {
    create: (): string => ["leads"].join("/"),
  },
};

// TODO: redo structure to be alike leads?
const pagesRoutes = {
  home: (): string => "/",
  brokers: (): string => "/brokers",
  blog: (): string => "/blog",
  properties: (): string => "/properties",
  propertyDetails: (id: string) => ["/properties", id].join("/"),
  leads: {
    create: (propertyId: string) => ["/properties", propertyId, "lead"].join("/"),
    list: (): string => "/leads",
  },
};

const routes = { app: pagesRoutes, api: apiRoutes };
export default routes;
