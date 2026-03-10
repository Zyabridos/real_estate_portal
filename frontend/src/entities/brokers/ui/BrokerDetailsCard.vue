<script setup lang="ts">
import { computed } from "vue";
import i18n from "@/shared/i18n";

import type { BrokerDetailsDto } from "@/features/brokers/api/dtos/broker-details.dto";

import femalePicture from "@/assets/images/defaultPictureFemale.png";
import malePicture from "@/assets/images/defaultPictureMale.png";
import neutralPicture from "@/assets/images/defaultPictureNeutral.png";

type Props = {
  broker: BrokerDetailsDto;
};

const props = defineProps<Props>();

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

const dateLocale = computed(() => {
  const language = i18n.resolvedLanguage ?? i18n.language;

  if (language === "ru") return "ru-RU";
  if (language === "no") return "nb-NO";

  return "en-GB";
});

function formatDate(value?: string): string {
  if (!value) {
    return i18n.t("brokers:card.details.emptyValue");
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return i18n.t("brokers:card.details.emptyValue");
  }

  return new Intl.DateTimeFormat(dateLocale.value, {
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(date);
}

const createdAtFormatted = computed(() => formatDate(props.broker.createdAt));
const updatedAtFormatted = computed(() => formatDate(props.broker.updatedAt));

const fullName = computed(() => `${props.broker.firstName} ${props.broker.lastName}`.trim());

const emailText = computed(() => props.broker.email || i18n.t("brokers:card.details.emptyValue"));
const phoneText = computed(() => props.broker.phoneNumber || i18n.t("brokers:card.details.emptyValue"));
</script>

<template>
  <article
    class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm"
    :aria-label="fullName"
    :data-testid="`broker-details-${broker.id}`"
  >
    <div class="flex flex-col gap-6 p-6 md:flex-row md:items-start">
      <div class="flex justify-center md:justify-start">
        <img
          :src="pictureSrc"
          :alt="fullName"
          class="h-32 w-32 rounded-2xl border border-slate-200 bg-slate-100 object-cover sm:h-40 sm:w-40"
        />
      </div>

      <div class="min-w-0 flex-1">
        <h2 class="text-2xl font-semibold tracking-tight text-slate-900">
          {{ broker.firstName }} {{ broker.lastName }}
        </h2>

        <div class="mt-4 grid gap-4 sm:grid-cols-2">
          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("brokers:card.emailLabel") }}
            </p>
            <p class="mt-1 break-words text-sm text-slate-900">
              {{ emailText }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("brokers:card.phoneLabel") }}
            </p>
            <p class="mt-1 break-words text-sm text-slate-900">
              {{ phoneText }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4 sm:col-span-2">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("brokers:card.details.experienceTitle") }}
            </p>
            <p class="mt-1 text-sm text-slate-900">
              {{ $t("brokers:card.details.experienceSince", { date: createdAtFormatted }) }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4 sm:col-span-2">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("brokers:card.details.lastUpdatedTitle") }}
            </p>
            <p class="mt-1 text-sm text-slate-900">
              {{ $t("brokers:card.details.profileUpdated", { date: updatedAtFormatted }) }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </article>
</template>
