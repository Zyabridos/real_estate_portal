<script setup lang="ts">
withDefaults(
  defineProps<{
    ariaLabel?: string;
    compact?: boolean;
  }>(),
  {
    ariaLabel: "Loading",
    compact: true,
  }
);
</script>

<template>
  <div class="w-full" role="status" aria-live="polite" :aria-label="ariaLabel">
    <div class="flex items-center gap-2">
      <i class="pi pi-spinner pi-spin text-slate-700" aria-hidden="true" />
      <div class="relative w-full overflow-hidden rounded-full bg-slate-200" :class="compact ? 'h-1.5' : 'h-2.5'">
        <div class="loadingbar__bar absolute left-0 top-0 h-full w-1/3 rounded-full bg-slate-900" />
      </div>
    </div>
    <span class="sr-only">{{ ariaLabel }}</span>
  </div>
</template>

<style scoped>
.loadingbar__bar {
  animation: loadingbar 1.05s ease-in-out infinite;
}
@keyframes loadingbar {
  0%   { transform: translateX(-120%); opacity: .85; }
  50%  { transform: translateX(120%);  opacity: 1; }
  100% { transform: translateX(320%);  opacity: .85; }
}
</style>
