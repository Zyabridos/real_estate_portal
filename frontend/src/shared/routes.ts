const apiBase = "/api";

const apiRoutes = {
  health: (): string => [apiBase, "health"].join("/"),
  properties: {
    list: (queryString: string): string => [apiBase, "properties", queryString].join("/"),
    getById: (id: string): string => [apiBase, "properties", id].join("/"),
  },
  brokers: {
    list: (queryString: string): string => [apiBase, "brokers", queryString].join("/"),
    getById: (id: string): string => [apiBase, "brokers", id].join("/"),
  }
};

const pagesRoutes = {
  home: (): string => "/",
};

const routes = {
  app: pagesRoutes,
  api: apiRoutes,
};

export default routes;
