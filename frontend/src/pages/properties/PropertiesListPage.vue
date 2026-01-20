<script setup lang="ts">
import { computed, ref, watch } from "vue";

import i18n from "@/shared/i18n";
import { propertiesApi } from "@/shared/api/properties";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";

import Pagination from "@/shared/ui/pagination/Pagination.vue";
import PropertyFilters from "@/pages/properties/components/PropertyFilters.vue";

import type { ApiError } from "@/shared/types/errors";
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";
import type { PropertyFiltersValue } from "@/shared/types/properties";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { UIStatus } from "@/shared/types/ui";

// TODO: the component is getting too big. Refactor to smaller
// TODO: consider filtration on frontend (?), so it will load as we write without hitting "apply"

const state = ref<UIStatus>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PagedResultDto<PropertyListItemDto> | null>(null);

const { page, pageSize, setPage } = usePagedQueryParams({ defaultPage: 1, defaultPageSize: 20 });

// ---------- filters state (applied) ----------
const filters = ref<PropertyFiltersValue>({});

function normalizeFilters(input: PropertyFiltersValue): PropertyFiltersValue {
  const out: PropertyFiltersValue = {};

  if (input.city?.trim()) out.city = input.city.trim();
  if (input.type) out.type = input.type;
  if (input.status) out.status = input.status;

  if (typeof input.minPrice === "number" && Number.isFinite(input.minPrice)) out.minPrice = input.minPrice;
  if (typeof input.maxPrice === "number" && Number.isFinite(input.maxPrice)) out.maxPrice = input.maxPrice;

  return out;
}

// ---------- derived ----------
const paging = computed(() => ({
  page: data.value?.page ?? page.value,
  pageSize: data.value?.pageSize ?? pageSize.value,
  totalItems: data.value?.totalItems ?? 0,
  totalPages: data.value?.totalPages ?? 0,
}));

const items = computed(() => data.value?.items ?? []);
const listAriaLabel = computed(() => i18n.t("pages:properties.list.ariaLabel"));

// ---------- actions ----------
async function onGoToPage(nextPage: number): Promise<void> {
  await setPage(nextPage, paging.value.totalPages);
}

async function onApplyFilters(next: PropertyFiltersValue): Promise<void> {
  filters.value = normalizeFilters(next);

  // apply всегда сбрасывает на page=1
  await setPage(1);

  // если и так на 1 — watch не сработает
  if (page.value === 1) {
    await load();
  }
}

async function onResetFilters(): Promise<void> {
  filters.value = {};

  await setPage(1);

  if (page.value === 1) {
    await load();
  }
}

// ---------- data ----------
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

    if (import.meta.env.DEV) {
      // eslint-disable-next-line no-console
      console.error("Failed to fetch properties", e);
    }
  }
}

watch(
  () => [page.value, pageSize.value],
  () => load(),
  { immediate: true }
);
</script>

<template>
  <section class="w-full" :aria-label="listAriaLabel">
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

      <!-- Meta -->
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

      <!-- States -->
      <LoadingState v-if="state === 'loading'" />
      <ErrorState
        v-else-if="state === 'error'"
        :message="error?.message ?? $t('errors:messages.unexpected')"
        :onRetry="load"
      />
      <EmptyState v-else-if="state === 'empty'" />

      <!-- List -->
      <div v-else class="mt-8">
        <div
          class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3"
          role="list"
          :aria-label="$t('pages:properties.list.cardsAriaLabel')"
        >
          <article
            v-for="p in items"
            :key="p.id"
            class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
            role="listitem"
            :aria-label="$t('pages:properties.list.cardAriaLabel', { id: p.id })"
          >
            <div class="p-5">
              <div class="flex items-start justify-between gap-3">
                <div>
                  <h2 class="text-base font-semibold text-slate-900">
                    {{ p.title }}
                  </h2>
                  <p class="mt-1 text-sm text-slate-600">
                    {{ p.city }} • {{ p.type }} • {{ p.status }}
                  </p>
                </div>

                <div class="text-right">
                  <div
                    class="text-sm font-semibold text-slate-900"
                    :aria-label="$t('entities:property.priceValueAriaLabel', { value: p.price.toLocaleString() })"
                  >
                    {{ p.price.toLocaleString() }}
                  </div>
                  <div class="text-xs text-slate-500">
                    {{ $t("common:currency.nok") }}
                  </div>
                </div>
              </div>

              <div class="mt-4 flex items-center justify-between">
                <RouterLink
                  :to="`/properties/${p.id}`"
                  class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
                  :aria-label="$t('pages:properties.list.viewDetailsAriaLabel', { id: p.id })"
                >
                  {{ $t("common:actions.viewDetails") }}
                </RouterLink>

                <div class="text-xs text-slate-500">
                  {{ $t("common:pagination.idShort") }}:
                  <span class="font-mono">{{ p.id }}</span>
                </div>
              </div>
            </div>
          </article>
        </div>
      </div>

      <Pagination
        v-if="state === 'success' && paging.totalPages > 1"
        :page="paging.page"
        :totalPages="paging.totalPages"
        @goToPage="onGoToPage"
      />
    </div>
  </section>
</template>
