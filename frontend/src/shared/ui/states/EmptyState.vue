<script setup lang="ts">

import {computed} from "vue";
import type { EmptyStatePropsProps } from "@/shared/types/states";

const props = defineProps<EmptyStatePropsProps>();

const resolvedTitle = computed(() => props.title?.trim() || "No results");
const resolvedMessage = computed(() => props.message?.trim() || "Try adjusting your filters.");
const resolvedActionLabel = computed(() => props.actionLabel?.trim() || "Clear filters");
const testId = computed(() => props.testId?.trim() || "empty-state");
</script>

<template>
  <section class="state state--empty" :data-testid="testId">
    <div class="state__body">
      <h2 class="state__title" data-testid="empty-title">
        {{ resolvedTitle }}
      </h2>

      <p class="state__desc" data-testid="empty-message">
        {{ resolvedMessage }}
      </p>

      <button
        v-if="onAction"
        type="button"
        class="state__action"
        data-testid="empty-action"
        @click="onAction"
      >
        {{ resolvedActionLabel }}
      </button>
    </div>
  </section>
</template>

