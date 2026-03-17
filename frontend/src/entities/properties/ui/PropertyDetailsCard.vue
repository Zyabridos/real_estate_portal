<script setup lang="ts">
import { computed } from "vue";
import { RouterLink } from "vue-router";

import i18n from "@/shared/i18n";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";
import routes from "@/shared/routes";
import { formatDateByLocale} from "@/shared/utils/formatters/formatDateByLocale.ts";
import house_default from "@/assets/images/house_default.png";
import PropertyImageGallery from "@/entities/properties/ui/PropertyImageGallery.vue";

type Props = {
  property: PropertyDetailsDto;
};

type PropertyImageObject = {
  src?: string;
  url?: string;
};

type PropertyWithGallery = PropertyDetailsDto & {
  imageUrls?: string[];
  images?: Array<string | PropertyImageObject>;
};

const { property } = defineProps<Props>();

const formattedPrice = computed(() => property.price.toLocaleString("nb-NO"));
const hasDescription = computed(() => !!property.description?.trim());
const titleText = computed(() => property.title?.trim() ?? "");

function normalizeUrl(value?: string | null): string | null {
  const normalized = value?.trim();
  return normalized ? normalized : null;
}

const imageBadges = computed(() => [
  {
    label: String(localizedStatus.value),
    class: `${statusClass.value} bg-white/85`,
    testId: "property-status-badge",
  },
  {
    label: String(localizedType.value),
    class: `${typeClass.value} bg-white/85`,
    testId: "property-type-badge",
  },
]);

const galleryImages = computed(() => {
  const urls: string[] = [];
  const extendedProperty = property as PropertyWithGallery;

  const pushUnique = (value?: string | null): void => {
    const normalized = normalizeUrl(value);

    if (normalized && !urls.includes(normalized)) {
      urls.push(normalized);
    }
  };

  pushUnique(property.mainImageUrl);

  for (const imageUrl of extendedProperty.imageUrls ?? []) {
    pushUnique(imageUrl);
  }

  for (const image of extendedProperty.images ?? []) {
    if (typeof image === "string") {
      pushUnique(image);
    } else {
      pushUnique(image.src ?? image.url);
    }
  }

  if (!urls.length) {
    urls.push(house_default);
  }

  return urls.map((src, index) => ({
    src,
    alt: index === 0
      ? titleText.value || "Property image"
      : `${titleText.value || "Property image"} ${index + 1}`,
  }));
});

const dateLocale = computed(() => {
  const language = i18n.resolvedLanguage ?? i18n.language;

  if (language === "ru") return "ru-RU";
  if (language === "no") return "nb-NO";

  return "en-GB";
});

const createdAtText = computed(() => {
  return formatDateByLocale(
    property.createdAt,
    i18n.resolvedLanguage ?? i18n.language,
    "long",
  );
});

const brokerTo = computed(() => {
  const brokerId = Number(property.brokerId);

  return Number.isInteger(brokerId) && brokerId > 0
    ? routes.app.brokers.details(brokerId)
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
  if (property.status === "Active") return i18n.t("properties:card.status.active");
  if (property.status === "Sold") return i18n.t("properties:card.status.sold");
  return property.status;
});

const localizedType = computed(() => {
  if (property.type === "Apartment") return i18n.t("properties:card.type.apartment");
  if (property.type === "House") return i18n.t("properties:card.type.house");
  if (property.type === "Commercial") return i18n.t("properties:card.type.commercial");
  return property.type;
});
</script>

