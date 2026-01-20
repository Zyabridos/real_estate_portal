import type { ApiErrorKind, ProblemDetails } from "@/shared/types/errors";

export const buildMessage = (params: {
  status?: number;
  kind: ApiErrorKind;
  pd?: ProblemDetails;
  fallback?: string;
}): string => {
  const { status, kind, pd, fallback } = params;

  const serverMessage =
    (typeof pd?.title === "string" && pd.title.trim()) ||
    (typeof pd?.detail === "string" && pd.detail.trim());

  if (serverMessage) return serverMessage;

  if (kind === "Network") return "Network error. Please check your connection.";
  if (kind === "Timeout") return "Request timed out. Please try again.";
  if (kind === "Abort") return "Request was aborted.";
  if (status) return `Request failed with status ${status}.`;

  return fallback ?? "Unexpected error.";
}
