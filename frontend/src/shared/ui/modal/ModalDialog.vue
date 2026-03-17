<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from "vue";

type Props = {
  isOpen: boolean;
  titleId: string;
  descriptionId?: string;
  testId?: string;
  onClose: () => void;
};

const props = defineProps<Props>();

const dialogRef = ref<HTMLDivElement | null>(null);
let lastActive: Element | null = null;

function onKeydown(e: KeyboardEvent): void {
  if (!props.isOpen) return;

  if (e.key === "Escape") {
    e.preventDefault();
    props.onClose();
    return;
  }

  if (e.key !== "Tab") return;

  const root = dialogRef.value;
  if (!root) return;

  const focusables = root.querySelectorAll<HTMLElement>(
    'a[href],button:not([disabled]),textarea,input,select,[tabindex]:not([tabindex="-1"])'
  );

  if (!focusables.length) return;

  const first = focusables.item(0);
  const last = focusables.item(focusables.length - 1);

  if (!first || !last) return;

  const active = document.activeElement as HTMLElement | null;

  if (e.shiftKey) {
    if (!active || active === first) {
      e.preventDefault();
      last.focus();
    }
  } else {
    if (active === last) {
      e.preventDefault();
      first.focus();
    }
  }
}

function focusFirst(): void {
  const root = dialogRef.value;
  if (!root) return;

  const el = root.querySelector<HTMLElement>(
    'button[data-testid="modal-close"],button,[tabindex]:not([tabindex="-1"])'
  );

  el?.focus();
}

watch(
  () => props.isOpen,
  (open) => {
    if (open) {
      lastActive = document.activeElement;
      document.body.style.overflow = "hidden";
      queueMicrotask(() => focusFirst());
    } else {
      document.body.style.overflow = "";
      (lastActive as HTMLElement | null)?.focus?.();
      lastActive = null;
    }
  },
  { immediate: true }
);

onMounted(() => window.addEventListener("keydown", onKeydown));

onBeforeUnmount(() => {
  window.removeEventListener("keydown", onKeydown);
  document.body.style.overflow = "";
});
</script>

<template>
  <Teleport to="body">
    <div
      v-if="isOpen"
      class="fixed inset-0 z-50"
      :data-testid="testId ?? 'modal-root'"
      aria-live="polite"
    >
      <div
        class="absolute inset-0 bg-slate-900/50"
        :data-testid="`${testId ?? 'modal'}-backdrop`"
        @click="onClose()"
        aria-hidden="true"
      />

      <div class="relative flex min-h-full items-center justify-center p-4">
        <div
          ref="dialogRef"
          class="w-full max-w-2xl rounded-2xl border border-slate-200 bg-white shadow-xl outline-none"
          role="dialog"
          aria-modal="true"
          :aria-labelledby="titleId"
          :aria-describedby="descriptionId"
          :data-testid="`${testId ?? 'modal'}-dialog`"
          @click.stop
        >
          <header class="flex items-start justify-between gap-4 border-b border-slate-100 px-5 py-4">
            <div class="min-w-0">
              <slot name="title" />
              <slot name="description" />
            </div>

            <button
              type="button"
              class="rounded-xl border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-900 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-300"
              data-testid="modal-close"
              :aria-label="$t('common:actions.close')"
              @click="onClose()"
            >
              {{ $t("common:actions.close") }}
            </button>
          </header>

          <div class="px-5 py-4">
            <slot />
          </div>

          <footer class="flex justify-end gap-3 border-t border-slate-100 px-5 py-4">
            <slot name="footer">
              <button
                type="button"
                class="rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-slate-300"
                data-testid="modal-ok"
                @click="onClose()"
              >
                {{ $t("common:actions.ok") }}
              </button>
            </slot>
          </footer>
        </div>
      </div>
    </div>
  </Teleport>
</template>
