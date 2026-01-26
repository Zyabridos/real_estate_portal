<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

type PageState = "loading" | "ready" | "error";

const state = ref<PageState>("loading");
const errorMessage = ref<string | null>(null);

// for now I just create template, evnt fetch from api
const totalItems = ref<number>(0);

const isLoading = computed(() => state.value === "loading");
const isError = computed(() => state.value === "error");
const isEmpty = computed(() => state.value === "ready" && totalItems.value === 0);

function retry(): void {
  state.value = "loading";
  errorMessage.value = null;

  // evnt fetch /api/leads
  state.value = "ready";
  totalItems.value = 0;
}

onMounted(() => {
  // evnt show real state
  state.value = "ready";
});
</script>

<template>
  <section
    class="w-full px-6 py-4"
    data-testid="leads-list-page"
    aria-labelledby="leads-list-title"
  >
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

    <!-- Status area -->
    <div aria-live="polite" aria-atomic="true" class="max-w-5xl">
      <!-- Loading -->
      <div
        v-if="isLoading"
        class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
        data-testid="leads-list-loading"
        role="status"
      >
        <div class="flex items-center gap-3">
          <span class="h-4 w-4 animate-spin rounded-full border-2 border-slate-300 border-t-slate-900" aria-hidden="true" />
          <span class="text-sm text-slate-700">{{ $t("common:loading") }}</span>
        </div>
      </div>

      <!-- Error -->
      <div
        v-else-if="isError"
        class="rounded-2xl border border-rose-200 bg-rose-50 p-5 text-rose-900"
        data-testid="leads-list-error"
        role="alert"
      >
        <p class="text-sm font-medium">
          {{ errorMessage || $t("common:error.unexpected") }}
        </p>

        <button
          type="button"
          class="mt-3 inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-slate-300"
          data-testid="leads-list-retry"
          @click="retry"
          :aria-label="$t('common:actions.retry')"
        >
          {{ $t("common:actions.retry") }}
        </button>
      </div>

      <!-- Empty -->
      <div
        v-else-if="isEmpty"
        class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
        data-testid="leads-list-empty"
        role="status"
      >
        <p class="text-sm text-slate-700">
          {{ $t("pages:leads.list.empty") }}
        </p>
      </div>

      <!-- Content placeholder -->
      <div
        v-else
        class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
        data-testid="leads-list-content"
      >
        <p class="text-sm text-slate-700">
          {{ $t("pages:leads.list.placeholder") }}
        </p>
      </div>
    </div>
  </section>
</template>
