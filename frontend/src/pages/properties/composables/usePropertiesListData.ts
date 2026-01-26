import { computed, ref, watch, type Ref } from "vue";
import { propertiesApi } from "@/shared/api/properties";

import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";
import type { PropertyFiltersValue } from "@/shared/types/properties";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";

type Params = {
  page: Ref<number>;
  pageSize: Ref<number>;
  filters: Ref<PropertyFiltersValue>;
};

export function usePropertiesListData(params: Params) {
  const state = ref<UIState>("loading");
  const error = ref<ApiError | null>(null);
  const data = ref<PagedResultDto<PropertyListItemDto> | null>(null);

  let requestId = 0;

  async function load(): Promise<void> {
    const rid = ++requestId;

    state.value = "loading";
    error.value = null;

    try {
      const response = await propertiesApi.list({
        page: params.page.value,
        pageSize: params.pageSize.value,
        ...params.filters.value,
      });

      if (rid !== requestId) return; // ignore stale response

      data.value = response;
      state.value = response.items.length === 0 ? "empty" : "success";
    } catch (e) {
      if (rid !== requestId) return;
      error.value = e as ApiError;
      state.value = "error";
    }
  }

  watch([params.page, params.pageSize, params.filters], () => void load(), {
    immediate: true,
    deep: true,
  });

  const items = computed(() => data.value?.items ?? []);
  const paging = computed(() => ({
    page: data.value?.page ?? params.page.value,
    pageSize: data.value?.pageSize ?? params.pageSize.value,
    totalItems: data.value?.totalItems ?? 0,
    totalPages: data.value?.totalPages ?? 0,
  }));

  return { state, error, items, paging, reload: load };
}
