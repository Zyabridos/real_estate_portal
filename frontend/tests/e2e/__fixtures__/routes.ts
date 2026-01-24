const pathBase = "";

const withQuery = (path: string, query?: string): string => {
  if (!query) return path;
  return query.startsWith("?") ? `${path}${query}` : `${path}?${query}`;
};

export const routes = {
  blog: {
    list: (query?: string) => withQuery([pathBase, "blog"].join("/"), query),
    details: (slug: string) => [pathBase, "blog", slug].join("/"),
  },

  health: (query?: string) => withQuery([pathBase, "health"].join("/"), query),

  leads: {
    create: (id: string) => [pathBase, "properties", id, "leads"].join("/"),
  },

  properties: {
    list: (query?: string) => withQuery([pathBase, "properties"].join("/"), query),
    details: (id: string) => [pathBase, "properties", id].join("/"),
  },
};
