<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import type { BrokerListItemDto } from "@/features/brokers/api/dtos/broker-list-item.dto";

import routes from "@/shared/routes";
import {
  getBrokerFallbackImage,
  getBrokerGenderLabelKey,
  normalizeBrokerGender,
} from "@/entities/brokers/utils/brokerGender.ts";

type Props = {
  broker: BrokerListItemDto;
};

const props = defineProps<Props>();

const detailsTo = computed(() => routes.app.brokers.details(props.broker.id));

const fullName = computed(() => `${props.broker.firstName} ${props.broker.lastName}`.trim());

const hasCustomPhoto = computed(() => Boolean(props.broker.photoUrl?.trim()));

const normalizedGender = computed(() => normalizeBrokerGender(props.broker.gender));

const pictureSrc = computed(() => {
  const customPhoto = props.broker.photoUrl?.trim();

  if (customPhoto) {
    return customPhoto;
  }

  return getBrokerFallbackImage(props.broker.gender);
});

const genderLabelKey = computed(() => getBrokerGenderLabelKey(props.broker.gender));

const useContainImage = computed(() => {
  return !hasCustomPhoto.value
    && (normalizedGender.value === "other" || normalizedGender.value === "unspecified");
});
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    role="article"
    :aria-label="fullName"
    :data-testid="`broker-card-${broker.id}`"
  >
    <div class="flex items-center gap-4 p-4 sm:gap-5 sm:p-5">
      <div
        class="flex h-20 w-20 shrink-0 items-center justify-center rounded-xl border border-slate-200 bg-black sm:h-24 sm:w-24"
      >
        <img
          :src="pictureSrc"
          :alt="fullName"
          :class="[
            'h-full w-full rounded-xl',
            useContainImage ? 'object-contain p-1.5' : 'object-cover'
          ]"
        />
      </div>

      <div class="min-w-0 flex-1">
        <h2 class="text-base font-semibold text-slate-900 sm:text-lg">
          {{ fullName }}
        </h2>

        <div class="mt-2 space-y-1 text-sm text-slate-600">
          <p class="truncate">
            <span class="font-medium text-slate-700">{{ $t("brokers:card.genderLabel") }}:</span>
            {{ $t(genderLabelKey) }}
          </p>

          <p v-if="broker.email" class="truncate">
            <span class="font-medium text-slate-700">{{ $t("brokers:card.emailLabel") }}:</span>
            {{ broker.email }}
          </p>

          <p v-if="broker.phoneNumber" class="truncate">
            <span class="font-medium text-slate-700">{{ $t("brokers:card.phoneLabel") }}:</span>
            {{ broker.phoneNumber }}
          </p>
        </div>
      </div>

      <div class="shrink-0">
        <RouterLink
          :to="detailsTo"
          class="inline-flex rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-slate-800"
          :aria-label="$t('brokers:card.openDetailsAria', { name: fullName })"
        >
          {{ $t("common:actions.viewDetails") }}
        </RouterLink>
      </div>
    </div>
  </article>
</template>
