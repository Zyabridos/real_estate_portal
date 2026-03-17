<script setup lang="ts">
import { computed } from "vue";

import i18n from "@/shared/i18n";
import type { EmptyStatePropsProps } from "@/shared/types/states";

const props = defineProps<EmptyStatePropsProps>();

const resolvedTitle = computed(() => props.title?.trim() || String(i18n.t("common:states.empty.title")));
const resolvedMessage = computed(() => props.message?.trim() || String(i18n.t("common:states.empty.message")));
const resolvedActionLabel = computed(() => props.actionLabel?.trim() || String(i18n.t("common:states.empty.action")));
const testId = computed(() => props.testId?.trim() || "empty-state");
</script>

<template>
  <section
    class="state state--empty"
    :data-testid="testId"
    role="status"
    aria-live="polite"
    :aria-label="$t('common:states.empty.ariaLabel')"
  >
    <div class="state__body">
      <h2 class="state__title" data-testid="empty-title">
        {{ resolvedTitle }}
      </h2>

      <p class="state__desc" data-testid="empty-message">
        {{ resolvedMessage }}
      </p>

      <button
        v-if="props.onAction"
        type="button"
        class="state__action"
        data-testid="empty-action"
        @click="props.onAction"
        :aria-label="$t('common:states.empty.actionAria')"
      >
        {{ resolvedActionLabel }}
      </button>
    </div>
  </section>
</template>
