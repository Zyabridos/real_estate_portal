export type ApiErrorKind =
  | "Network"
  | "Timeout"
  | "Abort"
  | "BadRequest"
  | "Unauthorized"
  | "Forbidden"
  | "NotFound"
  | "Conflict"
  | "Validation"
  | "Server"
  | "Unknown";

export class ApiError extends Error {
  public readonly kind: ApiErrorKind;
  public readonly status?: number;
  public readonly url?: string;
  public readonly method?: string;
  public readonly problemDetails?: ProblemDetails;
  public readonly raw?: unknown;

  constructor(params: {
    message: string;
    kind: ApiErrorKind;
    status?: number;
    url?: string;
    method?: string;
    problemDetails?: ProblemDetails;
    raw?: unknown;
  }) {
    super(params.message);
    this.name = "ApiError";
    this.kind = params.kind;
    this.status = params.status;
    this.url = params.url;
    this.method = params.method;
    this.problemDetails = params.problemDetails;
    this.raw = params.raw;
  }
}

export type ProblemDetails = {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
  [key: string]: unknown; // extensions - keep it flexible; God knows what we will have in the future
};
