<script setup lang="ts">
import { computed } from "vue";

import RealEstatePortal from "@/assets/RealEstateProtal.png";
import routes from "../../routes.ts";

import GuestNavbar from "@/shared/ui/layout/navbar/GuestNavbar.vue";
import Footer from "@/shared/ui/layout/Footer.vue";

type Props = {
  isLoading?: boolean;
  errorMessage?: string | null;
};

const props = withDefaults(defineProps<Props>(), {
  isLoading: false,
  errorMessage: null,
});

const hasError = computed(() => Boolean(props.errorMessage));
</script>

<template>
  <div class="flex min-h-screen flex-col bg-slate-50 text-slate-900">
    <header
      class="sticky top-0 z-10 border-b border-slate-200 bg-white/90 backdrop-blur"
      :aria-label="$t('common:layout.header.ariaLabel')"
    >
      <div class="flex w-full items-center justify-between px-6 py-3">
        <div class="flex items-center gap-3">
          <RouterLink
            :to="routes.app.home()"
            class="rounded-xl focus:outline-none focus:ring-2 focus:ring-slate-300"
            :aria-label="$t('common:layout.header.logoLinkAria')"
          >
            <img
              :src="RealEstatePortal"
              :alt="$t('common:layout.header.logoAlt')"
              class="h-20 w-40 rounded-xl object-contain"
            />
          </RouterLink>
        </div>

        <GuestNavbar />
      </div>

      <div
        v-if="isLoading"
        class="h-1 w-full bg-slate-200"
        role="status"
        aria-live="polite"
        :aria-label="$t('common:layout.header.loadingAria')"
      >
        <div class="h-1 w-1/3 animate-pulse bg-slate-900" />
      </div>

      <div
        v-if="hasError"
        class="border-t border-rose-200 bg-rose-50"
        role="alert"
        aria-live="assertive"
      >
        <div class="w-full px-6 py-3">
          <div class="text-sm font-medium text-rose-900">
            {{ $t("common:layout.error.title") }}
          </div>
          <div class="mt-1 text-sm text-rose-800">
            {{ errorMessage }}
          </div>
        </div>
      </div>
    </header>

    <main class="w-full flex-1 px-6 py-6" :aria-label="$t('common:layout.main.ariaLabel')">
      <slot />
    </main>

    <Footer />
  </div>
</template>
