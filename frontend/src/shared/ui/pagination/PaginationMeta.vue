<script setup lang="ts">
type Props = {
  page: number;
  pageSize: number;
  totalItems: number;
};

defineProps<Props>();
</script>

<template>
  <div
    class="flex flex-wrap items-center gap-2 text-xs text-slate-600"
    role="status"
    aria-live="polite"
  >
    <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
      {{ $t("common:pagination.page") }}:
      <span class="font-medium text-slate-900">{{ page }}</span>
    </span>

    <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
      {{ $t("common:pagination.pageSize") }}:
      <span class="font-medium text-slate-900">{{ pageSize }}</span>
    </span>

    <span class="rounded-full border border-slate-200 bg-white px-3 py-1">
      {{ $t("common:pagination.total") }}:
      <span class="font-medium text-slate-900">{{ totalItems }}</span>
    </span>
  </div>
</template>
