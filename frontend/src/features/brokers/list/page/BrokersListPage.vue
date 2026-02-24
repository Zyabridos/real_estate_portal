<script setup lang="ts">
import { computed, watch, ref } from "vue";
import { storeToRefs } from "pinia";

import i18n from "@/shared/i18n";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import Pagination from "@/shared/ui/pagination/Pagination.vue";
import PaginationMeta from "@/shared/ui/pagination/PaginationMeta.vue";
import BrokerCard from "@/features/brokers/list/components/BrokerCard.vue";

import { useBrokersStore } from "@/entities/brokers/model/brokersStore";
import type { BrokersListQuery } from "@/shared/types/queries";

const store = useBrokersStore();
const { brokers, listStatus, listError, lastPagedResult } = storeToRefs(store);

const { page, pageSize, setPage } = usePagedQueryParams({
  defaultPage: 1,
  defaultPageSize: 20,
});

const state = computed(() => listStatus.value);
const error = computed(() => listError.value);

const listAriaLabel = computed(() => i18n.t("pages:brokers.list.ariaLabel"));

async function load(): Promise<void> {
  const q: BrokersListQuery = { page: page.value, pageSize: pageSize.value };
  await store.fetchList(q);
}

async function onGoToPage(nextPage: number): Promise<void> {
  await setPage(nextPage);
  // load() вызовется watch'ем ниже
}

/**
 * ✅ Stable totals (без blinking)
 */
const stableTotals = ref<{ totalItems: number; totalPages: number } | null>(null);

watch(
  () => lastPagedResult.value,
  (res) => {
    if (!res) return;
    stableTotals.value = {
      totalItems: res.totalItems,
      totalPages: res.totalPages,
    };
  },
  { immediate: true }
);

const paging = computed(() => ({
  page: lastPagedResult.value?.page ?? page.value,
  pageSize: lastPagedResult.value?.pageSize ?? pageSize.value,
  totalItems: stableTotals.value?.totalItems ?? 0,
  totalPages: stableTotals.value?.totalPages ?? 0,
}));

const showMeta = computed(() => stableTotals.value !== null);

// ✅ loader: показываем на первом заходе и когда нет items
const showFullLoading = computed(
  () => state.value === "loading" && brokers.value.length === 0
);

// ✅ автоматическая загрузка на входе и при смене page/pageSize
watch(
  () => [page.value, pageSize.value],
  () => {
    void load();
  },
  { immediate: true }
);
</script>

<template>
  <section class="w-full" :aria-label="listAriaLabel">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">
            {{ $t("pages:brokers.list.title") }}
          </h1>
          <p class="mt-1 text-sm text-slate-600">
            {{ $t("pages:brokers.list.subtitle") }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:aria.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="load"
            :aria-label="$t('common:actions.refreshAria')"
            :disabled="state === 'loading'"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <PaginationMeta
        v-if="showMeta"
        class="mt-4"
        :page="paging.page"
        :pageSize="paging.pageSize"
        :totalItems="paging.totalItems"
      />
      <div v-else class="mt-4 h-5 w-48 animate-pulse rounded bg-slate-100" />

      <!-- States -->
      <LoadingState v-if="showFullLoading" />
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
          :aria-label="$t('pages:brokers.list.cardsAriaLabel')"
        >
          <BrokerCard v-for="b in brokers" :key="b.id" :broker="b" />
        </div>

        <div v-if="state === 'loading' && brokers.length > 0" class="mt-4 h-1 w-24 animate-pulse rounded bg-slate-100" />
      </div>

      <Pagination
        v-if="showMeta && paging.totalPages > 1"
        :page="paging.page"
        :totalPages="paging.totalPages"
        @goToPage="onGoToPage"
      />
    </div>
  </section>
</template>
