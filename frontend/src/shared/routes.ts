const apiBase = "/api";

const apiRoutes = {
  health: (): string => [apiBase, "health"].join("/"),
  properties: {
    list: (): string => [apiBase, "properties"].join("/"),
    getById: (id: string): string => [apiBase, "properties", id].join("/"),
  },
  brokers: {
    list: (): string => [apiBase, "brokers"].join("/"),
    getById: (id: string): string => [apiBase, "brokers", id].join("/"),
  },
  leads: {
    create: (propertyId: string) => [apiBase, "/properties", propertyId, "lead"].join("/"),
  },
};

// TODO: redo structure to be alike leads ?
const pagesRoutes = {
  home: (): string => "/",
  brokers: (): string => "/brokers",
  blog: (): string => "/blog",
  properties: (): string => "properties",
  propertyDetails: (id: string) => ["/properties", id].join("/"),
  leads: {
    create: (propertyId: string) => ["/properties", propertyId, "lead"].join("/"),
  },
};

const routes = {
  app: pagesRoutes,
  api: apiRoutes,
};

export default routes;
