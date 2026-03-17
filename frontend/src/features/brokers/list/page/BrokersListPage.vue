<script setup lang="ts">
import { computed, watch } from "vue";
import { storeToRefs } from "pinia";

import i18n from "@/shared/i18n";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import PagedListShell from "@/shared/ui/pagination/PagedListShell.vue";
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

const listAriaLabel = computed(() => i18n.t("brokers:list.ariaLabel"));

const currentPage = computed(() => lastPagedResult.value?.page ?? page.value);
const currentPageSize = computed(() => lastPagedResult.value?.pageSize ?? pageSize.value);

const showFullLoading = computed(
  () => state.value === "loading" && brokers.value.length === 0
);

async function load(): Promise<void> {
  const q: BrokersListQuery = {
    page: page.value,
    pageSize: pageSize.value,
  };

  await store.fetchList(q);
}

async function onGoToPage(nextPage: number): Promise<void> {
  await setPage(nextPage);
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
  <section class="w-full" :aria-label="listAriaLabel">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">
            {{ $t("brokers:list.title") }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("brokers:list.subtitle") }}
          </p>
        </div>

        <div
          class="flex items-center gap-3"
          role="group"
          :aria-label="$t('brokers:list.pageActionsAriaLabel')"
        >
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

      <PagedListShell
        :page="currentPage"
        :pageSize="currentPageSize"
        :totalItems="lastPagedResult?.totalItems"
        :totalPages="lastPagedResult?.totalPages"
        @goToPage="onGoToPage"
      >

        <LoadingState
          v-if="state === 'loading'"
          testId="properties-loading"
          :title="$t('common:states.loading.genericTitle')"
          :subtitle="$t('brokers:list.subtitle')"
        />

        <ErrorState
          v-else-if="state === 'error'"
          :message="error?.message ?? $t('errors:common.message.unexpected')"
          :onRetry="load"
        />

        <EmptyState v-else-if="state === 'empty'" />

        <div v-else class="mt-8">
          <div
            class="flex flex-col gap-4"
            role="list"
            :aria-label="$t('brokers:list.cardsAriaLabel')"
          >
            <BrokerCard v-for="b in brokers" :key="b.id" :broker="b" />
          </div>

          <div
            v-if="state === 'loading' && brokers.length > 0"
            class="mt-4 h-1 w-24 animate-pulse rounded bg-slate-100"
          />
        </div>
      </PagedListShell>
    </div>
  </section>
</template>
