<script setup lang="ts">
import routes from "@/shared/routes"
import LanguageSwitcher from "@/components/LanguageSwitcher.vue";

const routerLinkClasses: string = "rounded-lg text-2xl px-3 py-2 text-slate-700 hover:bg-slate-100 hover:text-indigo-800";
// TODO: show links vis Map (?)
</script>
<template>
  <nav class="flex h-16 items-center justify-between gap-2 text-sm">
    <div class="flex gap-4">
      <RouterLink
        :to="routes.app.properties()"
        :class="routerLinkClasses"
        active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
      >
        {{ $t('navigation:properties') }}
      </RouterLink>
    </div>

    <div class="flex gap-4">
      <RouterLink
        :to="routes.app.brokers()"
        :class="routerLinkClasses"
        active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
      >
        {{ $t('navigation:brokers') }}
      </RouterLink>
    </div>
    <div class="flex gap-4">
      <RouterLink
        :to="routes.app.blog()"
        :class="routerLinkClasses"
        active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
      >
        {{ $t('navigation:blog') }}
      </RouterLink>
    </div>
    <div class="flex items-center gap-3">
      <LanguageSwitcher />
    </div>
  </nav>
</template>
