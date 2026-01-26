<script setup lang="ts">
import { computed } from "vue";
import i18n from "@/shared/i18n";

import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import Pagination from "@/shared/ui/pagination/Pagination.vue";

import PropertyFilters from "@/pages/properties/components/PropertyFilters.vue";
import PropertyCardGrid from "@/pages/properties/components/PropertyCardGrid.vue";

import { usePropertiesQueryParams } from "@/pages/properties/composables/usePropertiesQueryParams";
import { usePropertiesListData } from "@/pages/properties/composables/usePropertiesListData";

const listAriaLabel = computed(() => i18n.t("pages:properties.list.ariaLabel"));

const { paging: queryPaging, filters, setFilters, resetFilters } = usePropertiesQueryParams({ debounceMs: 250 });

const { state, error, items, paging, reload } = usePropertiesListData({
  page: queryPaging.page,
  pageSize: queryPaging.pageSize,
  filters,
});

async function onGoToPage(nextPage: number): Promise<void> {
  await queryPaging.setPage(nextPage);
}
</script>

<template>
  <section class="w-full" :aria-label="listAriaLabel" data-testid="properties-page">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">
            {{ $t("pages:properties.list.title") }}
          </h1>
          <p class="mt-1 text-sm text-slate-600">
            {{ $t("pages:properties.list.subtitle") }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:aria.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="reload"
            :aria-label="$t('common:actions.refreshAria')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <PropertyFilters
        :model-value="filters"
        :disabled="state === 'loading'"
        @update:modelValue="setFilters"
        @reset="resetFilters"
      />

      <div class="mt-4 flex flex-wrap items-center gap-2 text-xs text-slate-600" role="status" aria-live="polite">
        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          {{ $t("common:pagination.page") }}:
          <span class="font-medium text-slate-900">{{ paging.page }}</span>
        </span>

        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          {{ $t("common:pagination.pageSize") }}:
          <span class="font-medium text-slate-900">{{ paging.pageSize }}</span>
        </span>

        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          {{ $t("common:pagination.total") }}:
          <span class="font-medium text-slate-900">{{ paging.totalItems }}</span>
        </span>
      </div>

      <LoadingState v-if="state === 'loading'" data-testid="properties-loading" />
      <ErrorState
        v-else-if="state === 'error'"
        data-testid="properties-error"
        :message="error?.message ?? $t('errors:messages.unexpected')"
        :onRetry="reload"
      />
      <EmptyState v-else-if="state === 'empty'" data-testid="properties-empty" />

      <PropertyCardGrid v-else :items="items" />

      <Pagination
        v-if="state === 'success' && paging.totalPages > 1"
        :page="paging.page"
        :totalPages="paging.totalPages"
        @goToPage="onGoToPage"
      />
    </div>
  </section>
</template>
