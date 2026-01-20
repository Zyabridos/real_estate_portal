const pathBase = "";

const withQuery = (path: string, query?: string): string => {
  if (!query) return path;
  return query.startsWith("?") ? `${path}${query}` : `${path}?${query}`;
};

export const routes = {
  health: (query?: string) => withQuery([pathBase, "health"].join("/"), query),

  properties: {
    list: (query?: string) => withQuery([pathBase, "properties"].join("/"), query),
    details: (id: string) => [pathBase, "properties", id].join("/"),
  },
};
