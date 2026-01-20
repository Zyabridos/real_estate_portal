<script setup lang="ts">
import { computed } from "vue";
import type { PropertyDetailsDto } from "@/shared/api/dtos/properties/property-details.dto";

type Props = {
  property: PropertyDetailsDto;
};

const { property } = defineProps<Props>();

const formattedPrice = computed(() =>
  property.price.toLocaleString("nb-NO")
);
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm"
    data-testid="property-details-card"
  >
    <div class="p-6 space-y-6">
      <!-- Header -->
      <header class="flex flex-col gap-2">
        <h1
          class="text-2xl font-semibold text-slate-900"
          data-testid="property-title"
        >
          {{ property.title }}
        </h1>

        <p
          class="text-sm text-slate-600"
          data-testid="property-subtitle"
        >
          {{ property.city }} • {{ property.type }} • {{ property.status }}
        </p>
      </header>

      <!-- Price -->
      <section
        class="flex items-baseline justify-between"
        data-testid="property-price"
      >
        <div class="text-2xl font-bold text-slate-900">
          {{ formattedPrice }}
        </div>
        <div class="text-sm text-slate-500">NOK</div>
      </section>

      <!-- Meta -->
      <section
        class="grid grid-cols-2 gap-4 text-sm"
        data-testid="property-meta"
      >
        <div>
          <div class="text-slate-500">Type</div>
          <div class="font-medium text-slate-900">
            {{ property.type }}
          </div>
        </div>

        <div>
          <div class="text-slate-500">Status</div>
          <div class="font-medium text-slate-900">
            {{ property.status }}
          </div>
        </div>

        <div>
          <div class="text-slate-500">City</div>
          <div class="font-medium text-slate-900">
            {{ property.city }}
          </div>
        </div>

        <div>
          <div class="text-slate-500">Property ID</div>
          <div class="font-mono text-xs text-slate-700">
            {{ property.id }}
          </div>
        </div>
      </section>

      <!-- Description -->
      <section
        v-if="property.description"
        data-testid="property-description"
      >
        <h2 class="mb-2 text-sm font-semibold text-slate-900">
          Description
        </h2>
        <p class="text-sm leading-relaxed text-slate-700">
          {{ property.description }}
        </p>
      </section>
    </div>
  </article>
</template>

