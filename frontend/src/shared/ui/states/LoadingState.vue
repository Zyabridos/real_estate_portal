<script setup lang="ts">
type Props = {
  title: string;
  subtitle?: string | null;
  testId?: string;
};

withDefaults(defineProps<Props>(), {
  subtitle: null,
  testId: "details-loading-state",
});
</script>

<template>
  <section
    :data-testid="testId"
    class="grid min-h-[34rem] place-items-center rounded-3xl border border-slate-200 bg-white/90 shadow-sm backdrop-blur"
    aria-live="polite"
    aria-busy="true"
  >
    <div class="flex flex-col items-center gap-4 text-center">
      <div class="relative h-16 w-16">
        <div class="absolute inset-0 rounded-full border-4 border-slate-200" />
        <div
          class="absolute inset-0 animate-spin rounded-full border-4 border-transparent border-t-emerald-500 border-r-emerald-300"
        />
        <div class="absolute inset-2 rounded-full bg-white" />
      </div>

      <div class="space-y-1">
        <p class="text-base font-semibold text-slate-900">
          {{ title }}
        </p>

        <p v-if="subtitle" class="text-sm text-slate-500">
          {{ subtitle }}
        </p>
      </div>
    </div>
  </section>
</template>
