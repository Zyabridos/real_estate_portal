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
    submit: "lead-submit",
    validationBanner: "lead-validation-banner",

    fullName: "lead-fullName",
    email: "lead-email",
    phoneNumber: "lead-phoneNumber",
    message: "lead-message",

    fullNameError: "lead-fullName-error",
    emailError: "lead-email-error",
    phoneNumberError: "lead-phoneNumber-error",
    messageError: "lead-message-error",

    success: "lead-success",
    error: "lead-error",
  },

  blog: {
    list: "blog-articles-list",
    categorySelect: "blog-category-select",
    detailsTitle: "blog-details-title",
    detailsContent: "blog-details-content",
  },
} as const;
