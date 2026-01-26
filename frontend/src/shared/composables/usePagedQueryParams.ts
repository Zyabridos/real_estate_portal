import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { mergeQuery } from "@/shared/router/query";

type Options = {
  defaultPage?: number;
  defaultPageSize?: number;
};

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
    await router.push({ query: mergeQuery(route.query, next) });
  }

  async function setPage(nextPage: number): Promise<void> {
    const safePage = Math.max(nextPage, 1);
    await setQuery({ page: safePage, pageSize: pageSize.value });
  }

  async function setPageSize(nextPageSize: number): Promise<void> {
    const safe = Number.isInteger(nextPageSize) && nextPageSize > 0 ? nextPageSize : defaultPageSize;
    await setQuery({ page: 1, pageSize: safe });
  }

  return { page, pageSize, setPage, setPageSize, setQuery };
}
