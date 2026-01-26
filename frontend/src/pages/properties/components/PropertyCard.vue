<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";

type Props = {
  property: PropertyListItemDto;
};

const props = defineProps<Props>();

const detailsTo = computed(() => `/properties/${props.property.id}`);
const priceText = computed(() => props.property.price.toLocaleString());
</script>

<template>
  <article
    data-testid="property-card"
    class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    role="listitem"
  >
    <div class="p-5" :data-testid="`property-card-${property.id}`">
      <div class="flex items-start justify-between gap-3">
        <div>
          <h2 class="text-base font-semibold text-slate-900">
            {{ property.title }}
          </h2>

          <p class="mt-1 text-sm text-slate-600" data-testid="property-card-meta">
            {{ property.city }} • {{ property.type }} • {{ property.status }}
          </p>
        </div>

        <div class="text-right">
          <div
            class="text-sm font-semibold text-slate-900"
            :aria-label="$t('entities:property.priceValueAriaLabel', { value: priceText })"
          >
            {{ priceText }}
          </div>
          <div class="text-xs text-slate-500">
            {{ $t("common:currency.nok") }}
          </div>
        </div>
      </div>

      <div class="mt-4 flex items-center justify-between">
        <RouterLink
          :to="detailsTo"
          class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
          :aria-label="$t('pages:properties.list.viewDetailsAriaLabel', { id: property.id })"
        >
          {{ $t("common:actions.viewDetails") }}
        </RouterLink>

        <div class="text-xs text-slate-500">
          {{ $t("common:pagination.idShort") }}:
          <span class="font-mono">{{ property.id }}</span>
        </div>
      </div>
    </div>
  </article>
</template>
