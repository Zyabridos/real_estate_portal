<script setup lang="ts">
import { computed, onMounted, watch, onBeforeUnmount } from "vue";
import { useRoute, useRouter } from "vue-router";

import i18n from "@/shared/i18n";
import routes from "@/shared/routes";
import { ErrorState, LoadingState } from "@/shared/ui/states";
import AgencyDetailsCard from "@/entities/agencies/ui/AgencyDetailsCard.vue";

import agenciesStore from "@/entities/agencies/model/agenciesStore";
import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

const route = useRoute();
const router = useRouter();
const store = agenciesStore();

const id = computed(() => String(route.params.id ?? "").trim());

const agency = computed(() => store.getById(id.value));
const status = computed<UIState>(() => store.getDetailsStatus(id.value));
const error = computed<ApiError | null>(() => store.getDetailsError(id.value));

const pageTitle = computed(() => {
  const fallback = i18n.t("pages:agencies.details.titleFallback");
  const name = agency.value?.name?.trim();
  return name ? name : fallback;
});

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
  if (!id.value) return;
  await store.fetchById(id.value, { force });
}

function goBack(): void {
  router.push(routes.app.agencies.list());
}

onMounted(() => void load(false));
watch(id, () => void load(false));

onBeforeUnmount(() => {
  store.cancelDetailsRequest();
});
</script>

<template>
  <section class="w-full" data-testid="agency-details-page" :aria-label="$t('pages:agencies.details.ariaLabel')">
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="page-title">
            {{ pageTitle }}
          </h1>
          <p class="mt-1 text-sm text-slate-600">
            {{ $t("pages:agencies.details.subtitle") }}
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
            class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            data-testid="refresh-button"
            @click="load(true)"
            :aria-label="$t('common:actions.refreshAria')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <div class="mt-8" aria-live="polite">
        <ErrorState
          v-if="!id"
          data-testid="error-state"
          :title="$t('errors:titles.badRequest')"
          :message="$t('errors:messages.invalidAgencyId')"
          :onRetry="() => load(true)"
        />

        <LoadingState
          v-else-if="status === 'loading' || status === 'idle'"
          data-testid="loading-state"
        />

        <ErrorState
          v-else-if="status === 'error'"
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
