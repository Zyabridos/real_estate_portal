<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { blogService, BlogServiceError } from "@/shared/api/blogService";
import type { ArticleListItemDto } from "@/shared/types/blog";
import type { UIStatus } from "@/shared/types/ui";
import { ErrorState, EmptyState, LoadingState } from "@/shared/ui/states";
import BlogArticleCard from "@/pages/blog/BlogDetailsPage.vue";

const state = ref<UIStatus>("idle");
const errorMessage = ref<string | null>(null);
const articles = ref<ArticleListItemDto[]>([]);

const isReady = computed(() => state.value === "success");
const isEmpty = computed(() => isReady.value && articles.value.length === 0);

function toErrorMessage(err: unknown): string {
  if (err instanceof BlogServiceError) return err.message;
  if (err instanceof Error) return err.message;
  return "Unknown error while loading blog articles.";
}

async function loadArticles(): Promise<void> {
  state.value = "loading";
  errorMessage.value = null;

  try {
    const data = await blogService.getArticles(null);
    articles.value = data;

    state.value = data.length === 0 ? "empty" : "success";
  } catch (err) {
    errorMessage.value = toErrorMessage(err);
    state.value = "error";
  }
}

onMounted(() => {
  void loadArticles();
});
</script>

<template>
  <main class="mx-auto w-full max-w-5xl px-4 py-6">
    <header class="mb-6">
      <h1 class="text-2xl font-semibold">Blog</h1>
      <p class="mt-1 text-sm opacity-80">Articles from Sanity CMS</p>
    </header>

    <LoadingState v-if="state === 'loading'" />
    <ErrorState
      v-else-if="state === 'error'"
      :message="errorMessage ?? $t('errors:messages.unexpected')"
      :onRetry="loadArticles"
    />
    <EmptyState v-else-if="state === 'empty'" />

    <section v-else-if="isReady" class="grid gap-4 md:grid-cols-2">
      <BlogArticleCard
        v-for="item in articles"
        :key="item.id"
        :item="item"
      />
    </section>

    <section v-else class="rounded-lg border p-4">
      <p class="text-sm">Preparing…</p>
    </section>
  </main>
</template>
