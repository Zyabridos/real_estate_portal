import type { SortDirection } from "@/shared/types/queries.ts";

export type PagingSortDefaults = {
  page: number;
  pageSize: number;
  sortBy: string;
  sortDirection: SortDirection;
};

const trimOrUndef = (v: unknown): string | undefined => {
  if (typeof v !== "string") return undefined;
  const s = v.trim();
  return s.length ? s : undefined;
}

const normalizeInt = (v: unknown, fallback: number, min: number, max: number): number => {
  const n =
    typeof v === "number"
      ? v
      : typeof v === "string" && v.trim().length
        ? Number(v)
        : NaN;

  if (!Number.isFinite(n)) return fallback;

  const i = Math.trunc(n);
  return Math.max(min, Math.min(max, i));
}

const normalizeSortDirection = (v: unknown, fallback: SortDirection): SortDirection => {
  if (v === "asc") return "asc";
  if (v === "desc") return "desc";
  return fallback;
}
export function normalizePagedQuery<T extends Record<string, unknown>>(
  query: T,
  defaults: PagingSortDefaults,
): Record<string, unknown> {
  const result: Record<string, unknown> = {};

  // 1) Trim strings and drop empty strings / null / undefined
  Object.entries(query).forEach(([key, value]) => {
    if (value === undefined || value === null) return;

    if (typeof value === "string") {
      const trimmed = value.trim();
      if (!trimmed) return;
      result[key] = trimmed;
      return;
    }

    result[key] = value;
  });

  // 2) Paging
  result.page = normalizeInt(result.page, defaults.page, 1, 10_000);
  result.pageSize = normalizeInt(result.pageSize, defaults.pageSize, 1, 100);

  // 3) Sorting
  const sortBy = trimOrUndef(result.sortBy) ?? defaults.sortBy;
  result.sortBy = sortBy;

  result.sortDirection = normalizeSortDirection(result.sortDirection, defaults.sortDirection);

  return result;
}

export default normalizePagedQuery;
