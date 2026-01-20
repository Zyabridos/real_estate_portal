<script setup lang="ts">
import { computed } from "vue";
import type { LoadingStateProps } from "@/shared/types/states";

const props = defineProps<LoadingStateProps>();

const resolvedTitle = computed(() => props.title?.trim() || "Loading…");
const testId = computed(() => props.testId?.trim() || "loading-state");
</script>

<template>
  <section class="state state--loading" :data-testid="testId">
    <div class="state__body">
      <div class="state__spinner" aria-hidden="true"></div>

      <h2 v-if="resolvedTitle" class="state__title" data-testid="loading-title">
        {{ resolvedTitle }}
      </h2>

      <p v-if="description" class="state__desc" data-testid="loading-description">
        {{ description }}
      </p>
    </div>
  </section>
</template>