<template>
  <article
    class="grid h-full gap-6 lg:min-h-[calc(100vh-16rem)] lg:grid-cols-[minmax(420px,1.02fr)_minmax(0,0.92fr)] xl:grid-cols-[minmax(520px,1.08fr)_minmax(0,0.86fr)]"
    data-testid="property-details-card"
    :aria-label="$t('properties:card.detailsCard.detailsCardAriaLabel')"
  >
    <div class="lg:self-start">
      <PropertyImageGallery
        :images="galleryImages"
        :badges="imageBadges"
      />
    </div>

    <section
      class="flex h-full flex-col rounded-3xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5 md:p-6"
    >
      <div class="flex flex-col gap-5">
        <div class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <section
            v-if="hasDescription"
            class="min-w-0 flex-1"
            data-testid="property-description"
            :aria-label="$t('properties:card.detailsCard.descriptionSectionAriaLabel')"
          >
            <h2 class="text-xs font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("properties:card.detailsCard.descriptionTitle") }}
            </h2>

            <p class="mt-2 whitespace-pre-line text-sm leading-6 text-slate-700">
              {{ property.description }}
            </p>
          </section>

          <div class="shrink-0 rounded-2xl border border-slate-200 bg-slate-50 px-3 py-2 text-right sm:px-4">
            <p class="text-[11px] font-semibold uppercase tracking-wide text-slate-500">
              {{ $t("properties:card.detailsCard.priceLabel") }}
            </p>

            <div
              class="mt-0.5 text-xl font-bold tracking-tight text-slate-900 sm:text-2xl"
              data-testid="property-price"
              :aria-label="$t('properties:card.detailsCard.priceValueAriaLabel', { value: formattedPrice })"
            >
              {{ formattedPrice }}
            </div>

            <div class="text-[11px] text-slate-500">
              {{ $t("common:app.currency.nok") }}
            </div>
          </div>
        </div>

        <div class="min-w-0">

          <p class="whitespace-nowrap"
             data-testid="property-address"
             :title="property.address">
            <span class="font-medium text-slate-500">
              {{ $t("properties:card.detailsCard.streetLabel") }}:
            </span>
            <span class="ml-1 font-semibold text-slate-900" data-testid="property-bedrooms">
              {{ property.address }}
            </span>
          </p>

        </div>
      </div>

      <section
        class="mt-5"
        :aria-label="$t('properties:card.detailsCard.metaSectionAriaLabel')"
      >
        <div class="flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-slate-700">
          <p class="whitespace-nowrap">
            <span class="font-medium text-slate-500">
              {{ $t("properties:card.detailsCard.bedroomsLabel") }}:
            </span>
            <span class="ml-1 font-semibold text-slate-900" data-testid="property-bedrooms">
              {{ property.bedrooms }}
            </span>
          </p>

          <p class="whitespace-nowrap">
            <span class="font-medium text-slate-500">
              {{ $t("properties:card.detailsCard.bathroomsLabel") }}:
            </span>
            <span class="ml-1 font-semibold text-slate-900" data-testid="property-bathrooms">
              {{ property.bathrooms }}
            </span>
          </p>

          <p class="whitespace-nowrap">
            <span class="font-medium text-slate-500">
              {{ $t("properties:card.detailsCard.areaLabel") }}:
            </span>
            <span class="ml-1 font-semibold text-slate-900" data-testid="property-area">
              {{ property.area }} {{ $t("properties:card.detailsCard.areaUnit") }}
            </span>
          </p>

          <p class="whitespace-nowrap">
            <span class="font-medium text-slate-500">
              {{ $t("properties:card.detailsCard.cityLabel") }}:
            </span>
            <span class="ml-1 font-semibold text-slate-900" data-testid="property-city">
              {{ property.city }}
            </span>
          </p>
        </div>
      </section>

      <div class="mt-5 grid gap-3 xl:grid-cols-[1.05fr_0.95fr]">
        <section class="rounded-2xl border border-slate-200 px-4 py-4">
          <h2 class="text-xs font-semibold uppercase tracking-wide text-slate-500">
            {{ $t("properties:card.detailsCard.metaTitle") }}
          </h2>

          <dl class="mt-3 space-y-3 text-sm">
            <div class="flex items-start justify-between gap-4">
              <dt class="text-slate-500">{{ $t("properties:card.detailsCard.typeLabel") }}</dt>
              <dd class="text-right font-medium text-slate-900">
                {{ localizedType }}
              </dd>
            </div>

            <div class="flex items-start justify-between gap-4">
              <dt class="text-slate-500">{{ $t("properties:card.detailsCard.statusLabel") }}</dt>
              <dd class="text-right font-medium text-slate-900">
                {{ localizedStatus }}
              </dd>
            </div>

            <div class="flex items-start justify-between gap-4">
              <dt class="text-slate-500">{{ $t("properties:card.detailsCard.propertyIdLabel") }}</dt>
              <dd class="break-all text-right font-mono text-xs text-slate-700">
                {{ property.id }}
              </dd>
            </div>

            <div v-if="createdAtText" class="flex items-start justify-between gap-4">
              <dt class="text-slate-500">{{ $t("properties:card.detailsCard.createdAtLabel") }}</dt>
              <dd class="text-right font-medium text-slate-900">
                {{ createdAtText }}
              </dd>
            </div>
          </dl>
        </section>

        <section class="rounded-2xl border border-slate-200 bg-slate-50 px-4 py-4">
          <h2 class="text-xs font-semibold uppercase tracking-wide text-slate-500">
            {{ $t("properties:card.detailsCard.brokerTitle") }}
          </h2>

          <p class="mt-2 text-sm leading-6 text-slate-600">
            {{ $t("properties:card.detailsCard.brokerSubtitle") }}
          </p>

          <RouterLink
            :to="brokerTo"
            class="mt-4 inline-flex items-center justify-center rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 transition hover:bg-slate-100"
            data-testid="property-broker-link"
          >
            {{ $t("common:actions.viewBroker") }}
          </RouterLink>
        </section>
      </div>
    </section>
  </article>
</template>
