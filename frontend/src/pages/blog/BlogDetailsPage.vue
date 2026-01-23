<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();
const slug = computed(() => String(route.params.slug ?? ""));
</script>

<template>
  <main class="mx-auto w-full max-w-3xl px-4 py-6">
    <header class="mb-6">
      <h1 class="text-2xl font-semibold" data-testid="blog-details-title">
        Article: {{ slug }}
      </h1>
      <p class="mt-1 text-sm opacity-80">Content will be added in PR12 (PortableText)</p>
    </header>
  </main>
</template>
