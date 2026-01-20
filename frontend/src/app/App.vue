<script setup lang="ts">
import { ref, onMounted } from "vue";
import AppLayout from "@/shared/ui/layout/AppLayout.vue";

const status = ref("loading...");
const globalLoading = false; // evnt will be in Pinia
const globalErrorMessage: string | null = null; // evnt will be in Pinia
</script>

<template>
  <AppLayout :is-loading="globalLoading" :error-message="globalErrorMessage">
    <RouterView />
  </AppLayout>
</template>
