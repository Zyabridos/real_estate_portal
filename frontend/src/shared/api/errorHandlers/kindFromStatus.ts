import type { ApiErrorKind, ProblemDetails } from "@/shared/types/errors";

export const kindFromStatus = (status: number, pd?: ProblemDetails): ApiErrorKind => {
  if (status >= 500) return "Server";

  switch (status) {
    case 400: {
      const hasValidationErrors =
        pd?.errors != null && typeof pd.errors === "object" && !Array.isArray(pd.errors);
      return hasValidationErrors ? "Validation" : "BadRequest";
    }
    case 401:
      return "Unauthorized";
    case 403:
      return "Forbidden";
    case 404:
      return "NotFound";
    case 409:
      return "Conflict";
    default:
      return "Unknown";
  }
};
