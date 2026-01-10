<script setup>
import { ref, onMounted } from "vue";

const status = ref("loading...");

onMounted(async () => {
  try {
    // TODO: use axios?
    // TODO: remove hardcoded routes
    const res = await fetch("/api/health");
    const text = await res.text();
    status.value = `HTTP ${res.status}: ${text || "(empty body)"}`;
  } catch (e) {
    status.value = `error: ${String(e)}`;
  }
});
</script>

<template>
  <div class="p-6">
    <h1 class="text-xl font-bold">Frontend OK</h1>
    <p class="mt-2">Backend health: {{ status }}</p>
  </div>
</template>
