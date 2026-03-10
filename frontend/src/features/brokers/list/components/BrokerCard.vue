<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import type { BrokerDetailsDto } from "@/features/brokers/api/dtos/broker-details.dto";

import routes from "@/shared/routes.ts";

import femalePicture from "@/assets/images/defaultPictureFemale.png";
import malePicture from "@/assets/images/defaultPictureMale.png";
import neutralPicture from "@/assets/images/defaultPictureNeutral.png";

type Props = {
  broker: BrokerDetailsDto;
};

const props = defineProps<Props>();

const detailsTo = computed(() => routes.app.brokers.details(props.broker.id));

const fullName = computed(() => `${props.broker.firstName} ${props.broker.lastName}`.trim());

const fallbackPicture = computed(() => {
  if (props.broker.gender === "female") {
    return femalePicture;
  }

  if (props.broker.gender === "male") {
    return malePicture;
  }

  return neutralPicture;
});

const pictureSrc = computed(() => props.broker.photoUrl || fallbackPicture.value);
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
    role="article"
    :aria-label="fullName"
    :data-testid="`broker-card-${broker.id}`"
  >
    <div class="flex items-center gap-4 p-4 sm:gap-5 sm:p-5">
      <img
        :src="pictureSrc"
        :alt="fullName"
        class="h-20 w-20 shrink-0 rounded-xl border border-slate-200 bg-slate-100 object-cover sm:h-24 sm:w-24"
      />

      <div class="min-w-0 flex-1">
        <h2 class="text-base font-semibold text-slate-900 sm:text-lg">
          {{ fullName }}
        </h2>

        <div class="mt-2 space-y-1 text-sm text-slate-600">
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
