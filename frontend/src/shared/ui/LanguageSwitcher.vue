<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from "vue";
import i18n, { supportedLanguages, type SupportedLanguage } from "@/shared/i18n";

const isOpen = ref(false);

const labels: Record<SupportedLanguage, string> = {
  en: "EN",
  ru: "RU",
  no: "NO",
};

const normalize = (lng?: string): SupportedLanguage => {
  const code = (lng ?? "en").slice(0, 2).toLowerCase();
  return (code as SupportedLanguage) ?? "en";
};

// ✅ реактивный текущий язык
const current = ref<SupportedLanguage>(normalize(i18n.language));

const onLanguageChanged = (lng: string) => {
  current.value = normalize(lng);
};

onMounted(() => {
  i18n.on("languageChanged", onLanguageChanged);
});

onBeforeUnmount(() => {
  i18n.off("languageChanged", onLanguageChanged);
});

const currentLabel = computed(() => labels[current.value] ?? current.value.toUpperCase());

const toggle = () => (isOpen.value = !isOpen.value);
const close = () => (isOpen.value = false);

const changeLanguage = (lng: SupportedLanguage) => {
  void i18n.changeLanguage(lng);
  close();
};

// close on click outside + ESC
const onDocumentClick = (e: MouseEvent) => {
  const target = e.target as HTMLElement | null;
  if (!target) return;
  if (!target.closest("[data-language-switcher]")) close();
};

const onKeydown = (e: KeyboardEvent) => {
  if (e.key === "Escape") close();
};

onMounted(() => {
  document.addEventListener("click", onDocumentClick);
  document.addEventListener("keydown", onKeydown);
});

onBeforeUnmount(() => {
  document.removeEventListener("click", onDocumentClick);
  document.removeEventListener("keydown", onKeydown);
});

// выровнено под твой navbar-стиль
const triggerClasses =
  "inline-flex items-center gap-2 rounded-lg px-3 py-2 text-2xl text-slate-700 hover:bg-slate-100 hover:text-indigo-800";

const itemClasses =
  "block w-full px-4 py-2 text-left text-sm text-slate-700 hover:bg-slate-100 hover:text-indigo-800";
</script>

<template>
  <div class="relative inline-flex h-16 items-center" data-language-switcher>
    <button
      type="button"
      :class="triggerClasses"
      :aria-expanded="isOpen"
      aria-haspopup="menu"
      @click="toggle"
    >
      {{ currentLabel }}
      <i
        class="pi pi-chevron-down text-base transition-transform"
        :class="{ 'rotate-180': isOpen }"
        aria-hidden="true"
      />
    </button>

    <div
      v-if="isOpen"
      class="absolute right-0 top-full z-10 mt-2 w-28 origin-top-right rounded-lg bg-white shadow-lg ring-1 ring-black/10"
      role="menu"
      aria-label="Language"
    >
      <div class="py-1">
        <button
          v-for="lng in supportedLanguages"
          :key="lng"
          type="button"
          role="menuitem"
          :class="itemClasses"
          @click="changeLanguage(lng as SupportedLanguage)"
        >
          <span class="flex items-center justify-between">
            <span>{{ labels[lng as SupportedLanguage] ?? String(lng).toUpperCase() }}</span>
            <i
              v-if="normalize(String(lng)) === current"
              class="pi pi-check text-sm text-slate-500"
              aria-hidden="true"
            />
          </span>
        </button>
      </div>
    </div>
  </div>
</template>
