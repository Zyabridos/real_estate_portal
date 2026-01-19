<script setup lang="ts">
import { computed } from "vue";

// TODO: remove hardcoded text
// TODO: move Navbar to a separate component?
type Props = {
  title?: string;
  isLoading?: boolean;
  errorMessage?: string | null;
};

const props = withDefaults(defineProps<Props>(), {
  title: "RealEstate Portal",
  isLoading: false,
  errorMessage: null,
});

const hasError = computed(() => Boolean(props.errorMessage));
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-900">
    <!-- Header -->
    <header class="sticky top-0 z-10 border-b border-slate-200 bg-white/90 backdrop-blur">
      <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
        <div class="flex items-center gap-3">
          <div class="h-9 w-9 rounded-xl bg-slate-900" />
          <div class="leading-tight">
            <div class="text-sm font-semibold">{{ title }}</div>
            <div class="text-xs text-slate-500">Vue 3 + Pinia</div>
          </div>
        </div>

        <!-- Nav -->
        <nav class="flex items-center gap-2 text-sm">
          <RouterLink
            to="/properties"
            class="rounded-lg px-3 py-2 text-slate-700 hover:bg-slate-100 hover:text-slate-900"
            active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
          >
            Properties
          </RouterLink>

          <RouterLink
            to="/brokers"
            class="rounded-lg px-3 py-2 text-slate-700 hover:bg-slate-100 hover:text-slate-900"
            active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
          >
            Brokers
          </RouterLink>

          <RouterLink
            to="/blog"
            class="rounded-lg px-3 py-2 text-slate-700 hover:bg-slate-100 hover:text-slate-900"
            active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
          >
            Blog
          </RouterLink>
        </nav>
      </div>

      <!-- Loading placeholder -->
      <div v-if="isLoading" class="h-1 w-full bg-slate-200">
        <div class="h-1 w-1/3 bg-slate-900 animate-pulse" />
      </div>

      <!-- Error placeholder -->
      <div v-if="hasError" class="border-t border-rose-200 bg-rose-50">
        <div class="mx-auto max-w-6xl px-4 py-3">
          <div class="text-sm font-medium text-rose-900">Something went wrong</div>
          <div class="mt-1 text-sm text-rose-800">
            {{ errorMessage }}
          </div>
        </div>
      </div>
    </header>

    <!-- Main -->
    <main class="mx-auto max-w-6xl px-4 py-6">
      <slot />
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-200 bg-white">
      <div class="mx-auto max-w-6xl px-4 py-4 text-xs text-slate-500">
        © {{ new Date().getFullYear() }} RealEstate Portal • Demo project
      </div>
    </footer>
  </div>
</template>
