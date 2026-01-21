export const testIds = {
  properties: {
    page: "properties-page",
    loading: "properties-loading",
    empty: "properties-empty",
    list: "properties-list",
    card: "property-card",
    cardMeta: "property-card-meta",

    detailsPage: "property-details-page",
    detailsTitle: "page-title",
    detailsGallery: "property-gallery",
    detailsBrokerBlock: "property-broker-block",
    createLeadButton: "create-lead-button",
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
  leads: {
    page: "lead-create-page",
    form: "lead-form",
    success: "lead-success",
    error: "lead-error",
    submit: "lead-submit",
    fullName: "lead-fullName",
    email: "lead-email",
    phoneNumber: "lead-phoneNumber",
    message: "lead-message"
  }
} as const;
