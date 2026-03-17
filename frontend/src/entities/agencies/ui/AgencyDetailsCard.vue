<script setup lang="ts">
import { computed } from "vue";

import { formatAddress } from "@/shared/utils/formatters/formatAddress";
import type { AgencyDetailsDto } from "@/features/agencies/api/dtos/agency-details.dto";

const props = defineProps<{ agency: AgencyDetailsDto }>();

const address = computed(() => formatAddress(props.agency));
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm"
    data-testid="agency-details-card"
  >
    <div class="flex flex-col gap-4">
      <div>
        <h2 class="text-lg font-semibold text-slate-900">
          {{ props.agency.name }}
        </h2>

        <p class="mt-1 text-sm text-slate-600">
          {{ $t("agencies:card.orgNumberLabel") }}:
          <span class="font-medium text-slate-800">{{ props.agency.orgNumber }}</span>
        </p>
      </div>

      <div class="grid grid-cols-1 gap-3 md:grid-cols-2">
        <div v-if="address" class="text-sm text-slate-700">
          <span class="text-slate-500">{{ $t("agencies:card.addressLabel") }}:</span>
          <span class="ml-1">{{ address }}</span>
        </div>

        <div v-if="props.agency.phoneNumber" class="text-sm text-slate-700">
          <span class="text-slate-500">{{ $t("agencies:card.phoneLabel") }}:</span>
          <a
            class="ml-1 font-medium text-slate-900 hover:text-indigo-800"
            :href="`tel:${props.agency.phoneNumber}`"
          >
            {{ props.agency.phoneNumber }}
          </a>
        </div>
      </div>
    </div>
  </article>
</template>
