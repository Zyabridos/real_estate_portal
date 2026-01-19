<script setup lang="ts">
import { ref, onMounted } from "vue";
import routes from "../routes";

const status = ref("loading...");

onMounted(async () => {
  try {
    const res = await fetch(routes.api.health());
    const text = await res.text();
    status.value = `HTTP ${res.status}: ${text || "(empty body)"}`;
  } catch (e) {
    status.value = `error: ${String(e)}`;
  }
});
</script>

<template>
  <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
    <h1 class="text-xl font-semibold">Frontend OK</h1>
    <p class="mt-2 text-slate-700">Backend health: {{ status }}</p>
  </div>
</template>
