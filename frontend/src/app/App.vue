<script setup lang="ts">
import AppLayout from "@/shared/ui/layout/AppLayout.vue";
</script>

<template>
  <AppLayout :is-loading="false" :error-message="null">
    <RouterView />
  </AppLayout>
</template>
