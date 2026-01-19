import axios from "axios";
import type { AxiosError } from "axios";
import { ApiError } from "@/shared/types/errors";
import { kindFromStatus } from "@/shared/api/errorHandlers/kindFromStatus";
import { buildMessage } from "@/shared/api/errors/messages";
import { tryExtractProblemDetails } from "@/shared/api/errors/utils";

export const normalizeAxiosError = (err: unknown): ApiError => {
  if (!axios.isAxiosError(err)) {
    return new ApiError({
      message: "Unexpected error.",
      kind: "Unknown",
      raw: err,
    });
  }

  const axErr = err as AxiosError;
  const status = axErr.response?.status;
  const method = axErr.config?.method?.toUpperCase();
  const url = axErr.config?.url;

  const isAborted =
    (typeof axErr.code === "string" && axErr.code === "ERR_CANCELED") ||
    axios.isCancel(axErr);

  if (isAborted) {
    return new ApiError({
      message: buildMessage({ kind: "Abort" }),
      kind: "Abort",
      status,
      url,
      method,
      raw: axErr,
    });
  }

  if (axErr.code === "ECONNABORTED" || axErr.code === "ETIMEDOUT") {
    return new ApiError({
      message: buildMessage({ kind: "Timeout" }),
      kind: "Timeout",
      status,
      url,
      method,
      raw: axErr,
    });
  }

  // Network error (=no response)
  if (!axErr.response) {
    return new ApiError({
      message: buildMessage({ kind: "Network" }),
      kind: "Network",
      url,
      method,
      raw: axErr,
    });
  }

  // Response with body: maybe ProblemDetails
  const pd = tryExtractProblemDetails(axErr.response.data);
  const kind = typeof status === "number" ? kindFromStatus(status, pd) : "Unknown";

  return new ApiError({
    message: buildMessage({ status, kind, pd, fallback: axErr.message }),
    kind,
    status,
    url,
    method,
    problemDetails: pd,
    raw: axErr.response.data,
  });
}
