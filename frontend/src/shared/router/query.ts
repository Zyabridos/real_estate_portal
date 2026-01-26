import type { LocationQueryRaw, LocationQueryValueRaw } from "vue-router";

export type QueryValue = LocationQueryValueRaw | LocationQueryValueRaw[] | undefined;

export function toQueryValue(v: unknown): QueryValue {
  if (v === undefined || v === null) return undefined;

  if (Array.isArray(v)) {
    const arr = v
      .map((x) => toQueryValue(x))
      .flat()
      .filter((x): x is LocationQueryValueRaw => x !== undefined && !Array.isArray(x));
    return arr.length ? arr : undefined;
  }

  if (typeof v === "string") return v.trim() ? v : undefined;
  if (typeof v === "number") return Number.isFinite(v) ? String(v) : undefined;
  if (typeof v === "boolean") return v ? "true" : "false";
  return String(v);
}

export function mergeQuery(base: LocationQueryRaw, next: Record<string, unknown>): LocationQueryRaw {
  const out: LocationQueryRaw = { ...base };

  for (const [k, v] of Object.entries(next)) {
    const qv = toQueryValue(v);
    if (qv === undefined) delete out[k];
    else out[k] = qv;
  }

  return out;
}
