<script setup lang="ts">
type Props = {
  id?: string;
  modelValue: string;
  disabled?: boolean;
  placeholder?: string;
  testid?: string;
  autocomplete?: string;
  inputmode?: string;
  ariaLabel?: string;
};

const props = withDefaults(defineProps<Props>(), { disabled: false });

const emit = defineEmits<{ (e: "update:modelValue", v: string): void }>();
</script>

<template>
  <input
    :id="id"
    :data-testid="testid"
    :disabled="disabled"
    :value="modelValue"
    type="text"
    class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
    :placeholder="placeholder"
    :autocomplete="autocomplete"
    :inputmode="inputmode"
    :aria-label="ariaLabel"
    @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
  />
</template>
