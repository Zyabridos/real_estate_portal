export const testIds = {
  properties: {
    page: "properties-page",
    loading: "properties-loading",
    empty: "properties-empty",
    list: "properties-list",
    card: "property-card",
    cardMeta: "property-card-meta",
  },

  filters: {
    root: "property-filters",
    city: "filter-city",
    type: "filter-type",
    status: "filter-status",
    minPrice: "filter-minPrice",
    maxPrice: "filter-maxPrice",
    apply: "filters-apply",
    reset: "filters-reset",
  },

  pagination: {
    root: "pagination",
    prev: "pagination-prev",
    next: "pagination-next",
    status: "pagination-status",
  },
} as const;
