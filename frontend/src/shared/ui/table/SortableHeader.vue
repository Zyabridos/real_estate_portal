<script setup lang="ts">
import { computed } from "vue";
import type { SortDirection } from "@/shared/types/queries";

type Props = {
  label: string;
  sortKey: string;
  activeSortBy: string;
  activeSortDirection: SortDirection;
  onSort: (sortKey: string) => void;
  testId?: string;
};

const props = defineProps<Props>();

const isActive = computed(() => props.activeSortBy === props.sortKey);

const ariaSort = computed<"none" | "ascending" | "descending">(() => {
  if (!isActive.value) return "none";
  return props.activeSortDirection === "asc" ? "ascending" : "descending";
});

const srText = computed(() => {
  if (ariaSort.value === "none") return "Not sorted";
  return `Sorted ${ariaSort.value}`;
});
</script>

<template>
  <th
    scope="col"
    class="whitespace-nowrap px-3 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-600"
    :aria-sort="ariaSort"
    :data-testid="testId ?? `th-${sortKey}`"
  >
    <button
      type="button"
      class="inline-flex items-center gap-2 rounded-md px-1 py-1 text-left hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-300"
      :data-testid="`sort-${sortKey}`"
      @click="onSort(sortKey)"
    >
      <span>{{ label }}</span>

      <span aria-hidden="true" class="inline-flex items-center text-slate-500">
        <!-- not active -->
        <i v-if="!isActive" class="pi pi-arrows-v text-base"></i>

        <!-- active asc -->
        <i v-else-if="activeSortDirection === 'asc'" class="pi pi-arrow-up text-base"></i>

        <!-- active desc -->
        <i v-else class="pi pi-arrow-down text-base"></i>
      </span>

      <span class="sr-only">{{ srText }}</span>
    </button>
  </th>
</template>
