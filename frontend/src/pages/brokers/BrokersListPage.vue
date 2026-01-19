<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import type { ApiError } from "@/shared/types/errors"
import type { BrokerListItem } from "@/shared/types/brokers"
import type { PagedResult } from "@/shared/types/pagedResult"

const state = ref<"loading" | "success" | "empty" | "error">("loading");
const error = ref<ApiError | null>(null);

const data = ref<PagedResult<BrokerListItem> | null>(null);

const items = computed(() => data.value?.items ?? []);
const meta = computed(() => ({
  page: data.value?.page ?? 1,
  pageSize: data.value?.pageSize ?? 20,
  totalItems: data.value?.totalItems ?? 0,
  totalPages: data.value?.totalPages ?? 0,
}));

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    // evnt shared api client + routes.api + store
    // TODO: remove hardcore
    const res = await fetch("/api/brokers?page=1&pageSize=20");

    if (!res.ok) {
      const text = await res.text().catch(() => "");
      error.value = {
        status: res.status,
        message: text || `Request failed with status ${res.status}`,
      };
      state.value = "error";
      return;
    }

    const json = (await res.json()) as PagedResult<BrokerListItem>;
    data.value = json;

    state.value = json.items.length === 0 ? "empty" : "success";
  } catch (e) {
    error.value = { message: String(e) };
    state.value = "error";
  }
}

onMounted(load);
</script>

<template>
  <section class="w-full">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900">Brokers</h1>
          <p class="mt-1 text-sm text-slate-600">
            Browse brokers from the backend. Filtering and paging will be added next.
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

      <!-- States -->
      <div v-if="state === 'loading'" class="mt-8">
        <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="h-4 w-40 animate-pulse rounded bg-slate-200" />
          <div class="mt-4 space-y-3">
            <div class="h-4 w-full animate-pulse rounded bg-slate-200" />
            <div class="h-4 w-5/6 animate-pulse rounded bg-slate-200" />
            <div class="h-4 w-2/3 animate-pulse rounded bg-slate-200" />
          </div>
        </div>
      </div>

      <div v-else-if="state === 'error'" class="mt-8">
        <div class="rounded-2xl border border-rose-200 bg-rose-50 p-6">
          <div class="text-sm font-semibold text-rose-900">Failed to load brokers</div>
          <div class="mt-2 text-sm text-rose-800">
            <div v-if="error?.status">HTTP {{ error.status }}</div>
            <div>{{ error?.message }}</div>
          </div>
          <button
            type="button"
            class="mt-4 rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            @click="load"
          >
            Try again
          </button>
        </div>
      </div>

      <div v-else-if="state === 'empty'" class="mt-8">
        <div class="rounded-2xl border border-slate-200 bg-white p-6 text-sm text-slate-700">
          No brokers found.
        </div>
      </div>

      <!-- List -->
      <div v-else class="mt-8">
        <div class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
          <article
            v-for="b in items"
            :key="b.id"
            class="rounded-2xl border border-slate-200 bg-white shadow-sm hover:shadow-md transition-shadow"
          >
            <div class="p-5">
              <div class="flex items-start justify-between gap-3">
                <div>
                  <h2 class="text-base font-semibold text-slate-900">
                    {{ b.firstName}} {{ b.lastName }}
                  </h2>
                  <p class="mt-1 text-sm text-slate-600">
                    {{ b.email }} {{ b.phoneNumber }} {{ b.createdAt }}
                  </p>
                </div>
              </div>

              <div class="mt-4 flex items-center justify-between">
                <RouterLink
                  :to="`/brokers/${b.id}`"
                  class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
                >
                  View details
                </RouterLink>

                <div class="text-xs text-slate-500">
                  ID: <span class="font-mono">{{ b.id }}</span>
                </div>
              </div>
            </div>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>
