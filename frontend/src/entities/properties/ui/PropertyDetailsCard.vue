<script setup lang="ts">
import { computed } from "vue";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";

type Props = {
  property: PropertyDetailsDto;
};

const { property } = defineProps<Props>();

const formattedPrice = computed(() => property.price.toLocaleString("nb-NO"));

const hasDescription = computed(() => !!property.description?.trim());

const titleText = computed(() => property.title?.trim() ?? "");
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm"
    data-testid="property-details-card"
    :aria-label="$t('entities:property.detailsCardAriaLabel')"
  >
    <div class="p-6 space-y-6">
      <!-- Header -->
      <header class="flex flex-col gap-2">
        <h1
          class="text-2xl font-semibold text-slate-900"
          data-testid="property-title"
        >
          {{ titleText }}
        </h1>

        <p class="text-sm text-slate-600" data-testid="property-subtitle">
          <span class="sr-only">{{ $t('entities:property.subtitleAriaLabel') }}</span>
          {{ property.city }} • {{ property.type }} • {{ property.status }}
        </p>
      </header>

      <!-- Price -->
      <section
        class="flex items-baseline justify-between"
        data-testid="property-price"
        :aria-label="$t('entities:property.priceSectionAriaLabel')"
      >
        <div class="text-2xl font-bold text-slate-900" :aria-label="$t('entities:property.priceValueAriaLabel', { value: formattedPrice })">
          {{ formattedPrice }}
        </div>
        <div class="text-sm text-slate-500">
          {{ $t('common:currency.nok') }}
        </div>
      </section>

      <!-- Meta (dl) -->
      <section data-testid="property-meta" aria-label="$t('entities:property.metaSectionAriaLabel')">
        <dl class="grid grid-cols-2 gap-4 text-sm">
          <div>
            <dt class="text-slate-500">{{ $t('entities:property.typeLabel') }}</dt>
            <dd class="font-medium text-slate-900">
              {{ property.type }}
            </dd>
          </div>

          <div>
            <dt class="text-slate-500">{{ $t('entities:property.statusLabel') }}</dt>
            <dd class="font-medium text-slate-900">
              {{ property.status }}
            </dd>
          </div>

          <div>
            <dt class="text-slate-500">{{ $t('entities:property.cityLabel') }}</dt>
            <dd class="font-medium text-slate-900">
              {{ property.city }}
            </dd>
          </div>

          <div>
            <dt class="text-slate-500">{{ $t('entities:property.propertyIdLabel') }}</dt>
            <dd class="font-mono text-xs text-slate-700">
              {{ property.id }}
            </dd>
          </div>
        </dl>
      </section>

      <!-- Description -->
      <section v-if="hasDescription" data-testid="property-description" aria-label="$t('entities:property.descriptionSectionAriaLabel')">
        <h2 class="mb-2 text-sm font-semibold text-slate-900">
          {{ $t('entities:property.descriptionTitle') }}
        </h2>
        <p class="text-sm leading-relaxed text-slate-700">
          {{ property.description }}
        </p>
      </section>
    </div>
  </article>
</template>
