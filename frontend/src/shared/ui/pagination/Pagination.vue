<script setup lang="ts">
import { computed } from "vue";
import i18n from "@/shared/i18n";

type Props = {
  page: number;
  totalPages: number;
  disabled?: boolean;
};

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
});

const emit = defineEmits<{
  (e: "goToPage", page: number): void;
}>();

const safePage = computed(() => (Number.isFinite(props.page) && props.page > 0 ? props.page : 1));
const safeTotalPages = computed(() =>
  Number.isFinite(props.totalPages) && props.totalPages > 0 ? props.totalPages : 0
);

const canGoPrev = computed(() => !props.disabled && safeTotalPages.value > 0 && safePage.value > 1);
const canGoNext = computed(
  () => !props.disabled && safeTotalPages.value > 0 && safePage.value < safeTotalPages.value
);

const label = computed(() => i18n.t("common:pagination.ariaLabel"));
const prevLabel = computed(() => i18n.t("common:pagination.prev"));
const nextLabel = computed(() => i18n.t("common:pagination.next"));

function goTo(page: number): void {
  if (props.disabled) return;
  if (!Number.isFinite(page)) return;
  if (safeTotalPages.value <= 0) return;
  if (page < 1 || page > safeTotalPages.value) return;

  emit("goToPage", page);
}

function goPrev(): void {
  if (!canGoPrev.value) return;
  goTo(safePage.value - 1);
}

function goNext(): void {
  if (!canGoNext.value) return;
  goTo(safePage.value + 1);
}

const pageText = computed(() =>
  i18n.t("common:pagination.pageOf", { page: safePage.value, total: safeTotalPages.value })
);
</script>

<template>
  <nav
    class="mt-8 flex flex-wrap items-center justify-between gap-3"
    data-testid="pagination"
    :aria-label="label"
    role="navigation"
  >
    <button
      type="button"
      data-testid="pagination-prev"
      class="inline-flex items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
      :disabled="!canGoPrev"
      @click="goPrev"
      :aria-label="prevLabel"
    >
      <span aria-hidden="true">←</span>
      {{ prevLabel }}
    </button>

    <div class="text-sm text-slate-700" role="status" aria-live="polite">
      <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
        {{ pageText }}
      </span>
    </div>

    <button
      type="button"
      data-testid="pagination-next"
      class="inline-flex items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
      :disabled="!canGoNext"
      @click="goNext"
      :aria-label="nextLabel"
    >
      {{ nextLabel }}
      <span aria-hidden="true">→</span>
    </button>
  </nav>
</template>
