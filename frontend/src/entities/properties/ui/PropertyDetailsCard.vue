<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import i18n from "@/shared/i18n";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";
import routes from "@/shared/routes";
import house_default from "@/assets/images/house_default.png";

type Props = {
  property: PropertyDetailsDto;
};

const { property } = defineProps<Props>();

const formattedPrice = computed(() => property.price.toLocaleString("nb-NO"));

const hasDescription = computed(() => !!property.description?.trim());
const titleText = computed(() => property.title?.trim() ?? "");

const imageSrc = computed(() => {
  return property.mainImageUrl?.trim() || house_default;
});

const createdAtText = computed(() => {
  if (!property.createdAt) return null;

  const date = new Date(property.createdAt);
  if (Number.isNaN(date.getTime())) return null;

  return new Intl.DateTimeFormat("nb-NO", {
    year: "numeric",
    month: "long",
    day: "numeric",
  }).format(date);
});

const brokerTo = computed(() => {
  return property.brokerId
    ? routes.app.brokers.details(property.brokerId)
    : routes.app.brokers.list();
});

const statusClass = computed(() => {
  if (property.status === "Active") {
    return "bg-emerald-50 text-emerald-700 ring-emerald-200";
  }

  if (property.status === "Sold") {
    return "bg-slate-100 text-slate-700 ring-slate-200";
  }

  return "bg-slate-100 text-slate-700 ring-slate-200";
});

const typeClass = computed(() => {
  if (property.type === "Apartment") {
    return "bg-blue-50 text-blue-700 ring-blue-200";
  }

  if (property.type === "House") {
    return "bg-amber-50 text-amber-700 ring-amber-200";
  }

  if (property.type === "Commercial") {
    return "bg-violet-50 text-violet-700 ring-violet-200";
  }

  return "bg-slate-100 text-slate-700 ring-slate-200";
});

const localizedStatus = computed(() => {
  if (property.status === "Active") return i18n.t("entities:property.status.active");
  if (property.status === "Sold") return i18n.t("entities:property.status.sold");
  return property.status;
});

const localizedType = computed(() => {
  if (property.type === "Apartment") return i18n.t("entities:property.type.apartment");
  if (property.type === "House") return i18n.t("entities:property.type.house");
  if (property.type === "Commercial") return i18n.t("entities:property.type.commercial");
  return property.type;
});
</script>

