<script setup lang="ts">
import { computed, ref, watch } from "vue";

import i18n from "@/shared/i18n";
import { propertiesApi } from "@/features/properties/api/propertiesApi";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import PagedListShell from "@/shared/ui/pagination/PagedListShell.vue";

import PropertyFilters from "@/features/properties/list/components/PropertyFilters.vue";
import PropertyCard from "@/features/properties/list/components/PropertyCard.vue";

import type { ApiError } from "@/shared/types/errors";
import type { PropertyListItemDto } from "@/features/properties/api/dtos/property-list-item.dto";
import type { PropertyFiltersValue } from "@/entities/properties/model/types";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { UIState } from "@/shared/types/ui";

const state = ref<UIState>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PagedResultDto<PropertyListItemDto> | null>(null);

const filters = ref<PropertyFiltersValue>({});

function normalizeFilters(input: PropertyFiltersValue): PropertyFiltersValue {
  const out: PropertyFiltersValue = {};

  if (input.city?.trim()) out.city = input.city.trim();
  if (input.type) out.type = input.type;
  if (input.status) out.status = input.status;

  if (typeof input.minPrice === "number" && Number.isFinite(input.minPrice)) {
    out.minPrice = input.minPrice;
  }

  if (typeof input.maxPrice === "number" && Number.isFinite(input.maxPrice)) {
    out.maxPrice = input.maxPrice;
  }

  return out;
}

const items = computed(() => data.value?.items ?? []);
const listAriaLabel = computed(() => i18n.t("properties:list.ariaLabel"));

const { page, pageSize, setPage, setQuery } = usePagedQueryParams({
  defaultPage: 1,
  defaultPageSize: 20,
});

const currentPage = computed(() => data.value?.page ?? page.value);
const currentPageSize = computed(() => data.value?.pageSize ?? pageSize.value);

async function onGoToPage(nextPage: number): Promise<void> {
  await setPage(nextPage);
}

function filtersToQuery(f: PropertyFiltersValue): Record<string, string> {
  const q: Record<string, string> = {};

  if (f.city?.trim()) q.city = f.city.trim();
  if (f.type) q.type = f.type;
  if (f.status) q.status = f.status;
  if (typeof f.minPrice === "number") q.minPrice = String(f.minPrice);
  if (typeof f.maxPrice === "number") q.maxPrice = String(f.maxPrice);

  return q;
}

async function onApplyFilters(next: PropertyFiltersValue): Promise<void> {
  filters.value = normalizeFilters(next);

  await setQuery({
    ...filtersToQuery(filters.value),
    page: 1,
  });

  if (page.value === 1) {
    await load();
  }
}

async function onResetFilters(): Promise<void> {
  filters.value = {};

  await setQuery({
    city: undefined,
    type: undefined,
    status: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    page: 1,
  });

  if (page.value === 1) {
    await load();
  }
}

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    const response = await propertiesApi.list({
      page: page.value,
      pageSize: pageSize.value,
      ...filters.value,
    });

    data.value = response;
    state.value = response.items.length === 0 ? "empty" : "success";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";
  }
}

watch(
  () => [page.value, pageSize.value],
  () => {
    void load();
  },
  { immediate: true }
);
</script>
<template>
  <section class="w-full" :aria-label="listAriaLabel" data-testid="properties-page">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">
            {{ $t("properties:list.title") }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("properties:list.subtitle") }}
          </p>
        </div>

        <div
          class="flex items-center gap-3"
          role="group"
          :aria-label="$t('properties:list.pageActionsAriaLabel')"
        >
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="load"
            :aria-label="$t('common:actions.refreshAria')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <PropertyFilters
        :initial="filters"
        :disabled="state === 'loading'"
        @apply="onApplyFilters"
        @reset="onResetFilters"
      />

      <PagedListShell
        :page="currentPage"
        :pageSize="currentPageSize"
        :totalItems="data?.totalItems"
        :totalPages="data?.totalPages"
        @goToPage="onGoToPage"
      >

        <ErrorState
          v-if="state === 'error'"
          data-testid="properties-error"
          :message="error?.message ?? $t('errors:common.message.unexpected')"
          :onRetry="load"
        />

        <EmptyState v-else-if="state === 'empty'" data-testid="properties-empty" />

        <div v-else class="mt-8">
          <div
            class="flex flex-col gap-5"
            data-testid="properties-list"
            role="list"
            :aria-label="$t('properties:list.cardsAriaLabel')"
          >
            <PropertyCard v-for="p in items" :key="p.id" :property="p" />
          </div>
        </div>
      </PagedListShell>
    </div>
  </section>
</template>
