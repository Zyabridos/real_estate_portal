
<script setup lang="ts">
import { computed } from "vue";
import type { ErrorStateProps } from "@/shared/types/states";

const props = defineProps<ErrorStateProps>();

const resolvedTitle = computed(() => props.title?.trim() || "Something went wrong");
const resolvedRetryLabel = computed(() => props.retryLabel?.trim() || "Try again");
const testId = computed(() => props.testId?.trim() || "error-state");
</script>
<template>
  <section class="state state--error" :data-testid="testId">
    <div class="state__body">
      <h2 class="state__title" data-testid="error-title">
        {{ resolvedTitle }}
      </h2>

      <p class="state__desc" data-testid="error-message">
        {{ message }}
      </p>

      <button
        v-if="onRetry"
        type="button"
        class="state__action"
        data-testid="retry-button"
        @click="onRetry"
      >
        {{ resolvedRetryLabel }}
      </button>
    </div>
  </section>
</template>