<template>
  <article
    class="overflow-hidden rounded-3xl border border-slate-200 bg-white shadow-sm"
    data-testid="property-details-card"
    :aria-label="$t('entities:property.detailsCard.detailsCardAriaLabel')"
  >
    <div class="grid grid-cols-1 lg:grid-cols-[1.2fr_0.9fr]">
      <div class="relative min-h-[280px] bg-slate-100 lg:min-h-[520px]">
        <img
          :src="imageSrc"
          :alt="titleText"
          class="h-full w-full object-cover"
        />

        <div class="absolute left-4 top-4 flex flex-wrap gap-2">
          <span
            class="inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold ring-1 ring-inset"
            :class="statusClass"
            data-testid="property-status-badge"
          >
            {{ localizedStatus }}
          </span>

          <span
            class="inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold ring-1 ring-inset"
            :class="typeClass"
            data-testid="property-type-badge"
          >
            {{ localizedType }}
          </span>
        </div>
      </div>

      <div class="p-6 md:p-8">
        <div class="flex flex-col gap-6">
          <header class="space-y-3">
            <div class="flex items-start justify-between gap-4">
              <div class="min-w-0">
                <p class="text-sm font-medium text-slate-500">
                  {{ property.city }}
                </p>

                <h1
                  class="text-2xl font-semibold tracking-tight text-slate-900 md:text-3xl"
                  data-testid="property-title"
                >
                  {{ titleText }}
                </h1>
              </div>

              <div class="shrink-0 text-right">
                <div
                  class="text-2xl font-bold tracking-tight text-slate-900 md:text-3xl"
                  data-testid="property-price"
                  :aria-label="$t('entities:property.detailsCard.priceValueAriaLabel', { value: formattedPrice })"
                >
                  {{ formattedPrice }}
                </div>

                <div class="text-sm text-slate-500">
                  {{ $t("common:currency.nok") }}
                </div>
              </div>
            </div>

            <p class="text-sm leading-6 text-slate-600" data-testid="property-address">
              {{ property.address }}
            </p>
          </header>

          <section :aria-label="$t('entities:property.detailsCard.metaSectionAriaLabel')">
            <div class="grid grid-cols-2 gap-3 sm:grid-cols-4">
              <div class="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                <p class="text-xs font-medium uppercase tracking-wide text-slate-500">
                  {{ $t("entities:property.detailsCard.bedroomsLabel") }}
                </p>
                <p class="mt-2 text-lg font-semibold text-slate-900" data-testid="property-bedrooms">
                  {{ property.bedrooms }}
                </p>
              </div>

              <div class="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                <p class="text-xs font-medium uppercase tracking-wide text-slate-500">
                  {{ $t("entities:property.detailsCard.bathroomsLabel") }}
                </p>
                <p class="mt-2 text-lg font-semibold text-slate-900" data-testid="property-bathrooms">
                  {{ property.bathrooms }}
                </p>
              </div>

              <div class="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                <p class="text-xs font-medium uppercase tracking-wide text-slate-500">
                  {{ $t("entities:property.detailsCard.areaLabel") }}
                </p>
                <p class="mt-2 text-lg font-semibold text-slate-900" data-testid="property-area">
                  {{ property.area }} m²
                </p>
              </div>

              <div class="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                <p class="text-xs font-medium uppercase tracking-wide text-slate-500">
                  {{ $t("entities:property.detailsCard.cityLabel") }}
                </p>
                <p class="mt-2 text-lg font-semibold text-slate-900" data-testid="property-city">
                  {{ property.city }}
                </p>
              </div>
            </div>
          </section>

          <section
            v-if="hasDescription"
            data-testid="property-description"
            :aria-label="$t('entities:property.detailsCard.descriptionSectionAriaLabel')"
          >
            <h2 class="mb-2 text-sm font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("entities:property.detailsCard.descriptionTitle") }}
            </h2>

            <p class="whitespace-pre-line text-sm leading-7 text-slate-700">
              {{ property.description }}
            </p>
          </section>

          <section class="rounded-2xl border border-slate-200 p-4">
            <h2 class="text-sm font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("entities:property.detailsCard.metaTitle") }}
            </h2>

            <dl class="mt-4 space-y-3 text-sm">
              <div class="flex items-start justify-between gap-4">
                <dt class="text-slate-500">{{ $t("entities:property.detailsCard.typeLabel") }}</dt>
                <dd class="text-right font-medium text-slate-900">
                  {{ localizedType }}
                </dd>
              </div>

              <div class="flex items-start justify-between gap-4">
                <dt class="text-slate-500">{{ $t("entities:property.detailsCard.statusLabel") }}</dt>
                <dd class="text-right font-medium text-slate-900">
                  {{ localizedStatus }}
                </dd>
              </div>

              <div class="flex items-start justify-between gap-4">
                <dt class="text-slate-500">{{ $t("entities:property.detailsCard.propertyIdLabel") }}</dt>
                <dd class="break-all text-right font-mono text-xs text-slate-700">
                  {{ property.id }}
                </dd>
              </div>

              <div v-if="createdAtText" class="flex items-start justify-between gap-4">
                <dt class="text-slate-500">{{ $t("entities:property.detailsCard.createdAtLabel") }}</dt>
                <dd class="text-right font-medium text-slate-900">
                  {{ createdAtText }}
                </dd>
              </div>
            </dl>
          </section>

          <section class="rounded-2xl border border-slate-200 p-4">
            <h2 class="text-sm font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("entities:property.detailsCard.brokerTitle") }}
            </h2>

            <p class="mt-3 text-sm leading-6 text-slate-600">
              {{ $t("entities:property.detailsCard.brokerSubtitle") }}
            </p>

            <RouterLink
              :to="brokerTo"
              class="mt-4 inline-flex items-center justify-center rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 transition hover:bg-slate-50"
              data-testid="property-broker-link"
            >
              {{ $t("common:actions.viewBroker") }}
            </RouterLink>
          </section>
        </div>
      </div>
    </div>
  </article>
</template>
