const apiRoutes = {
  health: {
    liveness: (): string => ["health", "liveness"].join("/"),
    readiness: (): string => ["health", "readiness"].join("/"),
  },
  agencies: {
    list: (): string => ["agencies"].join("/"),
    getById: (id: number): string => ["agencies", id].join("/"),
  },
  properties: {
    list: (): string => ["properties"].join("/"),
    getById: (id: number): string => ["properties", id].join("/"),
  },
  brokers: {
    list: (): string => ["brokers"].join("/"),
    getById: (id: number): string => ["brokers", id].join("/"),
  },
  leads: {
    create: (): string => ["leads"].join("/"),
    getById: (id: number): string => ["leads", id].join("/"),
  },
};

const pagesRoutes = {
  home: (): string => "/",
  agencies: {
    list: (): string => "/agencies",
    details: (id: number): string => ["/agencies", id].join("/"),
  },
  brokers: {
    list: (): string => "/brokers",
    details: (id: number): string => ["/brokers", id].join("/"),
  },
  blog: {
    list: (): string => "/blog",
    details: (slug: string): string => ["/blog", slug].join("/"),
  },
  properties: {
    list: (): string => "/properties",
    details: (id: number): string => ["/properties", id].join("/"),
  },
  leads: {
    create: (propertyId: number) => ["/properties", propertyId, "lead"].join("/"),
    list: (): string => "/leads",
  },
};

const routes = { app: pagesRoutes, api: apiRoutes };
export default routes;
