<script setup lang="ts">
type Option = { value: string; label: string };

type Props = {
  id?: string;
  modelValue: string;
  disabled?: boolean;
  options: Option[];
  testid?: string;
};

const props = withDefaults(defineProps<Props>(), { disabled: false });

const emit = defineEmits<{ (e: "update:modelValue", v: string): void }>();
</script>

<template>
  <select
    :id="id"
    :data-testid="testid"
    :disabled="disabled"
    :value="modelValue"
    class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
    @change="emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
  >
    <option v-for="opt in options" :key="opt.value || 'any'" :value="opt.value">
      {{ opt.label }}
    </option>
  </select>
</template>
