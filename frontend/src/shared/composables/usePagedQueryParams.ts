import { computed } from "vue";
import { useRoute, useRouter, type LocationQueryRaw, type LocationQueryValueRaw } from "vue-router";

type Options = {
  defaultPage?: number;
  defaultPageSize?: number;
};

type QueryValue = LocationQueryValueRaw | LocationQueryValueRaw[] | undefined;

// helper: converts unknown to a query-compatible value (or undefined to remove)
// TODO: move to separate func?
function toQueryValue(v: unknown): QueryValue {
  if (v === undefined) return undefined;          // means "remove key"
  if (v === null) return undefined;               // treat null as remove
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

function mergeQuery(base: LocationQueryRaw, next: Record<string, unknown>): LocationQueryRaw {
  const out: LocationQueryRaw = { ...base };

  for (const [k, v] of Object.entries(next)) {
    const qv = toQueryValue(v);
    if (qv === undefined) delete out[k];
    else out[k] = qv;
  }

  return out;
}

export function usePagedQueryParams(options: Options = {}) {
  const route = useRoute();
  const router = useRouter();

  const defaultPage = options.defaultPage ?? 1;
  const defaultPageSize = options.defaultPageSize ?? 20;

  const page = computed(() => {
    const p = Number(route.query.page);
    return Number.isInteger(p) && p > 0 ? p : defaultPage;
  });

  const pageSize = computed(() => {
    const ps = Number(route.query.pageSize);
    return Number.isInteger(ps) && ps > 0 ? ps : defaultPageSize;
  });

  async function setQuery(next: Record<string, unknown>): Promise<void> {
    await router.push({
      query: mergeQuery(route.query, next),
    });
  }

  function clampPage(next: number): number {
    const min = 1;
    return Math.max(next, min);
  }

  async function setPage(nextPage: number): Promise<void> {
    const safePage = clampPage(nextPage);
    await setQuery({
      page: safePage,
      pageSize: pageSize.value,
    });
  }


  async function setPageSize(nextPageSize: number): Promise<void> {
    const safe = Number.isInteger(nextPageSize) && nextPageSize > 0 ? nextPageSize : defaultPageSize;
    await setQuery({
      page: 1,
      pageSize: safe,
    });
  }

  return { page, pageSize, setPage, setPageSize, setQuery };
}
