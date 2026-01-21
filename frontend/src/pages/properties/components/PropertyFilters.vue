<script setup lang="ts">
import { computed, ref, watch } from "vue";
import i18n from "@/shared/i18n";

import type { PropertyFiltersValue, PropertyStatus, PropertyType } from "@/shared/types/properties";

type Props = {
  initial?: PropertyFiltersValue;
  disabled?: boolean;
};

const props = withDefaults(defineProps<Props>(), {
  initial: () => ({}),
  disabled: false,
});

const emit = defineEmits<{
  (e: "apply", value: PropertyFiltersValue): void;
  (e: "reset"): void;
}>();

const city = ref<string>(props.initial.city ?? "");
const type = ref<PropertyType | "">(props.initial.type ?? "");
const status = ref<PropertyStatus | "">(props.initial.status ?? "");
const minPrice = ref<string>(props.initial.minPrice != null ? String(props.initial.minPrice) : "");
const maxPrice = ref<string>(props.initial.maxPrice != null ? String(props.initial.maxPrice) : "");

// keep in sync if parent changes initial
watch(
  () => props.initial,
  (next) => {
    city.value = next?.city ?? "";
    type.value = next?.type ?? "";
    status.value = next?.status ?? "";
    minPrice.value = next?.minPrice != null ? String(next.minPrice) : "";
    maxPrice.value = next?.maxPrice != null ? String(next.maxPrice) : "";
  },
  { deep: true }
);

const typeOptions = computed(() => [
  { value: "" as const, label: i18n.t("entities:property.anyType") },
  { value: "Apartment" as const, label: i18n.t("entities:property.type.apartment") },
  { value: "House" as const, label: i18n.t("entities:property.type.house") },
  { value: "Commercial" as const, label: i18n.t("entities:property.type.commercial") },
]);

const statusOptions = computed(() => [
  { value: "" as const, label: i18n.t("entities:property.anyStatus") },
  { value: "Active" as const, label: i18n.t("entities:property.status.active") },
  { value: "Sold" as const, label: i18n.t("entities:property.status.sold") },
]);

function normalizeNumber(input: string): number | undefined {
  const trimmed = input.trim();
  if (!trimmed) return undefined;

  const n = Number(trimmed);
  if (!Number.isFinite(n)) return undefined;
  if (n < 0) return undefined;

  return n;
}

function buildPayload(): PropertyFiltersValue {
  const cityValue = city.value.trim();
  const payload: PropertyFiltersValue = {};

  if (cityValue) payload.city = cityValue;
  if (type.value) payload.type = type.value as PropertyType;
  if (status.value) payload.status = status.value as PropertyStatus;

  const min = normalizeNumber(minPrice.value);
  const max = normalizeNumber(maxPrice.value);

  if (min != null) payload.minPrice = min;
  if (max != null) payload.maxPrice = max;

  return payload;
}

function onApply(): void {
  emit("apply", buildPayload());
}

function onReset(): void {
  city.value = "";
  type.value = "";
  status.value = "";
  minPrice.value = "";
  maxPrice.value = "";

  emit("reset");
}
</script>

<template>
  <section
    class="mt-6 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm"
    :aria-label="$t('entities:property.filtersAriaLabel')"
  >
    <div class="flex flex-col gap-4">
      <div class="grid grid-cols-1 gap-3 md:grid-cols-5">
        <!-- City -->
        <div class="md:col-span-2">
          <label class="mb-1 block text-xs font-medium text-slate-700" for="property-city">
            {{ $t("entities:property.cityLabel") }}
          </label>
          <input
            id="property-city"
            data-testid="filter-city"
            v-model="city"
            :disabled="disabled"
            type="text"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
            :placeholder="$t('entities:property.cityPlaceholder')"
            autocomplete="address-level2"
          />
        </div>

        <!-- Type -->
        <div>
          <label class="mb-1 block text-xs font-medium text-slate-700" for="property-type">
            {{ $t("entities:property.typeLabel") }}
          </label>
          <select
            id="property-type"
            data-testid="filter-type"
            v-model="type"
            :disabled="disabled"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
          >
            <option v-for="opt in typeOptions" :key="opt.value || 'any'" :value="opt.value">
              {{ opt.label }}
            </option>
          </select>
        </div>

        <!-- Status -->
        <div>
          <label class="mb-1 block text-xs font-medium text-slate-700" for="property-status">
            {{ $t("entities:property.statusLabel") }}
          </label>
          <select
            id="property-status"
            data-testid="filter-status"
            v-model="status"
            :disabled="disabled"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
          >
            <option v-for="opt in statusOptions" :key="opt.value || 'any'" :value="opt.value">
              {{ opt.label }}
            </option>
          </select>
        </div>

        <!-- Price range -->
        <div class="md:col-span-1">
          <label class="mb-1 block text-xs font-medium text-slate-700">
            {{ $t("entities:property.priceRangeLabel") }}
          </label>

          <div class="flex items-center gap-2">
            <input
              v-model="minPrice"
              data-testid="filter-minPrice"
              :disabled="disabled"
              inputmode="numeric"
              type="text"
              class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
              :placeholder="$t('entities:property.minPriceLabel')"
              :aria-label="$t('entities:property.minPriceLabel')"
            />
            <span class="text-xs text-slate-400">—</span>
            <input
              v-model="maxPrice"
              :disabled="disabled"
              data-testid="filter-maxPrice"
              inputmode="numeric"
              type="text"
              class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm text-slate-900 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:opacity-60"
              :placeholder="$t('entities:property.maxPriceLabel')"
              :aria-label="$t('entities:property.maxPriceLabel')"
            />
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex flex-wrap items-center justify-end gap-2">
        <button
          type="button"
          data-testid="filters-reset"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 disabled:opacity-60"
          :disabled="disabled"
          @click="onReset"
        >
          {{ $t("common:actions.reset") }}
        </button>

        <button
          type="button"
          data-testid="filters-apply"
          class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:opacity-60"
          :disabled="disabled"
          @click="onApply"
        >
          {{ $t("common:actions.apply") }}
        </button>
      </div>
    </div>
  </section>
</template>
