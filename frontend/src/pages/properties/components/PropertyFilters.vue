<script setup lang="ts">
import { computed, ref, watch } from "vue";
import i18n from "@/shared/i18n";

import FilterField from "@/pages/properties/components/filters/FilterField.vue";
import FilterInput from "@/pages/properties/components/filters/FilterInput.vue";
import FilterSelect from "@/pages/properties/components/filters/FilterSelect.vue";

import type { PropertyFiltersValue, PropertyStatus, PropertyType } from "@/shared/types/properties";

type Props = {
  modelValue: PropertyFiltersValue;
  disabled?: boolean;
};

const props = withDefaults(defineProps<Props>(), { disabled: false });

const emit = defineEmits<{
  (e: "update:modelValue", value: PropertyFiltersValue): void;
  (e: "reset"): void;
}>();

// локальный ввод (не дергаем parent на каждый символ напрямую — parent сам дебаунсит query)
const city = ref("");
const type = ref<PropertyType | "">("");
const status = ref<PropertyStatus | "">("");
const minPrice = ref("");
const maxPrice = ref("");

watch(
  () => props.modelValue,
  (next) => {
    city.value = next.city ?? "";
    type.value = next.type ?? "";
    status.value = next.status ?? "";
    minPrice.value = next.minPrice != null ? String(next.minPrice) : "";
    maxPrice.value = next.maxPrice != null ? String(next.maxPrice) : "";
  },
  { immediate: true, deep: true }
);

const typeOptions = computed(() => [
  { value: "", label: i18n.t("entities:property.anyType") },
  { value: "Apartment", label: i18n.t("entities:property.type.apartment") },
  { value: "House", label: i18n.t("entities:property.type.house") },
  { value: "Commercial", label: i18n.t("entities:property.type.commercial") },
]);

const statusOptions = computed(() => [
  { value: "", label: i18n.t("entities:property.anyStatus") },
  { value: "Active", label: i18n.t("entities:property.status.active") },
  { value: "Sold", label: i18n.t("entities:property.status.sold") },
]);

function parseNonNegativeNumber(input: string): number | undefined {
  const s = input.trim();
  if (!s) return undefined;
  const n = Number(s);
  if (!Number.isFinite(n) || n < 0) return undefined;
  return n;
}

function buildValue(): PropertyFiltersValue {
  const out: PropertyFiltersValue = {};

  const cityValue = city.value.trim();
  if (cityValue) out.city = cityValue;
  if (type.value) out.type = type.value as PropertyType;
  if (status.value) out.status = status.value as PropertyStatus;

  const min = parseNonNegativeNumber(minPrice.value);
  const max = parseNonNegativeNumber(maxPrice.value);

  if (min != null) out.minPrice = min;
  if (max != null) out.maxPrice = max;

  return out;
}

function emitNow(): void {
  emit("update:modelValue", buildValue());
}

function onReset(): void {
  city.value = "";
  type.value = "";
  status.value = "";
  minPrice.value = "";
  maxPrice.value = "";

  emit("update:modelValue", {});
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
        <div class="md:col-span-2">
          <FilterField :label="$t('entities:property.cityLabel')" for-id="property-city">
            <FilterInput
              id="property-city"
              testid="filter-city"
              v-model="city"
              :disabled="disabled"
              :placeholder="$t('entities:property.cityPlaceholder')"
              autocomplete="address-level2"
              @update:modelValue="emitNow"
            />
          </FilterField>
        </div>

        <FilterField :label="$t('entities:property.typeLabel')" for-id="property-type">
          <FilterSelect
            id="property-type"
            testid="filter-type"
            v-model="type"
            :disabled="disabled"
            :options="typeOptions"
            @update:modelValue="emitNow"
          />
        </FilterField>

        <FilterField :label="$t('entities:property.statusLabel')" for-id="property-status">
          <FilterSelect
            id="property-status"
            testid="filter-status"
            v-model="status"
            :disabled="disabled"
            :options="statusOptions"
            @update:modelValue="emitNow"
          />
        </FilterField>

        <div class="md:col-span-1">
          <FilterField :label="$t('entities:property.priceRangeLabel')">
            <div class="flex items-center gap-2">
              <FilterInput
                testid="filter-minPrice"
                v-model="minPrice"
                :disabled="disabled"
                inputmode="numeric"
                :placeholder="$t('entities:property.minPriceLabel')"
                :aria-label="$t('entities:property.minPriceLabel')"
                @update:modelValue="emitNow"
              />
              <span class="text-xs text-slate-400">—</span>
              <FilterInput
                testid="filter-maxPrice"
                v-model="maxPrice"
                :disabled="disabled"
                inputmode="numeric"
                :placeholder="$t('entities:property.maxPriceLabel')"
                :aria-label="$t('entities:property.maxPriceLabel')"
                @update:modelValue="emitNow"
              />
            </div>
          </FilterField>
        </div>
      </div>

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
      </div>
    </div>
  </section>
</template>
