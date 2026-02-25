<script setup lang="ts">
import { computed } from "vue";
import i18n from "@/shared/i18n";
import type { ErrorStateProps } from "@/shared/types/states";

const props = defineProps<ErrorStateProps>();

const resolvedTitle = computed(
  () => props.title?.trim() || i18n.t("errors:generic.title")
);

const resolvedRetryLabel = computed(
  () => props.retryLabel?.trim() || i18n.t("errors:actions.retry")
);

const resolvedMessage = computed(
  () => props.message?.trim() || i18n.t("errors:message")
);

const testId = computed(() => props.testId?.trim() || "error-state");

const showRetry = computed(() => typeof props.onRetry === "function");
</script>

<template>
  <section
    :data-testid="testId"
    role="alert"
    aria-live="assertive"
    :aria-label="$t('error.ariaLabel')"
    class="rounded-2xl border border-rose-200 bg-rose-50 p-6 shadow-sm ring-1 ring-black/5"
  >
    <div class="flex items-start gap-4">
      <div
        class="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-rose-100 text-rose-800"
        aria-hidden="true"
      >
        <i class="pi pi-exclamation-triangle text-2xl" />
      </div>

      <div class="min-w-0 flex-1">
        <h2 class="text-lg font-semibold text-rose-900" data-testid="error-title">
          {{ resolvedTitle }}
        </h2>

        <p class="mt-1 text-sm leading-relaxed text-rose-800" data-testid="error-message">
          {{ resolvedMessage }}
        </p>

        <div v-if="showRetry" class="mt-4">
          <button
            type="button"
            class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-900 focus-visible:ring-offset-2"
            data-testid="retry-button"
            @click="props.onRetry"
            :aria-label="$t('error.retryAria')"
          >
            {{ resolvedRetryLabel }}
          </button>
        </div>
      </div>
    </div>
  </section>
</template>
