<script setup lang="ts">
import { computed } from "vue";
import i18n from "@/shared/i18n";
import type { ErrorStateProps } from "@/shared/types/states";

const props = defineProps<ErrorStateProps>();

const resolvedTitle = computed(
  () => props.title?.trim() || i18n.t("states:error.title")
);

const resolvedRetryLabel = computed(
  () => props.retryLabel?.trim() || i18n.t("states:error.retry")
);

const resolvedMessage = computed(
  () => props.message?.trim() || i18n.t("states:error.message")
);

const testId = computed(() => props.testId?.trim() || "error-state");

const showRetry = computed(() => typeof props.onRetry === "function");
</script>

<template>
  <section
    class="state state--error"
    :data-testid="testId"
    role="alert"
    aria-live="assertive"
    :aria-label="$t('states:error.ariaLabel')"
  >
    <div class="state__body">
      <h2 class="state__title" data-testid="error-title">
        {{ resolvedTitle }}
      </h2>

      <p class="state__desc" data-testid="error-message">
        {{ resolvedMessage }}
      </p>

      <button
        v-if="showRetry"
        type="button"
        class="state__action"
        data-testid="retry-button"
        @click="props.onRetry"
        :aria-label="$t('states:error.retryAria')"
      >
        {{ resolvedRetryLabel }}
      </button>
    </div>
  </section>
</template>
