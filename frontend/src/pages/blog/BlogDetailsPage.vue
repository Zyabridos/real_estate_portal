<script setup lang="ts">
import { computed } from "vue";
import type { ArticleListItemDto } from "@/shared/types/blog";

const props = defineProps<{ item: ArticleListItemDto }>();
const to = computed(() => `/blog/${props.item.slug}`);
</script>

<template>
  <article class="group relative rounded-xl border bg-white p-5 shadow-sm transition hover:shadow-md">
    <RouterLink
      :to="to"
      class="absolute inset-0 rounded-xl focus:outline-none focus-visible:ring-2 focus-visible:ring-offset-2"
      aria-label="Open article"
    />
    <div class="relative flex flex-col gap-3">
      <h2 class="text-lg font-semibold leading-snug">
        <span class="line-clamp-2 break-words">{{ item.title }}</span>
      </h2>

      <p v-if="item.excerpt" class="text-sm opacity-80">
        <span class="line-clamp-3 break-words">{{ item.excerpt }}</span>
      </p>

      <div class="flex flex-wrap items-center gap-x-3 gap-y-1 text-xs opacity-70">
        <span v-if="item.author?.name">By <span class="font-medium">{{ item.author.name }}</span></span>
        <span v-if="item.publishedAt">• {{ item.publishedAt }}</span>
      </div>

      <div v-if="item.categories?.length" class="flex flex-wrap gap-1.5">
        <span
          v-for="c in item.categories"
          :key="c.id"
          class="max-w-full truncate rounded-full bg-gray-100 px-2.5 py-1 text-xs"
        >
          {{ c.title }}
        </span>
      </div>
    </div>
  </article>
</template>

<style scoped>
.line-clamp-2,
.line-clamp-3 {
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.line-clamp-2 { -webkit-line-clamp: 2; }
.line-clamp-3 { -webkit-line-clamp: 3; }
</style>
