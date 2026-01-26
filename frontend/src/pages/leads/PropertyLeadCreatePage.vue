<script setup lang="ts">
import { computed, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import routes from "@/shared/routes";
import LeadForm from "@/pages/leads/forms/LeadForm.vue";
import { leadsApi } from "@/shared/api/leads";
import type { LeadFormStatus, LeadFormValues } from "@/shared/types/leads";
import type { ApiError } from "@/shared/types/errors";

const route = useRoute();
const router = useRouter();

const state = ref<LeadFormStatus>("idle");
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const formKey = ref(0);
const leadFormRef = ref<InstanceType<typeof LeadForm> | null>(null);

const propertyId = computed(() => String(route.params.id ?? "").trim());

function goBackToDetails(): void {
  router.push({ path: routes.app.propertyDetails(propertyId.value), query: route.query });
}

async function onSubmit(values: LeadFormValues) {
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
    successMessage.value = null; // TODO: add some text
    formKey.value += 1; // clear form vie remount
  } catch (e) {
    const err = e as ApiError;

    if (err.kind === "Validation") {
      state.value = "idle";
      leadFormRef.value?.applyServerErrors?.(err.problemDetails);
      return;
    }

    // other errors -> show red banner
    state.value = "error";
    errorMessage.value = err.message ?? "Unexpected error.";
  }
}
</script>

<template>
  <section
    class="flex items-center w-full"
    data-testid="lead-create-page"
    :aria-label="$t('pages:properties.details.leads.ariaLabel')"
  >
    <div class="w-full px-6 py-2">
      <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
          <h1 class="text-2xl font-semibold tracking-tight text-slate-900" data-testid="lead-create-title">
            {{ $t("pages:properties.details.leads.pageTitle") }}
          </h1>
          <p class="mt-1 text-sm text-slate-600" data-testid="lead-create-subtitle">
            {{ $t("pages:properties.details.leads.pageSubtitle") }}
          </p>
        </div>

        <div class="flex items-center gap-3" role="group" :aria-label="$t('common:aria.pageActions')">
          <button
            type="button"
            class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50"
            data-testid="back-to-details-button"
            @click="goBackToDetails"
            :aria-label="$t('pages:properties.details.leads.actions.backAria')"
          >
            {{ $t("pages:properties.details.leads.actions.back") }}
          </button>
        </div>
      </div>

      <div class="mt-8 max-w-2xl" aria-live="polite">
        <LeadForm
          :key="formKey"
          ref="leadFormRef"
          :state="state"
          :errorMessage="errorMessage"
          :successMessage="successMessage"
          :testId="'lead-form'"
          @submit="onSubmit"
        />
      </div>
    </div>
  </section>
</template>
