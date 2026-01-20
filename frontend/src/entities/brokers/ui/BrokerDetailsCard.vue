<script setup lang="ts">
import { computed } from "vue";
import type { BrokerDetailsDto } from "@/shared/api/dtos/brokers/broker-details.dto";

type Props = {
  broker: BrokerDetailsDto;
};

const { broker } = defineProps<Props>();

const fullName = computed(() => `${broker.firstName} ${broker.lastName}`.trim());

const initials = computed(() => {
  const f = broker.firstName?.trim()?.[0] ?? "";
  const l = broker.lastName?.trim()?.[0] ?? "";
  return (f + l).toUpperCase() || "B";
});

const contactParts = computed(() => {
  const parts: string[] = [];
  if (broker.email) parts.push(broker.email);
  if (broker.phoneNumber) parts.push(broker.phoneNumber);
  return parts;
});

const contactInfo = computed(() => contactParts.value.join(" • "));
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm"
    data-testid="broker-details-card"
  >
    <div class="p-6 space-y-6">
      <!-- Header -->
      <header class="flex items-start gap-4">
        <!-- Photo / Avatar -->
        <div class="shrink-0">
          <div
            class="h-16 w-16 overflow-hidden rounded-2xl border border-slate-200 bg-slate-50"
            data-testid="broker-photo"
          >
            <img
              v-if="broker.photoUrl"
              :src="broker.photoUrl"
              :alt="fullName"
              class="h-full w-full object-cover"
              referrerpolicy="no-referrer"
            />
            <div
              v-else
              class="flex h-full w-full items-center justify-center text-lg font-semibold text-slate-700"
              data-testid="broker-photo-fallback"
              aria-hidden="true"
            >
              {{ initials }}
            </div>
          </div>
        </div>

        <div class="min-w-0 flex-1">
          <h1
            class="truncate text-2xl font-semibold text-slate-900"
            data-testid="broker-fullName"
          >
            {{ fullName }}
          </h1>

          <p
            v-if="contactInfo"
            class="mt-1 text-sm text-slate-600"
            data-testid="broker-contactInfo"
          >
            {{ contactInfo }}
          </p>

          <p
            v-else
            class="mt-1 text-sm text-slate-500"
            data-testid="broker-contactInfo-empty"
          >
            Contact details are not available.
          </p>
        </div>
      </header>

      <!-- Meta -->
      <section class="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2" data-testid="broker-meta">
        <div>
          <div class="text-slate-500">Agency ID</div>
          <div class="font-medium text-slate-900" data-testid="broker-agencyId">
            {{ broker.agencyId }}
          </div>
        </div>

        <div>
          <div class="text-slate-500">Broker ID</div>
          <div class="font-mono text-xs text-slate-700" data-testid="broker-id">
            {{ broker.id }}
          </div>
        </div>
      </section>

      <!-- Optional timestamps (если хочешь оставить для dev/MVP) -->
      <section class="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2" data-testid="broker-dates">
        <div>
          <div class="text-slate-500">Created</div>
          <div class="font-medium text-slate-900" data-testid="broker-createdAt">
            {{ broker.createdAt }}
          </div>
        </div>
        <div>
          <div class="text-slate-500">Updated</div>
          <div class="font-medium text-slate-900" data-testid="broker-updatedAt">
            {{ broker.updatedAt }}
          </div>
        </div>
      </section>
    </div>
  </article>
</template>
