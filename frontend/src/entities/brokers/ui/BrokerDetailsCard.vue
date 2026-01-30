<script setup lang="ts">
import { computed } from "vue";
import type { BrokerDetailsDto } from "@/features/brokers/api/dtos/broker-details.dto";

type Props = {
  broker: BrokerDetailsDto;
};

const { broker } = defineProps<Props>();

const fullName = computed(() => `${broker.firstName ?? ""} ${broker.lastName ?? ""}`.trim());

const initials = computed(() => {
  const f = broker.firstName?.trim()?.[0] ?? "";
  const l = broker.lastName?.trim()?.[0] ?? "";
  return (f + l).toUpperCase() || "B";
});

const hasEmail = computed(() => !!broker.email?.trim());
const hasPhone = computed(() => !!broker.phoneNumber?.trim());

const emailHref = computed(() => (hasEmail.value ? `mailto:${broker.email!.trim()}` : ""));
const phoneHref = computed(() => {
  if (!hasPhone.value) return "";
  // minimal localozation of phone number: (evnt move to separate func?)
  const digits = broker.phoneNumber!.replace(/[^\d+]/g, "");
  return `tel:${digits}`;
});

const hasAnyContact = computed(() => hasEmail.value || hasPhone.value);

const createdAtText = computed(() => broker.createdAt ?? "");
const updatedAtText = computed(() => broker.updatedAt ?? "");
</script>

<template>
  <article
    class="rounded-2xl border border-slate-200 bg-white shadow-sm"
    data-testid="broker-details-card"
    :aria-label="$t('entities:broker.detailsCardAriaLabel')"
  >
    <div class="p-6 space-y-6">
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
              :alt="fullName ? $t('entities:broker.photoAlt', { name: fullName }) : $t('entities:broker.photoAltFallback')"
              class="h-full w-full object-cover"
              referrerpolicy="no-referrer"
              loading="lazy"
            />

            <!-- initials -->
            <div
              v-else
              class="flex h-full w-full items-center justify-center text-lg font-semibold text-slate-700"
              data-testid="broker-photo-fallback"
              :aria-label="fullName ? $t('entities:broker.initialsAriaLabel', { initials, name: fullName }) : $t('entities:broker.initialsAriaLabelFallback', { initials })"
              role="img"
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

          <!-- Contact -->
          <div
            class="mt-1 text-sm text-slate-600"
            data-testid="broker-contactInfo"
            aria-live="polite"
          >
            <p v-if="hasAnyContact" class="flex flex-wrap gap-x-3 gap-y-1">
              <a
                v-if="hasEmail"
                class="underline underline-offset-2 hover:text-slate-900"
                :href="emailHref"
                :aria-label="$t('entities:broker.emailAriaLabel', { email: broker.email })"
                data-testid="broker-email"
              >
                {{ broker.email }}
              </a>

              <a
                v-if="hasPhone"
                class="underline underline-offset-2 hover:text-slate-900"
                :href="phoneHref"
                :aria-label="$t('entities:broker.phoneAriaLabel', { phone: broker.phoneNumber })"
                data-testid="broker-phone"
              >
                {{ broker.phoneNumber }}
              </a>
            </p>

            <p
              v-else
              class="text-slate-500"
              data-testid="broker-contactInfo-empty"
            >
              {{ $t('entities:broker.contactNotAvailable') }}
            </p>
          </div>
        </div>
      </header>

      <!-- Meta -->
      <section data-testid="broker-meta" aria-label="$t('entities:broker.metaSectionAriaLabel')">
        <dl class="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2">
          <div>
            <dt class="text-slate-500">{{ $t('entities:broker.agencyIdLabel') }}</dt>
            <dd class="font-medium text-slate-900" data-testid="broker-agencyId">
              {{ broker.agencyId }}
            </dd>
          </div>

          <div>
            <dt class="text-slate-500">{{ $t('entities:broker.brokerIdLabel') }}</dt>
            <dd class="font-mono text-xs text-slate-700" data-testid="broker-id">
              {{ broker.id }}
            </dd>
          </div>
        </dl>
      </section>

      <!-- Dates -->
      <section data-testid="broker-dates" aria-label="$t('entities:common.datesSectionAriaLabel')">
        <dl class="grid grid-cols-1 gap-4 text-sm sm:grid-cols-2">
          <div>
            <dt class="text-slate-500">{{ $t('entities:common.createdLabel') }}</dt>
            <dd class="font-medium text-slate-900" data-testid="broker-createdAt">
              <time v-if="createdAtText" :datetime="createdAtText">{{ createdAtText }}</time>
              <span v-else class="text-slate-500">{{ $t('common:notAvailableShort') }}</span>
            </dd>
          </div>

          <div>
            <dt class="text-slate-500">{{ $t('entities:common.updatedLabel') }}</dt>
            <dd class="font-medium text-slate-900" data-testid="broker-updatedAt">
              <time v-if="updatedAtText" :datetime="updatedAtText">{{ updatedAtText }}</time>
              <span v-else class="text-slate-500">{{ $t('common:notAvailableShort') }}</span>
            </dd>
          </div>
        </dl>
      </section>
    </div>
  </article>
</template>
