import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { mergeQuery } from "@/shared/router/query";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import useDebouncedFn from "@/shared/composables/useDebouncedFn";

import type { PropertyFiltersValue } from "@/shared/types/properties";
import {
  clearPropertyFiltersQuery,
  parsePropertyFiltersFromQuery,
  propertyFiltersToQuery,
} from "@/pages/properties/utils/propertyFiltersQuery";

type Options = { debounceMs?: number };

export function usePropertiesQueryParams(options: Options = {}) {
  const route = useRoute();
  const router = useRouter();

  const paging = usePagedQueryParams({ defaultPage: 1, defaultPageSize: 20 });
  const filters = computed(() => parsePropertyFiltersFromQuery(route.query));

  async function replaceQuery(patch: Record<string, unknown>): Promise<void> {
    await router.replace({ query: mergeQuery(route.query, patch) });
  }

  async function setFiltersNow(next: PropertyFiltersValue): Promise<void> {
    await replaceQuery({
      ...clearPropertyFiltersQuery(),
      ...propertyFiltersToQuery(next),
      page: 1,
      pageSize: paging.pageSize.value,
    });
  }

  const setFilters = useDebouncedFn(setFiltersNow, options.debounceMs ?? 250);

  async function resetFilters(): Promise<void> {
    await replaceQuery({
      ...clearPropertyFiltersQuery(),
      page: 1,
      pageSize: paging.pageSize.value,
    });
  }

  return { paging, filters, setFilters, resetFilters };
}
