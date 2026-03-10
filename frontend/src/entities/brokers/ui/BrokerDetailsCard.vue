<script setup lang="ts">
import { computed } from "vue";

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

function formatDate(value?: string): string {
  if (!value) {
    return "—";
  }

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "—";
  }

  return new Intl.DateTimeFormat("ru-RU", {
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(date);
}

const createdAtFormatted = computed(() => formatDate(props.broker.createdAt));
const updatedAtFormatted = computed(() => formatDate(props.broker.updatedAt));

const fullName = computed(() => `${props.broker.firstName} ${props.broker.lastName}`.trim());
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
              Email
            </p>
            <p class="mt-1 break-words text-sm text-slate-900">
              {{ broker.email || "—" }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Телефон
            </p>
            <p class="mt-1 break-words text-sm text-slate-900">
              {{ broker.phoneNumber || "—" }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4 sm:col-span-2">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Стаж в агентстве
            </p>
            <p class="mt-1 text-sm text-slate-900">
              Работает в агентстве с {{ createdAtFormatted }}
            </p>
          </div>

          <div class="rounded-xl border border-slate-200 bg-slate-50 p-4 sm:col-span-2">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Последнее обновление
            </p>
            <p class="mt-1 text-sm text-slate-900">
              Профиль обновлён {{ updatedAtFormatted }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </article>
</template>
