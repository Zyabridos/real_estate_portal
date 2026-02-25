export const testIds = {
  states: {
    errorTitle: "error-title",
    errorMessage: "error-message",
    retryButton: "retry-button",
  },

  agencies: {
    listPage: "agencies-list-page",
    detailsPage: "agency-details-page",
    errorState: "error-state",

    pageTitle: "page-title",
    detailsCard: "agency-details-card",

    refreshButton: "refresh-button",
    backToListButton: "back-to-list-button",

    viewDetails: "agency-view-details",

    card: (id: string) => `agency-card-${id}`,
  },

  blog: {
    list: "blog-articles-list",
    categorySelect: "blog-category-select",
    detailsTitle: "blog-details-title",
    detailsContent: "blog-details-content",
  },

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

  leadsList: {
    page: "leads-list-page",

    viewGrouped: "view-grouped",
    viewList: "view-list",

    loading: "leads-list-loading",
    error: "leads-list-error",
    empty: "leads-list-empty",

    tableGrouped: "leads-table-grouped",
    tableList: "leads-table-list",

    groupHeader: (propertyId: string) => `lead-group-header-${propertyId}`,
    row: (leadId: string) => `lead-row-${leadId}`,
    commentBtn: (leadId: string) => `lead-action-comment-${leadId}`,
    propertyLink: (leadId: string) => `lead-property-link-${leadId}`,
  },

  leadMessageModal: {
    root: "lead-message-modal",
    body: "lead-message-body",
    content: "lead-message-content",
    close: "lead-message-close",
    loading: "lead-message-loading",
    error: "lead-message-error",
  }
} as const;
