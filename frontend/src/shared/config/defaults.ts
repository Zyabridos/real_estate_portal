export const DEFAULT_HTTP_TIMEOUT_MS = 15_000;

export const REQUEST_QUERY_DEFAULTS = {
  page: 1,
  pageSize: 20,
  sortBy: "CreatedAt",
  sortDirection: "desc" as const,
};
