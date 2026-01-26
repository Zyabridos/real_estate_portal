<script setup lang="ts">
defineProps<{ label: string; forId?: string }>();
// TODO: move to separate (i. e. reusable) component?
</script>

<template>
  <div>
    <label class="mb-1 block text-xs font-medium text-slate-700" :for="forId">
      {{ label }}
    </label>
    <slot />
  </div>
</template>
