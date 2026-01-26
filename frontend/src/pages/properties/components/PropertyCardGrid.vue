<script setup lang="ts">
import type { PropertyListItemDto } from "@/shared/api/dtos/properties/property-list-item.dto";

defineProps<{ items: PropertyListItemDto[] }>();
</script>

<template>
  <div class="mt-8">
    <div
      class="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3"
      data-testid="properties-list"
      role="list"
      :aria-label="$t('pages:properties.list.cardsAriaLabel')"
    >
      <article
        v-for="p in items"
        :key="p.id"
        data-testid="property-card"
        class="rounded-2xl border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md"
        role="listitem"
      >
        <div class="p-5" :data-testid="`property-card-${p.id}`">
          <div class="flex items-start justify-between gap-3">
            <div>
              <h2 class="text-base font-semibold text-slate-900">{{ p.title }}</h2>
              <p class="mt-1 text-sm text-slate-600" data-testid="property-card-meta">
                {{ p.city }} • {{ p.type }} • {{ p.status }}
              </p>
            </div>

            <div class="text-right">
              <div
                class="text-sm font-semibold text-slate-900"
                :aria-label="$t('entities:property.priceValueAriaLabel', { value: p.price.toLocaleString() })"
              >
                {{ p.price.toLocaleString() }}
              </div>
              <div class="text-xs text-slate-500">{{ $t("common:currency.nok") }}</div>
            </div>
          </div>

          <div class="mt-4 flex items-center justify-between">
            <RouterLink
              :to="`/properties/${p.id}`"
              class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
              :aria-label="$t('pages:properties.list.viewDetailsAriaLabel', { id: p.id })"
            >
              {{ $t("common:actions.viewDetails") }}
            </RouterLink>

            <div class="text-xs text-slate-500">
              {{ $t("common:pagination.idShort") }}:
              <span class="font-mono">{{ p.id }}</span>
            </div>
          </div>
        </div>
      </article>
    </div>
  </div>
</template>
