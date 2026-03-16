<script setup lang="ts">
import { computed, ref, watch } from "vue";

import Pagination from "@/shared/ui/pagination/Pagination.vue";
import PaginationMeta from "@/shared/ui/pagination/PaginationMeta.vue";

const props = withDefaults(
  defineProps<{
    page: number;
    pageSize: number;
    totalItems?: number | null;
    totalPages?: number | null;
    topClass?: string;
    bottomClass?: string;
    showSkeleton?: boolean;
  }>(),
  {
    topClass: "mt-4",
    bottomClass: "mt-8",
    showSkeleton: true,
  }
);

const emit = defineEmits<{
  (e: "goToPage", nextPage: number): void;
}>();

const stableTotals = ref<{ totalItems: number; totalPages: number } | null>(null);

watch(
  () => [props.totalItems, props.totalPages] as const,
  ([totalItems, totalPages]) => {
    if (
      typeof totalItems === "number" &&
      Number.isFinite(totalItems) &&
      totalItems >= 0 &&
      typeof totalPages === "number" &&
      Number.isFinite(totalPages) &&
      totalPages >= 0
    ) {
      stableTotals.value = {
        totalItems,
        totalPages,
      };
    }
  },
  { immediate: true }
);

const hasResolvedTotals = computed(() => stableTotals.value !== null);

const effectiveTotalItems = computed(() => stableTotals.value?.totalItems ?? 0);
const effectiveTotalPages = computed(() => stableTotals.value?.totalPages ?? 0);

const showPagination = computed(
  () => hasResolvedTotals.value && effectiveTotalPages.value > 1
);

function onGoToPage(nextPage: number): void {
  emit("goToPage", nextPage);
}
</script>

<template>
  <div class="w-full">
    <div :class="topClass">
      <PaginationMeta
        v-if="hasResolvedTotals"
        :page="page"
        :pageSize="pageSize"
        :totalItems="effectiveTotalItems"
      />

      <div
        v-else-if="showSkeleton"
        class="h-5 w-48 animate-pulse rounded bg-slate-100"
        aria-hidden="true"
      />
    </div>

    <slot />

    <Pagination
      v-if="showPagination"
      :class="bottomClass"
      :page="page"
      :totalPages="effectiveTotalPages"
      @goToPage="onGoToPage"
    />
  </div>
</template>
