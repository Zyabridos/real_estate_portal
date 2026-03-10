<script setup lang="ts">
import ModalDialog from "@/shared/ui/modal/ModalDialog.vue";

type Props = {
  isOpen: boolean;
  leadId: string | null;
  fullName: string | null;
  message: string | null;
  isLoading: boolean;
  errorMessage: string | null;
  onClose: () => void;
};

defineProps<Props>();

const titleId = "lead-message-title";
const descId = "lead-message-desc";
</script>

<template>
  <ModalDialog
    :isOpen="isOpen"
    :onClose="onClose"
    :titleId="titleId"
    :descriptionId="descId"
    testId="lead-message-modal"
  >
    <template #title>
      <h2 :id="titleId" class="text-lg font-semibold text-slate-900" data-testid="lead-message-title">
        {{ $t("leads:list.modal.title") }}
      </h2>
    </template>

    <template #description>
      <p :id="descId" class="mt-1 text-sm text-slate-600" data-testid="lead-message-subtitle">
        {{
          fullName
            ? $t("leads:list.modal.subtitleFrom", { name: fullName })
            : $t("leads:list.modal.subtitleFallback")
        }}
        <span v-if="leadId" class="ml-2 font-mono text-[11px] text-slate-500">{{ leadId }}</span>
      </p>
    </template>

    <div class="space-y-3" data-testid="lead-message-body">
      <div
        v-if="isLoading"
        class="rounded-xl border border-slate-200 bg-white p-4"
        role="status"
        data-testid="lead-message-loading"
      >
        <div class="flex items-center gap-3">
          <span
            class="h-4 w-4 animate-spin rounded-full border-2 border-slate-300 border-t-slate-900"
            aria-hidden="true"
          />
          <span class="text-sm text-slate-700">{{ $t("leads:list.modal.loading") }}</span>
        </div>
      </div>

      <div
        v-else-if="errorMessage"
        class="rounded-xl border border-rose-200 bg-rose-50 p-4 text-rose-900"
        role="alert"
        data-testid="lead-message-error"
      >
        <p class="text-sm font-medium">{{ errorMessage }}</p>
      </div>

      <div
        v-else
        class="rounded-xl border border-slate-200 bg-slate-50 p-4"
        data-testid="lead-message-content"
      >
        <p class="whitespace-pre-wrap break-words text-sm text-slate-900">
          {{ message && message.trim().length ? message : $t("leads:list.modal.empty") }}
        </p>
      </div>
    </div>

    <template #footer>
      <button
        type="button"
        class="rounded-xl border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-300"
        data-testid="lead-message-close"
        @click="onClose"
      >
        {{ $t("leads:list.modal.close") }}
      </button>
    </template>
  </ModalDialog>
</template>
