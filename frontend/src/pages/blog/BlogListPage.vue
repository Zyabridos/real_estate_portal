<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { blogService, BlogServiceError } from "@/shared/api/blogService";
import type { ArticleListItemDto } from "@/shared/types/blog";
import type { UIStatus } from "@/shared/types/ui";
import { ErrorState, EmptyState, LoadingState } from "@/shared/ui/states";

const state = ref<UIStatus>("idle");
const errorMessage = ref<string | null>(null);
const articles = ref<ArticleListItemDto[]>([]);

const isLoading = computed(() => state.value === "loading");
const isError = computed(() => state.value === "error");
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
    state.value = "success";
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
      <p class="mt-1 text-sm opacity-80">
        Articles from Sanity CMS
      </p>
    </header>

    <!-- States -->
    <LoadingState v-if="state === 'loading'" />
    <ErrorState
      v-else-if="state === 'error'"
      :message="errorMessage ?? $t('errors:messages.unexpected')"
      :onRetry="loadArticles"
    />
    <EmptyState v-else-if="state === 'empty'" />

    <!-- LIST -->
    <section v-else-if="isReady" class="space-y-3">
      <article
        v-for="item in articles"
        :key="item.id"
        class="rounded-lg border p-4 hover:bg-gray-50"
      >
        <RouterLink
          class="block"
          :to="`/blog/${item.slug}`"
        >
          <h2 class="text-lg font-semibold">
            {{ item.title }}
          </h2>

          <p v-if="item.excerpt" class="mt-2 text-sm opacity-80">
            {{ item.excerpt }}
          </p>

          <div class="mt-3 flex flex-wrap gap-2 text-xs opacity-70">
            <span v-if="item.author?.name">
              By {{ item.author.name }}
            </span>

            <span v-if="item.publishedAt">
              • {{ item.publishedAt }}
            </span>

            <span v-if="item.categories?.length">
              •
              <span class="inline-flex flex-wrap gap-1">
                <span
                  v-for="c in item.categories"
                  :key="c.id"
                  class="rounded bg-gray-200 px-2 py-0.5"
                >
                  {{ c.title }}
                </span>
              </span>
            </span>
          </div>
        </RouterLink>
      </article>
    </section>

    <!-- fallback just in case - for now -->
    <section v-else class="rounded-lg border p-4">
      <p class="text-sm">Preparing…</p>
    </section>
  </main>
</template>
