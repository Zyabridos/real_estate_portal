import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";

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

  function clampPage(next: number, totalPages?: number): number {
    const min = 1;
    const max = totalPages && totalPages > 0 ? totalPages : 1;
    return Math.min(Math.max(next, min), max);
  }

  async function setPage(nextPage: number, totalPages?: number): Promise<void> {
    const safePage = clampPage(nextPage, totalPages);

    await router.push({
      query: {
        ...route.query,
        page: String(safePage),
        pageSize: String(pageSize.value),
      },
    });
  }

  async function setPageSize(nextPageSize: number): Promise<void> {
    const safe = Number.isInteger(nextPageSize) && nextPageSize > 0 ? nextPageSize : defaultPageSize;

    await router.push({
      query: {
        ...route.query,
        page: "1", // change of pageSize always set paging to 1
        pageSize: String(safe),
      },
    });
  }

  return {
    page,
    pageSize,
    setPage,
    setPageSize,
  };
}
