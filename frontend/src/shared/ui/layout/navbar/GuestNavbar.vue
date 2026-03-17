<!-- GuestNavbar.vue -->
<script setup lang="ts">
import { RouterLink } from "vue-router";
import routes from "@/shared/routes";
import LanguageSwitcher from "@/shared/ui/LanguageSwitcher.vue";

const routerLinkClasses =
  "rounded-lg px-2.5 text-2xl text-slate-700 hover:bg-slate-100 hover:text-indigo-800";

const activeClass =
  "bg-slate-900 text-white hover:bg-slate-900 hover:text-white";

type NavItem = {
  i18nkey: string;
  to: string;
};

const navItems: NavItem[] = [
  { i18nkey: "navigation:properties", to: routes.app.properties.list() },
  { i18nkey: "navigation:brokers", to: routes.app.brokers.list() },
  { i18nkey: "navigation:agencies", to: routes.app.agencies.list() },
  { i18nkey: "navigation:leads", to: routes.app.leads.list() },
  { i18nkey: "navigation:blog", to: routes.app.blog.list() },
];
</script>

<template>
  <nav
    class="flex h-14 items-center justify-between gap-1.5 text-sm"
    :aria-label="$t('navigation:mainAriaLabel')"
  >
    <div class="flex gap-3">
      <RouterLink
        v-for="item in navItems"
        :key="item.to"
        :to="item.to"
        :class="routerLinkClasses"
        :exact-active-class="activeClass"
      >
        {{ $t(item.i18nkey) }}
      </RouterLink>
    </div>

    <div class="flex items-center gap-2">
      <LanguageSwitcher />
    </div>
  </nav>
</template>
