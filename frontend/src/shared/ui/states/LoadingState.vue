<script setup lang="ts">
import { computed } from "vue";

import i18n from "@/shared/i18n";
import type { LoadingStateProps } from "@/shared/types/states";

const props = defineProps<LoadingStateProps>();

const resolvedTitle = computed(
  () => props.title?.trim() || i18n.t("common:states.loading.genericTitle")
);

const testId = computed(() => props.testId?.trim() || "loading-state");
</script>


<template>
  <section
    class="state state--loading"
    :data-testid="testId"
    role="status"
    aria-live="polite"
    :aria-label="$t('common:states.loading.ariaLabel')"
  >
    <div class="state__body">
      <div class="state__spinner" aria-hidden="true"></div>

      <h2 v-if="resolvedTitle" class="state__title" data-testid="loading-title">
        {{ resolvedTitle }}
      </h2>

      <p v-if="props.description" class="state__desc" data-testid="loading-description">
        {{ props.description }}
      </p>
    </div>
  </section>
</template>
