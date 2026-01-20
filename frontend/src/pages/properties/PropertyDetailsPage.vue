<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import { propertiesApi } from "@/shared/api/properties";
import routes from "@/shared/routes"

import { ErrorState, LoadingState } from "@/shared/ui/states";
import PropertyDetailsCard from "@/entities/properties/ui/PropertyDetailsCard.vue";

import type { ApiError } from "@/shared/types/errors";
import type { UIStatus } from "@/shared/types/ui";
import type { PropertyDetailsDto } from "@/shared/api/dtos/properties/property-details.dto";

const route = useRoute();
const router = useRouter();

const state = ref<UIStatus>("loading");
const error = ref<ApiError | null>(null);
const data = ref<PropertyDetailsDto | null>(null);

const id = computed(() => String(route.params.id ?? "").trim());

const pageTitle = computed(() =>
  data.value?.title?.trim() ? data.value.title : "Property details"
);

const errorTitle = computed(() => {
  if (error.value?.kind === "NotFound") return "Property not found";
  if (error.value?.kind === "Network") return "Network error";
  if (error.value?.kind === "Timeout") return "Request timed out";
  return "Failed to load property";
});

const errorMessage = computed(() => {
  if (error.value?.kind === "NotFound") {
    return "We couldn't find this property. It may have been removed or the link is incorrect.";
  }
  return error.value?.message ?? "Unexpected error.";
});

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;
  data.value = null;

  if (!id.value) {
    state.value = "error";
    error.value = {
      kind: "BadRequest",
      message: "Invalid property id.",
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
      // eslint-disable-next-line no-console
      console.error("Failed to fetch property details", e);
    }
  }
}

function goBack(): void {
  router.push({ path: routes.app.brokers(), query: route.query });
}

onMounted(load);

// TODO: remove hardcoded texts
// If the user navigates between property detail pages (e.g. via links), reload data when the id changes
watch(id, () => {
  load();
});
</script>

<template>
  <section class="w-full" data-testid="property-details-page">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="page-title">
            {{ pageTitle }}
          </h1>
          <p class="mt-1 text-sm text-slate-600">
            Property details loaded from the backend.
          </p>
        </div>

        <div class="flex items-center gap-3">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="back-to-list-button"
            @click="goBack"
          >
            Back to list
          </button>

          <button
            type="button"
            class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            data-testid="refresh-button"
            @click="load"
          >
            Refresh
          </button>
        </div>
      </div>

      <div class="mt-8">
        <LoadingState
          v-if="state === 'loading'"
          data-testid="loading-state"
          title="Loading property…"
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
