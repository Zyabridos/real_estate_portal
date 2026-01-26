import type { LocationQuery } from "vue-router";
import type { PropertyFiltersValue, PropertyStatus, PropertyType } from "@/shared/types/properties";

const typeSet = new Set<PropertyType>(["Apartment", "House", "Commercial"]);
const statusSet = new Set<PropertyStatus>(["Active", "Sold"]);

function readQueryString(q: LocationQuery, key: string): string | undefined {
  const v = q[key];
  const raw = Array.isArray(v) ? v[0] : v;
  return typeof raw === "string" && raw.trim() ? raw.trim() : undefined;
}

function readQueryNumber(q: LocationQuery, key: string): number | undefined {
  const s = readQueryString(q, key);
  if (!s) return undefined;
  const n = Number(s);
  if (!Number.isFinite(n) || n < 0) return undefined;
  return n;
}

function readEnum<T extends string>(value: string | undefined, allowed: Set<T>): T | undefined {
  if (!value) return undefined;
  return allowed.has(value as T) ? (value as T) : undefined;
}

export function parsePropertyFiltersFromQuery(q: LocationQuery): PropertyFiltersValue {
  const city = readQueryString(q, "city");
  const type = readEnum(readQueryString(q, "type"), typeSet);
  const status = readEnum(readQueryString(q, "status"), statusSet);
  const minPrice = readQueryNumber(q, "minPrice");
  const maxPrice = readQueryNumber(q, "maxPrice");

  const out: PropertyFiltersValue = {};
  if (city) out.city = city;
  if (type) out.type = type;
  if (status) out.status = status;
  if (minPrice != null) out.minPrice = minPrice;
  if (maxPrice != null) out.maxPrice = maxPrice;

  return out;
}

export function propertyFiltersToQuery(f: PropertyFiltersValue): Record<string, string> {
  const q: Record<string, string> = {};

  if (f.city?.trim()) q.city = f.city.trim();
  if (f.type) q.type = f.type;
  if (f.status) q.status = f.status;
  if (typeof f.minPrice === "number") q.minPrice = String(f.minPrice);
  if (typeof f.maxPrice === "number") q.maxPrice = String(f.maxPrice);

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
