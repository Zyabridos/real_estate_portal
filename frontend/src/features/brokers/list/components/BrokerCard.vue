<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import type { BrokerListItemDto } from "@/features/brokers/api/dtos/broker-list-item.dto";

type Props = {
  broker: BrokerListItemDto;
};

const props = defineProps<Props>();

const detailsTo = computed(() => `/brokers/${props.broker.id}`);
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    role="listitem"
    :aria-label="$t('pages:brokers.list.cardAriaLabel', { id: broker.id })"
    :data-testid="`broker-card-${broker.id}`"
  >
    <div class="p-5">
      <div class="flex items-start justify-between gap-3">
        <div>
          <h2 class="text-base font-semibold text-slate-900">
            {{ broker.firstName }} {{ broker.lastName }}
          </h2>

          <p class="mt-1 text-sm text-slate-600">
            {{ broker.email }} {{ broker.phoneNumber }} {{ broker.createdAt }}
          </p>
        </div>
      </div>

      <div class="mt-4 flex items-center justify-between">
        <RouterLink
          :to="detailsTo"
          class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
          :aria-label="$t('pages:brokers.list.viewDetailsAriaLabel', { id: broker.id })"
        >
          {{ $t("common:actions.viewDetails") }}
        </RouterLink>

        <div class="text-xs text-slate-500">
          {{ $t("common:pagination.idShort") }}:
          <span class="font-mono">{{ broker.id }}</span>
        </div>
      </div>
    </div>
  </article>
</template>
