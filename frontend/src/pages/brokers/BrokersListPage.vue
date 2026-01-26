<script setup lang="ts">
import { computed, ref, watch } from "vue";

import i18n from "@/shared/i18n";
import { brokersApi } from "@/shared/api/brokers";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";
import { usePagedQueryParams } from "@/shared/composables/usePagedQueryParams";
import Pagination from "@/shared/ui/pagination/Pagination.vue";
import PaginationMeta from "@/shared/ui/pagination/PaginationMeta.vue";
import BrokerCard from "@/pages/brokers/components/BrokerCard.vue";

import type { ApiError } from "@/shared/types/errors";
import type { BrokerListItemDto } from "@/shared/api/dtos/brokers/broker-list-item.dto";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { UIState } from "@/shared/types/ui";

const state = ref<UIState>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PagedResultDto<BrokerListItemDto> | null>(null);

const { page, pageSize, setPage } = usePagedQueryParams({ defaultPage: 1, defaultPageSize: 20 });

// Show data from query until we will get full response from backend, so we do not break UI
const paging = computed(() => ({
  page: data.value?.page ?? page.value,
  pageSize: data.value?.pageSize ?? pageSize.value,
  totalItems: data.value?.totalItems ?? 0,
  totalPages: data.value?.totalPages ?? 0,
}));

const items = computed(() => data.value?.items ?? []);

const listAriaLabel = computed(() => i18n.t("pages:brokers.list.ariaLabel"));

async function onGoToPage(nextPage: number): Promise<void> {
  await setPage(nextPage);
}

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    const response = await brokersApi.list({
      page: page.value,
      pageSize: pageSize.value,
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
            {{ $t('pages:brokers.list.title') }}
          </h1>
          <p class="mt-1 text-sm text-slate-600">
            {{ $t('pages:brokers.list.subtitle') }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:aria.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="load"
            :aria-label="$t('common:actions.refreshAria')"
          >
            {{ $t('common:actions.refresh') }}
          </button>
        </div>
      </div>

      <PaginationMeta
        class="mt-4"
        :page="paging.page"
        :pageSize="paging.pageSize"
        :totalItems="paging.totalItems"
      />

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
          :aria-label="$t('pages:brokers.list.cardsAriaLabel')"
        >
          <BrokerCard v-for="b in items" :key="b.id" :broker="b" />
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
