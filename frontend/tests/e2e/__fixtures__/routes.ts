const pathBase = "";

const withQuery = (path: string, query?: string): string => {
  if (!query) return path;
  return query.startsWith("?") ? `${path}${query}` : `${path}?${query}`;
};

export const routes = {
  agencies: {
    list: (query?: string) => withQuery([pathBase, "agencies"].join("/"), query),
    details: (id: string | number) => [pathBase, "agencies", id].join("/"),
  },

  blog: {
    list: (query?: string) => withQuery([pathBase, "blog"].join("/"), query),
    details: (slug: string | number) => [pathBase, "blog", slug].join("/"),
  },

  health: (query?: string) => withQuery([pathBase, "health"].join("/"), query),

  leads: {
    create: (id: string | number) => [pathBase, "properties", id, "leads"].join("/"),
    getById: (id: string | number) => [pathBase, "id", id],
    list: (query?: string) => withQuery([pathBase, "leads"].join("/"), query),
  },

  properties: {
    list: (query?: string) => withQuery([pathBase, "properties"].join("/"), query),
    details: (id: string | number) => [pathBase, "properties", id].join("/"),
  },
};
