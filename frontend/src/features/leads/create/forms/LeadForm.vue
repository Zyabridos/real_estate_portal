<script setup lang="ts">
import { toRefs, proxyRefs } from "vue";

import type { LeadFormValues, LeadFormProps } from "@/entities/leads/model/types";
import { MESSAGE_MAX } from "@/features/leads/create/validation/leadSchema";
import { useLeadFormValidation } from "@/features/leads/create/validation/useLeadFormValidation";

type Props = LeadFormProps;

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

const { state, disabled, initialValues } = toRefs(props);

const vm = proxyRefs(
  useLeadFormValidation({
    state,
    disabled,
    initialValues,
    onSubmit: (vals) => emit("submit", vals),
  })
);
defineExpose({
  applyServerErrors: vm.applyServerErrors,
});
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

    <div
      v-if="vm.isSuccess"
      class="mb-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-900"
      data-testid="lead-success"
      role="status"
    >
      {{ props.successMessage || $t("pages:leads.success") }}
    </div>

    <div
      v-if="vm.isError"
      class="mb-4 rounded-xl border border-rose-200 bg-rose-50 px-x-4 py-3 text-sm text-rose-900"
      data-testid="lead-error"
      role="alert"
    >
      {{ props.errorMessage || $t("pages:leads.error") }}
    </div>

    <div
      v-if="vm.showFormError"
      class="mb-4 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900"
      data-testid="lead-validation-banner"
      role="alert"
    >
      {{ $t("errors:validation.lead.form.fixErrors") }}
    </div>

    <form class="space-y-4" @submit.prevent="vm.submit" data-testid="lead-form-form" novalidate>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2">

        <!-- Full name -->
        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-fullName">
            {{ $t("pages:leads.fields.fullName.label") }}
          </label>
          <input
            id="lead-fullName"
            v-model.trim="vm.values.fullName"
            type="text"
            autocomplete="name"
            :placeholder="$t('pages:leads.fields.fullName.placeholder')"
            class="w-full rounded-xl border bg-white px-3 py-2 text-slate-900 outline-none transition focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:bg-slate-50"
            :class="[
              vm.touched.fullName && vm.errors.fullName ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-100' : 'border-slate-200 focus:border-slate-300 focus:ring-slate-200'
            ]"
            :disabled="vm.isFormDisabled"
            data-testid="lead-fullName"
            @blur="vm.onBlur('fullName')"
            @input="vm.onInput('fullName')"
          />
          <p
            v-if="vm.touched.fullName && vm.errors.fullName"
            class="text-xs text-rose-700"
            data-testid="lead-fullName-error"
          >
            {{ $t(vm.errors.fullName.key, vm.errors.fullName.params) }}
          </p>
        </div>

        <!-- Email -->
        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-email">
            {{ $t("pages:leads.fields.email.label") }}
          </label>
          <input
            id="lead-email"
            v-model.trim="vm.values.email"
            type="email"
            autocomplete="email"
            inputmode="email"
            :placeholder="$t('pages:leads.fields.email.placeholder')"
            class="w-full rounded-xl border bg-white px-3 py-2 text-slate-900 outline-none transition focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:bg-slate-50"
            :class="[
              vm.touched.email && vm.errors.email ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-100' : 'border-slate-200 focus:border-slate-300 focus:ring-slate-200'
            ]"
            :disabled="vm.isFormDisabled"
            data-testid="lead-email"
            @blur="vm.onBlur('email')"
            @input="vm.onInput('email')"
          />
          <p v-if="vm.touched.email && vm.errors.email" class="text-xs text-rose-700" data-testid="lead-email-error">
            {{ $t(vm.errors.email.key, vm.errors.email.params) }}
          </p>
        </div>

        <!-- Phone -->
        <div class="space-y-1">
          <label class="text-sm font-medium text-slate-900" for="lead-phoneNumber">
            {{ $t("pages:leads.fields.phoneNumber.label") }}
          </label>
          <input
            id="lead-phoneNumber"
            v-model.trim="vm.values.phoneNumber"
            type="tel"
            autocomplete="tel"
            inputmode="tel"
            :placeholder="$t('pages:leads.fields.phoneNumber.placeholder')"
            class="w-full rounded-xl border bg-white px-3 py-2 text-slate-900 outline-none transition focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:bg-slate-50"
            :class="[
              vm.touched.phoneNumber && vm.errors.phoneNumber ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-100' : 'border-slate-200 focus:border-slate-300 focus:ring-slate-200'
            ]"
            :disabled="vm.isFormDisabled"
            data-testid="lead-phoneNumber"
            @blur="vm.onBlur('phoneNumber')"
            @input="vm.onInput('phoneNumber')"
          />
          <p
            v-if="vm.touched.phoneNumber && vm.errors.phoneNumber"
            class="text-xs text-rose-700"
            data-testid="lead-phoneNumber-error"
          >
            {{ $t(vm.errors.phoneNumber.key, vm.errors.phoneNumber.params) }}
          </p>
        </div>

        <!-- Message -->
        <div class="space-y-1 md:col-span-2">
          <label class="text-sm font-medium text-slate-900" for="lead-message">
            {{ $t("pages:leads.fields.message.label") }}
          </label>
          <textarea
            id="lead-message"
            v-model.trim="vm.values.message"
            rows="4"
            :placeholder="$t('pages:leads.fields.message.placeholder')"
            class="w-full resize-none rounded-xl border bg-white px-3 py-2 text-slate-900 outline-none transition focus:outline-none focus:ring-2 disabled:cursor-not-allowed disabled:bg-slate-50"
            :class="[
              vm.touched.message && vm.errors.message ? 'border-rose-300 focus:border-rose-400 focus:ring-rose-100' : 'border-slate-200 focus:border-slate-300 focus:ring-slate-200'
            ]"
            :disabled="vm.isFormDisabled"
            data-testid="lead-message"
            @blur="vm.onBlur('message')"
            @input="vm.onInput('message')"
          />
          <p v-if="vm.touched.message && vm.errors.message" class="text-xs text-rose-700" data-testid="lead-message-error">
            {{ $t(vm.errors.message.key, vm.errors.message.params) }}
          </p>

          <p class="text-xs text-slate-500" data-testid="lead-message-hint">
            {{ vm.values.message.trim().length }} / {{ MESSAGE_MAX }}
          </p>
        </div>
      </div>

      <div class="flex items-center justify-between gap-3 pt-1">
        <p class="text-xs text-slate-500" data-testid="lead-form-note">
          {{ $t("pages:leads.note") }}
        </p>

        <button
          type="submit"
          class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-slate-300 disabled:cursor-not-allowed disabled:bg-slate-400"
          :disabled="vm.isSubmitDisabled"
          data-testid="lead-submit"
        >
          <span v-if="vm.isLoading" class="inline-flex items-center gap-2" data-testid="lead-submit-loading">
            <span class="h-4 w-4 animate-spin rounded-full border-2 border-white/30 border-t-white" aria-hidden="true" />
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
