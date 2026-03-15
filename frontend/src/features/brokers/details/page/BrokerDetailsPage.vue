<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";

import i18n from "@/shared/i18n";
import { brokersApi } from "@/features/brokers/api/brokersApi";
import routes from "@/shared/routes";

import BrokerDetailsCard from "@/entities/brokers/ui/BrokerDetailsCard.vue";
import { ErrorState, LoadingState } from "@/shared/ui/states";

import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";
import type { BrokerDetailsDto } from "@/features/brokers/api/dtos/broker-details.dto";

const route = useRoute();
const router = useRouter();

const state = ref<UIState>("loading");
const error = ref<ApiError | null>(null);
const data = ref<BrokerDetailsDto | null>(null);

const rawId = computed(() => String(route.params.id ?? "").trim());

const id = computed<number>(() => {
  const parsed = Number(rawId.value);
  return Number.isInteger(parsed) && parsed > 0 ? parsed : 0;
});

const pageTitle = computed(() => {
  const fallback = i18n.t("brokers:details.titleFallback");
  const firstName = data.value?.firstName?.trim() ?? "";
  const lastName = data.value?.lastName?.trim() ?? "";
  const fullName = `${firstName} ${lastName}`.trim();

  return fullName || fallback;
});

const errorTitle = computed(() => {
  if (error.value?.kind === "NotFound") return i18n.t("errors:common.title.notFound.broker");
  if (error.value?.kind === "Network") return i18n.t("errors:common.title.network");
  if (error.value?.kind === "Timeout") return i18n.t("errors:common.title.timeout");
  if (error.value?.kind === "BadRequest") return i18n.t("errors:common.title.badRequest");

  return i18n.t("errors:common.title.loadFailed.broker");
});

const errorMessage = computed(() => {
  if (error.value?.kind === "NotFound") {
    return i18n.t("errors:common.message.notFound.broker");
  }

  if (error.value?.kind === "BadRequest") {
    return i18n.t("errors:common.message.invalidBrokerId");
  }

  return error.value?.message ?? i18n.t("errors:common.message.unexpected");
});

async function load(): Promise<void> {
  state.value = "loading";
  error.value = null;
  data.value = null;

  if (id.value <= 0) {
    state.value = "error";
    error.value = {
      name: "BadRequest",
      kind: "BadRequest",
      message: i18n.t("errors:common.message.invalidBrokerId"),
    } as ApiError;

    return;
  }

  try {
    const response = await brokersApi.getById(id.value);
    data.value = response;
    state.value = "success";
  } catch (e) {
    error.value = e as ApiError;
    state.value = "error";

    if (import.meta.env.DEV) {
      console.error("Failed to fetch broker details", e);
    }
  }
}

function goBack(): void {
  router.push(routes.app.brokers.list());
}

onMounted(load);

watch(id, () => {
  load();
});
</script>

<template>
  <section
    class="w-full"
    data-testid="broker-details-page"
    :aria-label="$t('brokers:details.ariaLabel')"
  >
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1
            class="text-2xl font-semibold tracking-tight text-slate-900"
            data-testid="page-title"
          >
            {{ pageTitle }}
          </h1>

          <p class="mt-1 text-sm text-slate-600">
            {{ $t("brokers:details.subtitle") }}
          </p>
        </div>

        <div
          class="flex items-center gap-3"
          role="group"
          :aria-label="$t('brokers:details.pageActionsAriaLabel')"
        >
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="back-to-list-button"
            @click="goBack"
            :aria-label="$t('common:actions.backToList')"
          >
            {{ $t("common:actions.backToList") }}
          </button>

          <button
            type="button"
            class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800"
            data-testid="refresh-button"
            @click="load"
            :aria-label="$t('common:actions.refresh')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>
      </div>

      <div class="mt-8" aria-live="polite">
        <LoadingState
          v-if="state === 'loading'"
          data-testid="loading-state"
          :title="$t('states:loading.brokerDetailsTitle')"
        />

        <ErrorState
          v-else-if="state === 'error'"
          data-testid="error-state"
          :title="errorTitle"
          :message="errorMessage"
          :onRetry="load"
        />

        <BrokerDetailsCard v-else-if="data" :broker="data" />
      </div>
    </div>
  </section>
</template>
