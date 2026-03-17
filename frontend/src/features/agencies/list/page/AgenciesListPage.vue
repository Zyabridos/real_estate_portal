<script setup lang="ts">
import { computed, watch } from "vue";
import { storeToRefs } from "pinia";

import i18n from "@/shared/i18n";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import PagedListShell from "@/shared/ui/pagination/PagedListShell.vue";

import AgencyCard from "@/features/agencies/list/components/AgencyCard.vue";
import { useAgenciesStore } from "@/entities/agencies/model/agenciesStore";
import type { AgenciesListQuery } from "@/shared/types/queries";

const store = useAgenciesStore();
const { agencies, listStatus, listError, lastPagedResult } = storeToRefs(store);

const { page, pageSize, setPage } = usePagedQueryParams({
  defaultPage: 1,
  defaultPageSize: 20,
});

const state = computed(() => listStatus.value);
const error = computed(() => listError.value);
const listAriaLabel = computed(() => i18n.t("agencies:list.ariaLabel"));

const currentPage = computed(() => lastPagedResult.value?.page ?? page.value);
const currentPageSize = computed(() => lastPagedResult.value?.pageSize ?? pageSize.value);

const showFullLoading = computed(
  () => state.value === "loading" && agencies.value.length === 0,
);

async function load(): Promise<void> {
  const q: AgenciesListQuery = {
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
  { immediate: true },
);
</script>
<template>
  <section class="w-full" :aria-label="listAriaLabel" data-testid="agencies-list-page">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">
            {{ $t("agencies:list.title") }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("agencies:list.subtitle") }}
          </p>
        </div>

        <div
          class="flex items-center gap-3"
          role="group"
          :aria-label="$t('common:app.pageActions')"
        >
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="load"
            :disabled="state === 'loading'"
            :aria-label="$t('common:actions.refreshAria')"
            data-testid="refresh-button"
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
          v-if="showFullLoading"
          testId="properties-loading"
          :title="$t('common:states.loading.genericTitle')"
          :subtitle="$t('properties:list.subtitle')"
        />

        <ErrorState
          v-else-if="state === 'error'"
          :message="error?.message ?? $t('errors:common.message.unexpected')"
          :onRetry="load"
        />

        <EmptyState v-else-if="state === 'empty'" />

        <div v-else class="mt-8">
          <div
            class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3"
            role="list"
            :aria-label="$t('agencies:list.cardsAriaLabel')"
          >
            <AgencyCard v-for="a in agencies" :key="a.id" :agency="a" />
          </div>

          <div
            v-if="state === 'loading' && agencies.length > 0"
            class="mt-4 h-1 w-24 animate-pulse rounded bg-slate-100"
          />
        </div>
      </PagedListShell>
    </div>
  </section>
</template>
