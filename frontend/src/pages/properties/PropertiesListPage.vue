<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useRoute } from "vue-router";

import { propertiesApi } from "@/shared/api/properties";
import { EmptyState, ErrorState, LoadingState } from "@/shared/ui/states";

import type { ApiError } from "@/shared/types/errors";
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";
import type { PagedResultDto } from "@/shared/api/dtos/common/paged-result.dto";
import type { UIStatus } from "@/shared/types/ui";

const state = ref<UIStatus>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PagedResultDto<PropertyListItemDto> | null>(null);

const route = useRoute();

const page = computed(() => {
  const p = Number(route.query.page);
  return Number.isInteger(p) && p > 0 ? p : 1;
});

const pageSize = computed(() => {
  const ps = Number(route.query.pageSize);
  return Number.isInteger(ps) && ps > 0 ? ps : 20;
});


const items = computed(() => data.value?.items ?? []);
const meta = computed(() => ({
  page: data.value?.page ?? page.value,
  pageSize: data.value?.pageSize ?? pageSize.value,
  totalItems: data.value?.totalItems ?? 0,
  totalPages: data.value?.totalPages ?? 0,
}));

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    const response = await propertiesApi.list({
      page: page.value,
      pageSize: pageSize.value,
    });

    data.value = response;
    state.value = response.items.length === 0 ? "empty" : "success";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";

    if (import.meta.env.DEV) {
      console.error("Failed to fetch properties", e);
    }
  }
}

watch(
  () => [page.value, pageSize.value],
  () => {
    load();
  },
  { immediate: true }
);
// TODO: remove hardcoded texts in templates
</script>

<template>
  <section class="w-full">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">Properties</h1>
          <p class="mt-1 text-sm text-slate-600">
            Browse properties from the backend. Filtering and paging will be added next.
          </p>
        </div>

        <div class="flex items-center gap-3">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            @click="load"
          >
            Refresh
          </button>
        </div>
      </div>

      <!-- Meta -->
      <div class="mt-4 flex flex-wrap items-center gap-2 text-xs text-slate-600">
        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          Page: <span class="font-medium text-slate-900">{{ meta.page }}</span>
        </span>
        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          Page size: <span class="font-medium text-slate-900">{{ meta.pageSize }}</span>
        </span>
        <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
          Total: <span class="font-medium text-slate-900">{{ meta.totalItems }}</span>
        </span>
      </div>

      <LoadingState v-if="state === 'loading'" />
      <ErrorState v-else-if="state === 'error'" :message="error?.message ?? 'Unexpected error'" :onRetry="load" />
      <EmptyState v-else-if="state === 'empty'" />

      <!-- List -->
      <div v-else class="mt-8">
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
          <article
            v-for="p in items"
            :key="p.id"
            class="rounded-2xl border border-slate-200 bg-white shadow-sm hover:shadow-md transition-shadow"
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
                  <div class="text-sm font-semibold text-slate-900">
                    {{ p.price.toLocaleString() }}
                  </div>
                  <div class="text-xs text-slate-500">NOK</div>
                </div>
              </div>

              <div class="mt-4 flex items-center justify-between">
                <RouterLink
                  :to="`/properties/${p.id}`"
                  class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
                >
                  View details
                </RouterLink>

                <div class="text-xs text-slate-500">
                  ID: <span class="font-mono">{{ p.id }}</span>
                </div>
              </div>
            </div>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>
