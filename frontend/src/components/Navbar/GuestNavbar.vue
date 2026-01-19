<script setup lang="ts">
const routerLinkClasses: string = "rounded-lg text-2xl px-3 py-2 text-slate-700 hover:bg-slate-100 hover:text-indigo-800";
// TODO: show links vis Map (?)
</script>
<template>
<nav class="flex h-16 items-center gap-2 text-sm">
  <RouterLink
    to="/properties"
    :class="routerLinkClasses"
    active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
  >
  Properties
  </RouterLink>

  <RouterLink
    to="/brokers"
    :class="routerLinkClasses"
    active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
  >
  Brokers
  </RouterLink>

  <RouterLink
    to="/blog"
    :class="routerLinkClasses"
    active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
  >
  Blog
  </RouterLink>
  </nav>
</template>
