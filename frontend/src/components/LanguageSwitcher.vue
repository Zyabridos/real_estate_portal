<script setup lang="ts">
import { computed } from 'vue';
import i18n, { supportedLanguages, type SupportedLanguage } from '@/shared/i18n';
import { useTranslation } from 'i18next-vue';

const { t } = useTranslation();

const languages = supportedLanguages;

const labels: Record<SupportedLanguage, string> = {
  en: 'EN',
  ru: 'RU',
  no: 'NO',
};

const currentLng = computed(() => (i18n.language as SupportedLanguage) || 'en');

const onChange = (e: Event) => {
  const value = (e.target as HTMLSelectElement).value as SupportedLanguage;
  i18n.changeLanguage(value);
}
</script>

<template>
  <label class="flex items-center gap-2 text-sm">
    <span class="sr-only">{{ t('common.language') }}</span>

    <select
      class="rounded-md border px-2 py-1 bg-transparent"
      :value="currentLng"
      @change="onChange"
    >
      <option v-for="lng in languages" :key="lng" :value="lng">
        {{ labels[lng] ?? lng.toUpperCase() }}
      </option>
    </select>
  </label>
</template>
