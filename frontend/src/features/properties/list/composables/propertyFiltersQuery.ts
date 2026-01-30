import type { LocationQuery, LocationQueryValue } from "vue-router";
import type { PropertyFiltersValue, PropertyStatus, PropertyType } from "@/entities/properties/model/types";

function first(q: LocationQueryValue | LocationQueryValue[] | undefined): string | undefined {
  if (typeof q === "string") return q;
  if (Array.isArray(q)) return typeof q[0] === "string" ? q[0] : undefined;
  return undefined;
}

function readTrimmed(query: LocationQuery, key: string): string | undefined {
  const v = first(query[key]);
  if (!v) return undefined;
  const s = v.trim();
  return s.length ? s : undefined;
}

function readNonNegativeNumber(query: LocationQuery, key: string): number | undefined {
  const v = readTrimmed(query, key);
  if (!v) return undefined;
  const n = Number(v);
  if (!Number.isFinite(n) || n < 0) return undefined;
  return n;
}

export function parsePropertyFiltersFromQuery(query: LocationQuery): PropertyFiltersValue {
  const out: PropertyFiltersValue = {};

  const city = readTrimmed(query, "city");
  if (city) out.city = city;

  const type = readTrimmed(query, "type");
  if (type) out.type = type as PropertyType;

  const status = readTrimmed(query, "status");
  if (status) out.status = status as PropertyStatus;

  const minPrice = readNonNegativeNumber(query, "minPrice");
  if (minPrice != null) out.minPrice = minPrice;

  const maxPrice = readNonNegativeNumber(query, "maxPrice");
  if (maxPrice != null) out.maxPrice = maxPrice;

  return out;
}

export function propertyFiltersToQuery(f: PropertyFiltersValue): Record<string, string> {
  const q: Record<string, string> = {};

  if (f.city?.trim()) q.city = f.city.trim();
  if (f.type) q.type = String(f.type);
  if (f.status) q.status = String(f.status);

  if (typeof f.minPrice === "number" && Number.isFinite(f.minPrice)) q.minPrice = String(f.minPrice);
  if (typeof f.maxPrice === "number" && Number.isFinite(f.maxPrice)) q.maxPrice = String(f.maxPrice);

  return q;
}

export function clearPropertyFiltersQuery(): Record<string, undefined> {
  return {
    city: undefined,
    type: undefined,
    status: undefined,
    minPrice: undefined,
    maxPrice: undefined,
  };
}
