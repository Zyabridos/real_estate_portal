<script setup lang="ts">
import { computed, reactive, watch } from "vue";
import type { LeadFormStatus, LeadFormValues } from "@/shared/types/leads";

type Props = {
  state?: LeadFormStatus;
  disabled?: boolean;
  errorMessage?: string | null;
  successMessage?: string | null;
  testId?: string;
  initialValues?: Partial<LeadFormValues>;
};

const props = withDefaults(defineProps<Props>(), {
  state: "idle",
  disabled: false,
  errorMessage: null,
  successMessage: null,
  testId: "lead-form",
  initialValues: () => ({}),
});

const emit = defineEmits<{
  (e: "submit", values: LeadFormValues): void;
}>();

const values = reactive<LeadFormValues>({
  fullName: props.initialValues.fullName ?? "",
  email: props.initialValues.email ?? "",
  phoneNumber: props.initialValues.phoneNumber ?? "",
  message: props.initialValues.message ?? "",
});

watch(
  () => props.initialValues,
  (next) => {
    if (!next) return;
    values.fullName = next.fullName ?? values.fullName;
    values.email = next.email ?? values.email;
    values.phoneNumber = next.phoneNumber ?? values.phoneNumber;
    values.message = next.message ?? values.message;
  },
  { deep: true }
);

const isLoading = computed(() => props.state === "loading");
const isSuccess = computed(() => props.state === "success");
const isError = computed(() => props.state === "error");

const isFormDisabled = computed(() => props.disabled || isLoading.value || isSuccess.value);

function onSubmit() {
  // for now
  emit("submit", { ...values });
}
</script>

<template>
  <section
    class="w-full rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
    :data-testid="testId"
    aria-label="Lead form"
  >
    <header class="mb-4">
      <h3 class="text-lg font-semibold text-slate-900" data-testid="lead-form-title">
        {{ $t("pages:leads.title") }}
      </h3>
      <p class="mt-1 text-sm text-slate-600" data-testid="lead-form-subtitle">
        {{ $t("pages:leads.subtitle") }}
      </p>
    </header>

    <!-- Success banner -->
    <div
      v-if="isSuccess"
      class="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-900"
      data-testid="lead-success"
      role="status"
    >
      {{ props.successMessage || $t("pages:leads.success") }}
    </div>

    <!-- Error banner -->
    <div
      v-if="isError"
      class="mb-4 rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-900"
      data-testid="lead-error"
      role="alert"
    >
      {{ props.errorMessage || $t("pages:leads.error") }}
    </div>

    <form class="space-y-4" @submit.prevent="onSubmit" data-testid="lead-form-form">
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-fullName">
            {{ $t("pages:leads.fields.fullName.label") }}
          </label>
          <input
            id="lead-fullName"
            v-model.trim="values.fullName"
            type="text"
            autocomplete="fullName"
            :placeholder="$t('pages:leads.fields.fullName.placeholder')"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-slate-900 outline-none ring-0 transition focus:border-slate-300 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:cursor-not-allowed disabled:bg-slate-50"
            :disabled="isFormDisabled"
            data-testid="lead-fullName"
          />
        </div>

        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-email">
            {{ $t("pages:leads.fields.email.label") }}
          </label>
          <input
            id="lead-email"
            v-model.trim="values.email"
            type="email"
            autocomplete="email"
            inputmode="email"
            :placeholder="$t('pages:leads.fields.email.placeholder')"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-slate-900 outline-none ring-0 transition focus:border-slate-300 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:cursor-not-allowed disabled:bg-slate-50"
            :disabled="isFormDisabled"
            data-testid="lead-email"
          />
        </div>

        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-phoneNumber">
            {{ $t("pages:leads.fields.phoneNumber.label") }}
          </label>
          <input
            id="lead-phoneNumber"
            v-model.trim="values.phoneNumber"
            type="tel"
            autocomplete="tel"
            inputmode="tel"
            :placeholder="$t('pages:leads.fields.phoneNumber.placeholder')"
            class="w-full rounded-xl border border-slate-200 bg-white px-3 py-2 text-slate-900 outline-none ring-0 transition focus:border-slate-300 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:cursor-not-allowed disabled:bg-slate-50"
            :disabled="isFormDisabled"
            data-testid="lead-phoneNumber"
          />
        </div>

        <div class="space-y-1 md:col-span-2">
          <label class="text-sm font-medium text-slate-900" for="lead-message">
            {{ $t("pages:leads.fields.message.label") }}
          </label>
          <textarea
            id="lead-message"
            v-model.trim="values.message"
            rows="4"
            :placeholder="$t('pages:leads.fields.message.placeholder')"
            class="w-full resize-none rounded-xl border border-slate-200 bg-white px-3 py-2 text-slate-900 outline-none ring-0 transition focus:border-slate-300 focus:outline-none focus:ring-2 focus:ring-slate-200 disabled:cursor-not-allowed disabled:bg-slate-50"
            :disabled="isFormDisabled"
            data-testid="lead-message"
          />
        </div>
      </div>

      <div class="flex items-center justify-between gap-3 pt-1">
        <p class="text-xs text-slate-500" data-testid="lead-form-note">
          {{ $t("pages:leads.note") }}
        </p>

        <button
          type="submit"
          class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-slate-300 disabled:cursor-not-allowed disabled:bg-slate-400"
          :disabled="isFormDisabled"
          data-testid="lead-submit"
        >
          <span v-if="isLoading" class="inline-flex items-center gap-2" data-testid="lead-submit-loading">
            <span
              class="h-4 w-4 animate-spin rounded-full border-2 border-white/30 border-t-white"
              aria-hidden="true"
            />
            {{ $t("pages:leads.submitLoading") }}
          </span>
          <span v-else data-testid="lead-submit-idle">
            {{ $t("pages:leads.submit") }}
          </span>
        </button>
      </div>
    </form>
  </section>
</template>
