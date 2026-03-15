<script setup lang="ts">
import { computed } from "vue";

import type { BrokerDetailsDto } from "@/features/brokers/api/dtos/broker-details.dto";
import {
  getBrokerFallbackImage,
  getBrokerGenderLabelKey,
  normalizeBrokerGender,
} from "@/entities/brokers/utils/brokerGender.ts";

type Props = {
  broker: BrokerDetailsDto;
};

const props = defineProps<Props>();

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
    class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm"
    :aria-label="fullName"
    data-testid="broker-details-card"
  >
    <div class="grid gap-6 p-6 md:grid-cols-[240px_minmax(0,1fr)] md:p-8">
      <div class="flex justify-center md:justify-start">
        <div
          class="flex h-64 w-full max-w-[240px] items-center justify-center rounded-2xl border border-slate-200 bg-black"
        >
          <img
            :src="pictureSrc"
            :alt="fullName"
            :class="[
              'h-full w-full rounded-2xl',
              useContainImage ? 'object-contain p-3' : 'object-cover'
            ]"
          />
        </div>
      </div>

      <div class="min-w-0">
        <h2 class="text-2xl font-semibold tracking-tight text-slate-900">
          {{ fullName }}
        </h2>

        <p class="mt-2 text-sm text-slate-600">
          {{ $t("brokers:details.subtitle") }}
        </p>

        <dl class="mt-6 grid gap-4 sm:grid-cols-2">
          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4">
            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">
              {{ $t("brokers:details.genderLabel") }}
            </dt>
            <dd class="mt-1 text-sm font-medium text-slate-900">
              {{ $t(genderLabelKey) }}
            </dd>
          </div>

          <div
            v-if="broker.email"
            class="rounded-xl border border-slate-200 bg-slate-50 p-4"
          >
            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">
              {{ $t("brokers:details.emailLabel") }}
            </dt>
            <dd class="mt-1 break-all text-sm text-slate-900">
              {{ broker.email }}
            </dd>
          </div>

          <div
            v-if="broker.phoneNumber"
            class="rounded-xl border border-slate-200 bg-slate-50 p-4"
          >
            <dt class="text-xs font-medium uppercase tracking-wide text-slate-500">
              {{ $t("brokers:details.phoneLabel") }}
            </dt>
            <dd class="mt-1 text-sm text-slate-900">
              {{ broker.phoneNumber }}
            </dd>
          </div>
        </dl>
      </div>
    </div>
  </article>
</template>
