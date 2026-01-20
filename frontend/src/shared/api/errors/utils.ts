import type { ProblemDetails } from "@/shared/types/errors";

export const isPlainObject = (value: unknown): value is Record<string, unknown> => {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

export const tryExtractProblemDetails = (data: unknown): ProblemDetails | undefined => {
  if (!isPlainObject(data)) return undefined;

  const hasAny =
    "title" in data ||
    "status" in data ||
    "detail" in data ||
    "type" in data ||
    "errors" in data;

  return hasAny ? (data as ProblemDetails) : undefined;
}
