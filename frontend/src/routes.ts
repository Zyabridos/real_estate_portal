const apiBase = "/api";

const apiRoutes = {
  health: (): string => [apiBase, "health"].join("/"),
};

const pagesRoutes = {
  home: (): string => "/",
};

const routes = {
  app: pagesRoutes,
  api: apiRoutes,
};

export default routes;
