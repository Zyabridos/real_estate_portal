<script setup lang="ts">
import { computed, ref, onMounted } from "vue";
import i18n from "@/shared/i18n";
import routes from "@/shared/routes";

const status = ref<string>("");

const statusLabel = computed(() => i18n.t("pages:home.backendHealthLabel"));

onMounted(async () => {
  status.value = i18n.t("states:loading.health");

  try {
    const res = await fetch(routes.api.health());
    const text = await res.text();
    status.value = i18n.t("pages:home.healthStatus", {
      code: res.status,
      body: text || i18n.t("pages:home.emptyBody"),
    });
  } catch (e) {
    status.value = i18n.t("pages:home.healthError", { message: String(e) });
  }
});
</script>

<template>
  <section class="w-full" :aria-label="$t('pages:home.ariaLabel')">
    <div class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
      <h1 class="text-xl font-semibold">
        {{ $t('pages:home.title') }}
      </h1>

      <p class="mt-2 text-slate-700" role="status" aria-live="polite">
        <span class="font-medium">{{ statusLabel }}:</span>
        {{ status }}
      </p>
    </div>
  </section>
</template>
