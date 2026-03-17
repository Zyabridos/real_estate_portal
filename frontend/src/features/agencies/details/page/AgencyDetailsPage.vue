<script setup lang="ts">
import { computed, onBeforeUnmount, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import i18n from "@/shared/i18n";
import routes from "@/shared/routes";
import { ErrorState, LoadingState } from "@/shared/ui/states";
import EntityDetailsErrorState from "@/shared/ui/errors/EntityDetailsErrorState.vue";
import AgencyDetailsCard from "@/entities/agencies/ui/AgencyDetailsCard.vue";
import { useAgenciesStore } from "@/entities/agencies/model/agenciesStore";

import { parsePositiveIntParam } from "@/shared/utils/parsePositiveIntParam";
import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

const route = useRoute();
const router = useRouter();
const store = useAgenciesStore();

const backToList = routes.app.agencies.list();

const rawId = computed(() => String(route.params.id ?? "").trim());
const id = computed<number>(() => parsePositiveIntParam(route.params.id));

const agency = computed(() => (id.value > 0 ? store.getById(id.value) : null));

const state = computed<UIState>(() =>
  id.value > 0 ? store.getDetailsStatus(id.value) : "idle",
);

const error = computed<ApiError | null>(() =>
  id.value > 0 ? store.getDetailsError(id.value) : null,
);

const pageTitle = computed(() => {
  const fallback = i18n.t("agencies:details.titleFallback");
  const name = agency.value?.name?.trim();
  return name ? name : fallback;
});

const showInvalidId = computed(() => id.value <= 0);
const showNotFound = computed(
  () => state.value === "error" && error.value?.kind === "NotFound",
);
const showGenericError = computed(
  () => state.value === "error" && !showInvalidId.value && !showNotFound.value,
);

const errorTitle = computed(() => {
  if (error.value?.kind === "NotFound") return i18n.t("errors:titles.agencyNotFound");
  if (error.value?.kind === "Network") return i18n.t("errors:titles.network");
  if (error.value?.kind === "Timeout") return i18n.t("errors:titles.timeout");
  if (error.value?.kind === "BadRequest") return i18n.t("errors:titles.badRequest");

  return i18n.t("errors:titles.genericLoadFailed");
});

const errorMessage = computed(() => {
  if (error.value?.kind === "NotFound") return i18n.t("errors:messages.agencyNotFoundLong");
  if (error.value?.kind === "BadRequest") return i18n.t("errors:messages.invalidAgencyId");

  return error.value?.message ?? i18n.t("errors:messages.unexpected");
});

async function load(force = false): Promise<void> {
  if (id.value <= 0) return;
  await store.fetchById(id.value, { force });
}

function goBack(): void {
  router.push(backToList);
}

watch(id, () => void load(false), { immediate: true });

onBeforeUnmount(() => {
  store.cancelDetailsRequest();
});
</script>

<template>
  <section
    class="w-full"
    data-testid="agency-details-page"
    :aria-label="$t('agencies:details.ariaLabel')"
  >
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="page-title">
            {{ pageTitle }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("agencies:details.subtitle") }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:app.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="back-to-list-button"
            :aria-label="$t('common:actions.backToListAria')"
            @click="goBack"
          >
            {{ $t("common:actions.backToList") }}
          </button>

          <button
            type="button"
            class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            data-testid="refresh-button"
            :aria-label="$t('common:actions.refreshAria')"
            @click="load(true)"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <div class="mt-8" aria-live="polite">
        <EntityDetailsErrorState
          v-if="showInvalidId"
          entity="agency"
          variant="invalidId"
          :requested-id="rawId"
          :back-to="backToList"
        />

        <EntityDetailsErrorState
          v-else-if="showNotFound"
          entity="agency"
          variant="notFound"
          :requested-id="rawId"
          :back-to="backToList"
          :on-refresh="() => load(true)"
        />

        <LoadingState
          v-else-if="state === 'loading'"
          testId="agencies-loading"
          :title="$t('common:states.loading.genericTitle')"
          :subtitle="$t('agencies:details.subtitle')"
        />

        <ErrorState
          v-else-if="showGenericError"
          data-testid="error-state"
          :title="errorTitle"
          :message="errorMessage"
          :onRetry="() => load(true)"
        />

        <AgencyDetailsCard v-else-if="agency" :agency="agency" />
      </div>
    </div>
  </section>
</template>
