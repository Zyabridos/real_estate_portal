<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import i18n from "@/shared/i18n";
import routes from "@/shared/routes";
import LeadForm from "@/features/leads/create/forms/LeadForm.vue";
import { leadsApi } from "@/features/leads/api/leadsApi";

import { propertiesApi } from "@/features/properties/api/propertiesApi";
import PropertyLeadBackgroundCarousel from "@/features/leads/create/ui/PropertyLeadBackgroundCarousel.vue";
import defaultCarouselImage from "@/assets/images/DefaultCarouselImage.png";
import {
  getPropertyImageUrls,
  type PropertyImageSource,
} from "@/shared/utils/properties/getPropertyImageUrls";

import type { LeadFormStatus, LeadFormValues } from "@/entities/leads/model/types";
import type { ApiError } from "@/shared/types/errors";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";

const route = useRoute();
const router = useRouter();

const state = ref<LeadFormStatus>("idle");
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const formKey = ref(0);
const leadFormRef = ref<InstanceType<typeof LeadForm> | null>(null);
const property = ref<PropertyDetailsDto | null>(null);

const rawPropertyId = computed(() => String(route.params.id ?? "").trim());

const propertyId = computed<number>(() => {
  const parsed = Number(rawPropertyId.value);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : 0;
});

const propertyTitle = computed(() => {
  const title = property.value?.title?.trim();
  return title || i18n.t("leads:form.title");
});

function normalizeUrl(value?: string | null): string | null {
  const normalized = value?.trim();
  return normalized ? normalized : null;
}

const carouselImages = computed(() =>
  getPropertyImageUrls(property.value as PropertyImageSource | null, {
    fallbackImage: defaultCarouselImage,
    preferImageUrlsFirst: true,
  }),
);

async function loadPropertyPreview(): Promise<void> {
  if (propertyId.value <= 0) {
    property.value = null;
    return;
  }

  try {
    property.value = await propertiesApi.getById(propertyId.value);
  } catch {
    property.value = null;
  }
}

watch(
  propertyId,
  () => {
    void loadPropertyPreview();
  },
  { immediate: true },
);

function goBackToDetails(): void {
  if (propertyId.value <= 0) {
    router.push(routes.app.properties.list());
    return;
  }

  router.push({
    path: routes.app.properties.details(propertyId.value),
    query: route.query,
  });
}

async function onSubmit(values: LeadFormValues): Promise<void> {
  if (propertyId.value <= 0) {
    state.value = "error";
    errorMessage.value = i18n.t("errors:common.message.invalidPropertyId");
    return;
  }

  state.value = "loading";
  errorMessage.value = null;
  successMessage.value = null;

  try {
    await leadsApi.createLead({
      propertyId: propertyId.value,
      fullName: values.fullName,
      email: values.email?.trim() || null,
      phoneNumber: values.phoneNumber?.trim() || null,
      message: values.message?.trim() || null,
    });

    state.value = "success";
    successMessage.value = null;
    formKey.value += 1;
  } catch (e) {
    const err = e as ApiError;

    if (err.kind === "Validation") {
      state.value = "idle";
      leadFormRef.value?.applyServerErrors?.(err.problemDetails);
      return;
    }

    state.value = "error";
    errorMessage.value = err.message ?? i18n.t("errors:common.message.unexpected");
  }
}
</script>

<template>
  <section
    class="w-full"
    data-testid="lead-create-page"
    :aria-label="$t('leads:form.pageAriaLabel')"
  >
    <div class="mx-auto w-full max-w-7xl px-4 sm:px-6 lg:px-8">
      <div
        class="relative isolate min-h-[calc(100vh-10rem)] overflow-hidden rounded-[2rem] border border-slate-200 shadow-xl"
      >
        <PropertyLeadBackgroundCarousel :images="carouselImages" />

        <button
          type="button"
          class="absolute left-4 top-4 z-20 rounded-xl border border-white/30 bg-white/15 px-4 py-2 text-sm font-medium text-white backdrop-blur-md transition hover:bg-white/25"
          data-testid="back-to-details-button"
          @click="goBackToDetails"
          :aria-label="$t('common:actions.back')"
        >
          {{ $t("common:actions.back") }}
        </button>

        <div class="relative z-10 flex min-h-[calc(100vh-10rem)] items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
          <div class="w-full max-w-2xl">
            <header class="mb-6 text-center text-white">
              <h1
                class="text-3xl font-semibold tracking-tight sm:text-4xl"
                data-testid="lead-create-title"
              >
                {{ $t("leads:form.title") }}
              </h1>

              <p
                class="mt-2 text-sm text-white/85 sm:text-base"
                data-testid="lead-create-subtitle"
              >
                {{ propertyTitle }}
              </p>
            </header>

            <LeadForm
              :key="formKey"
              ref="leadFormRef"
              :state="state"
              :errorMessage="errorMessage"
              :successMessage="successMessage"
              testId="lead-form"
              class="mx-auto w-full border-white/25 bg-white/95 shadow-2xl backdrop-blur-md"
              @submit="onSubmit"
            />
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
