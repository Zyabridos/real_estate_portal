<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import routes from "@/shared/routes";
import type { AgencyListItemDto } from "@/features/agencies/api/dtos/agency-list-item.dto";

const props = defineProps<{ agency: AgencyListItemDto }>();

const detailsTo = computed(() => routes.app.agencies.details(props.agency.id));

function formatAddress(a: AgencyListItemDto): string {
  const parts = [a.street, [a.zipCode, a.city].filter(Boolean).join(" ")].filter(Boolean);
  return parts.join(", ");
}

const address = computed(() => formatAddress(props.agency));
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:shadow-md"
    role="article"
    :aria-label="$t('pages:agencies.list.cardAriaLabel', { id: props.agency.id })"
    :data-testid="`agency-card-${props.agency.id}`"
  >
    <div class="flex items-start justify-between gap-4">
      <div class="min-w-0">
        <RouterLink
          class="block truncate text-base font-semibold text-slate-900 hover:text-indigo-800"
          :to="detailsTo"
          :aria-label="$t('pages:agencies.card.openDetailsAria', { name: props.agency.name })"
        >
          {{ props.agency.name }}
        </RouterLink>

        <p class="mt-1 text-sm text-slate-600">
          {{ $t("pages:agencies.card.orgNumberLabel") }}:
          <span class="font-medium text-slate-800">{{ props.agency.orgNumber }}</span>
        </p>

        <p v-if="address" class="mt-2 text-sm text-slate-600">
          {{ $t("pages:agencies.card.addressLabel") }}:
          <span class="text-slate-800">{{ address }}</span>
        </p>

        <p v-if="props.agency.phoneNumber" class="mt-2 text-sm text-slate-600">
          {{ $t("pages:agencies.card.phoneLabel") }}:
          <a
            class="font-medium text-slate-900 hover:text-indigo-800"
            :href="`tel:${props.agency.phoneNumber}`"
          >
            {{ props.agency.phoneNumber }}
          </a>
        </p>
      </div>

      <RouterLink
        class="shrink-0 rounded-xl border border-slate-200 bg-white px-3 py-2 text-xs font-medium text-slate-900 hover:bg-slate-50"
        :to="detailsTo"
        :aria-label="$t('pages:agencies.card.openDetailsAria', { name: props.agency.name })"
      >
        {{ $t("common:actions.view") }}
      </RouterLink>
    </div>
  </article>
</template>
