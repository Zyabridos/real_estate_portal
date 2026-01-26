<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { leadsApi } from "@/shared/api/leads";
import type { ApiError } from "@/shared/types/errors";
import LoadingState from "@/shared/ui/states/LoadingState.vue";
import ErrorState from "@/shared/ui/states/ErrorState.vue";
import EmptyState from "@/shared/ui/states/EmptyState.vue";

type PageState = "loading" | "ready" | "error" | "empty";

const state = ref<PageState>("loading");
const error = ref<ApiError | null>(null);

const totalItems = ref<number>(0);

const isLoading = computed(() => state.value === "loading");
const isError = computed(() => state.value === "error");
const isEmpty = computed(() => state.value === "empty");

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;

  try {
    const result = await leadsApi.list({ page: 1, pageSize: 20 });

    totalItems.value = result.items.length;
    state.value = totalItems.value === 0 ? "empty" : "ready";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";
  }
}

function retry(): void {
  void load();
}

onMounted(() => {
  void load();
});
</script>

<template>
  <section class="w-full px-6 py-4" data-testid="leads-list-page" aria-labelledby="leads-list-title">
    <header class="mb-6">
      <h1
        id="leads-list-title"
        class="text-2xl font-semibold tracking-tight text-slate-900"
        data-testid="leads-list-title"
      >
        {{ $t("pages:leads.list.title") }}
      </h1>

      <p class="mt-1 text-sm text-slate-600" data-testid="leads-list-subtitle">
        {{ $t("pages:leads.list.subtitle") }}
      </p>
    </header>

    <!-- States -->
    <LoadingState v-if="isLoading" data-testid="leads-list-loading" />
    <ErrorState
      v-else-if="isError"
      data-testid="leads-list-error"
      :message="error?.message ?? $t('errors:messages.unexpected')"
      :onRetry="retry"
    />
    <EmptyState v-else-if="isEmpty" data-testid="leads-list-empty" />

    <!-- Content placeholder -->
    <div v-else class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm" data-testid="leads-list-content">
      <p class="text-sm text-slate-700">
        {{ $t("pages:leads.list.placeholder") }}
      </p>
      <p class="mt-2 text-xs text-slate-500" data-testid="leads-list-count">
        {{ $t("pages:leads.list.totalItems", { count: totalItems }) }}
      </p>
    </div>
  </section>
</template>
