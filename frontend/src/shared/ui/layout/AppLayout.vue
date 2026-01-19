<script setup lang="ts">
import { computed } from "vue";
import RealEstatePortal from "@/assets/RealEstateProtal.png";
import GuestNavbar from "@/components/Navbar/GuestNavbar.vue";

// TODO: remove hardcoded text
// TODO: move Navbar to a separate component?
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
  <div class="min-h-screen flex flex-col bg-slate-50 text-slate-900">
  <!-- Header -->
    <header class="sticky top-0 z-10 border-b border-slate-200 bg-white/90 backdrop-blur">
      <div class="flex w-full items-center justify-between px-6 py-3">
        <div class="flex items-center gap-3">
          <img
            :src="RealEstatePortal"
            alt="RealEstate Portal"
            class="h-20 w-40 rounded-xl object-contain"
          />
        </div>
        <GuestNavbar />
      </div>

      <!-- Loading placeholder -->
      <div v-if="isLoading" class="h-1 w-full bg-slate-200">
        <div class="h-1 w-1/3 bg-slate-900 animate-pulse" />
      </div>

      <!-- Error placeholder -->
      <div v-if="hasError" class="border-t border-rose-200 bg-rose-50">
        <div class="w-full px-6 py-3">
        <div class="text-sm font-medium text-rose-900">Something went wrong</div>
          <div class="mt-1 text-sm text-rose-800">
            {{ errorMessage }}
          </div>
        </div>
      </div>
    </header>

    <!-- Main -->
    <main class="w-full flex-1 px-6 py-6">
    <slot />
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-200 bg-white">
      <div class="w-full px-4 py-4 text-xs text-slate-500">
        © {{ new Date().getFullYear() }} RealEstate Portal • Demo project
      </div>
    </footer>
  </div>
</template>
