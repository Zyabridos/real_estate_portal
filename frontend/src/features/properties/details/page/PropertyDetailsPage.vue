<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import i18n from "@/shared/i18n";
import { propertiesApi } from "@/features/properties/api/propertiesApi";
import routes from "@/shared/routes";

import { ErrorState, LoadingState } from "@/shared/ui/states";
import PropertyDetailsCard from "@/entities/properties/ui/PropertyDetailsCard.vue";

import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";
import type { PropertyDetailsDto } from "@/features/properties/api/dtos/property-details.dto";

const route = useRoute();
const router = useRouter();

const state = ref<UIState>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PropertyDetailsDto | null>(null);

const id = computed(() => String(route.params.id ?? "").trim());

const pageTitle = computed(() => {
  const fallback = i18n.t("pages:properties.details.titleFallback");
  const title = data.value?.title?.trim();
  return title ? title : fallback;
});

const canCreateLead = computed(() => state.value === "success" && !!data.value?.id?.trim());

function goCreateLead(): void {
  const propertyId = data.value?.id?.trim();
  if (!propertyId) return;

  router.push({ path: routes.app.leads.create(propertyId), query: route.query });
}

const errorTitle = computed(() => {
  if (error.value?.kind === "NotFound") return i18n.t("errors:title.notFound.property");
  if (error.value?.kind === "Network") return i18n.t("errors:title.network");
  if (error.value?.kind === "Timeout") return i18n.t("errors:title.timeout");
  if (error.value?.kind === "BadRequest") return i18n.t("errors:title.badRequest");
  return i18n.t("errors:titles.genericLoadFailed");
});

const errorMessage = computed(() => {
  if (error.value?.kind === "NotFound") {
    return i18n.t("errors:message.notFound.property");
  }

  if (error.value?.kind === "BadRequest") {
    return i18n.t("errors:message.invalidPropertyId");
  }

  return error.value?.message ?? i18n.t("errors:message.unexpected");
});

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;
  data.value = null;

  if (!id.value) {
    state.value = "error";
    error.value = {
      kind: "BadRequest",
      message: i18n.t("errors:message.invalidPropertyId"),
    } as ApiError;
    return;
  }

  try {
    const res = await propertiesApi.getById(id.value);
    data.value = res;
    state.value = "success";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";

    if (import.meta.env.DEV) {
      console.error("Failed to fetch property details", e);
    }
  }
}

function goBack(): void {
  router.push(routes.app.properties.list());
}

onMounted(load);

watch(id, () => {
  load();
});
</script>

<template>
  <section class="w-full" data-testid="property-details-page" :aria-label="$t('pages:properties.details.ariaLabel')">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="page-title">
            {{ pageTitle }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("pages:properties.details.subtitle") }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:aria.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="back-to-list-button"
            @click="goBack"
            :aria-label="$t('common:actions.backToListAria')"
          >
            {{ $t("common:actions.backToList") }}
          </button>

          <button
            type="button"
            class="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-medium text-white hover:bg-emerald-500 disabled:cursor-not-allowed disabled:bg-emerald-300"
            data-testid="create-lead-button"
            @click="goCreateLead"
            :disabled="!canCreateLead"
            :aria-label="$t('pages:properties.details.actions.createLeadAria')"
          >
            {{ $t("pages:properties.details.actions.createLead") }}
          </button>

          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="refresh-button"
            @click="load"
            :aria-label="$t('common:actions.refreshAria')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <div class="mt-8" aria-live="polite">
        <LoadingState
          v-if="state === 'loading'"
          data-testid="loading-state"
          :title="$t('states:loading.propertyDetailsTitle')"
        />

        <ErrorState
          v-else-if="state === 'error'"
          data-testid="error-state"
          :title="errorTitle"
          :message="errorMessage"
          :onRetry="load"
        />

        <PropertyDetailsCard v-else-if="data" :property="data" />
      </div>
    </div>
  </section>
</template>
