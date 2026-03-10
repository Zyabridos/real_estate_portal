<script setup lang="ts">
const baseClasses =
  "rounded-md px-3 py-2 text-xl font-medium text-gray-300 hover:bg-gray-700 hover:text-white";
</script>

<template>
  <footer class="h-18 bg-gray-800 text-gray-300">
    <div class="mx-auto flex max-w-7xl items-center justify-between px-4 py-3">
      <p :class="baseClasses">{{ $t("navigation:footer.name") }}</p>

      <div class="flex gap-4">
        <a
          href="https://github.com/zyabridos"
          target="_blank"
          rel="noopener noreferrer"
          :class="`${baseClasses} flex items-center`"
        >
          {{ $t("navigation:footer.github") }}
          <i class="pi pi-github ml-2 text-2xl" aria-hidden="true" />
        </a>

        <a
          href="https://t.me/zyabridos"
          target="_blank"
          rel="noopener noreferrer"
          :class="`${baseClasses} flex items-center`"
        >
          {{ $t("navigation:footer.telegram") }}
          <i class="pi pi-send ml-2 text-2xl" aria-hidden="true" />
        </a>

        <a
          href="mailto:zyabrina95@gmail.com"
          :class="`${baseClasses} flex items-center`"
        >
          {{ $t("navigation:footer.email") }}
          <i class="pi pi-envelope ml-2 text-2xl" aria-hidden="true" />
        </a>
      </div>
    </div>
  </footer>
</template>
