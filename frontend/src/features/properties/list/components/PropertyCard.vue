<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import i18n from "@/shared/i18n";
import type { PropertyListItemDto } from "@/features/properties/api/dtos/property-list-item.dto";
import house_default from "@/assets/images/house_default.png";

type Props = {
  property: PropertyListItemDto;
};

const props = defineProps<Props>();

const detailsTo = computed(() => `/properties/${props.property.id}`);
const priceText = computed(() => props.property.price.toLocaleString("nb-NO"));

const imageSrc = computed(() => {
  const propertyWithImage = props.property as PropertyListItemDto & {
    thumbnailUrl?: string | null;
    imageUrl?: string | null;
  };

  return propertyWithImage.thumbnailUrl?.trim() || propertyWithImage.imageUrl?.trim() || house_default;
});

const localizedStatus = computed(() => {
  if (props.property.status === "Active") return i18n.t("properties:card.status.active");
  if (props.property.status === "Sold") return i18n.t("properties:card.status.sold");
  return props.property.status;
});

const localizedType = computed(() => {
  if (props.property.type === "Apartment") return i18n.t("properties:card.type.apartment");
  if (props.property.type === "House") return i18n.t("properties:card.type.house");
  if (props.property.type === "Commercial") return i18n.t("properties:card.type.commercial");
  return props.property.type;
});
</script>

<template>
  <article
    data-testid="property-card"
    class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    role="listitem"
  >
    <div class="flex h-full flex-col md:flex-row" :data-testid="`property-card-${property.id}`">
      <div class="h-56 w-full shrink-0 bg-slate-100 md:h-auto md:w-80 lg:w-96">
        <img
          :src="imageSrc"
          :alt="property.title"
          class="h-full w-full object-cover"
          loading="lazy"
        />
      </div>

      <div class="flex min-w-0 flex-1 flex-col justify-between p-5 md:p-6">
        <div class="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div class="min-w-0">
            <h2 class="text-lg font-semibold text-slate-900">
              {{ property.title }}
            </h2>

            <p class="mt-2 text-sm text-slate-600" data-testid="property-card-meta">
              {{ property.city }} • {{ localizedType }} • {{ localizedStatus }}
            </p>
          </div>

          <div class="shrink-0 text-left md:text-right">
            <div
              class="text-xl font-semibold text-slate-900"
              :aria-label="$t('properties:filters.priceValueAriaLabel', { value: priceText })"
            >
              {{ priceText }}
            </div>

            <div class="text-sm text-slate-500">
              {{ $t("common:app.currency.nok") }}
            </div>
          </div>
        </div>

        <div
          class="mt-6 flex flex-col gap-3 border-t border-slate-100 pt-4 sm:flex-row sm:items-center sm:justify-between"
        >
          <div class="text-xs text-slate-500">
            {{ $t("common:pagination.idShort") }}:
            <span class="font-mono">{{ property.id }}</span>
          </div>

          <RouterLink
            :to="detailsTo"
            class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            :aria-label="$t('properties:list.viewDetailsAriaLabel', { id: property.id })"
          >
            {{ $t("common:actions.viewDetails") }}
          </RouterLink>
        </div>
      </div>
    </div>
  </article>
</template>
