<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import { blogService, BlogServiceError } from "@/features/blog/api/blogService";
import type { ArticleDetailsDto } from "@/entities/blog/model/types";
import type { UIState } from "@/shared/types/ui";
import { ErrorState, LoadingState, EmptyState } from "@/shared/ui/states";

const route = useRoute();
const slug = computed(() => String(route.params.slug ?? "").trim());

const state = ref<UIState>("idle");
const errorMessage = ref<string | null>(null);
const article = ref<ArticleDetailsDto | null>(null);

function toErrorMessage(err: unknown): string {
  if (err instanceof BlogServiceError) return err.message;
  if (err instanceof Error) return err.message;
  return "Unknown error while loading article.";
}

// minimal render for now: Portable Text -> plain text
const contentText = computed(() => {
  const blocks = article.value?.content ?? [];
  if (!blocks.length) return "";

  return blocks
    .map((b) => (b.children ?? []).map((c) => c.text ?? "").join(""))
    .map((line) => line.trim())
    .filter(Boolean)
    .join("\n\n");
});

async function load(): Promise<void> {
  const s = slug.value;
  if (!s) {
    state.value = "empty";
    article.value = null;
    return;
  }

  state.value = "loading";
  errorMessage.value = null;

  try {
    const data = await blogService.getArticleBySlug(s);
    article.value = data;

    state.value = data ? "success" : "empty";
  } catch (err) {
    errorMessage.value = toErrorMessage(err);
    state.value = "error";
  }
}

onMounted(() => void load());

watch(
  () => slug.value,
  () => void load(),
);
</script>

<template>
  <main class="mx-auto w-full max-w-3xl px-4 py-6">
    <LoadingState
      v-if="state === 'loading'"
      testId="properties-loading"
      :title="$t('common:states.loading.genericTitle')"
      :subtitle="$t('properties:list.subtitle')"
    />

    <ErrorState
      v-else-if="state === 'error'"
      :message="errorMessage ?? $t('errors:messages.unexpected')"
      :onRetry="load"
    />

    <EmptyState v-else-if="state === 'empty'" />

    <section v-else class="space-y-6">
      <header>
        <h1 class="text-2xl font-semibold" data-testid="blog-details-title">
          {{ article?.title ?? `Article: ${slug}` }}
        </h1>

        <div class="mt-2 flex flex-wrap items-center gap-x-3 gap-y-1 text-xs opacity-70">
          <span v-if="article?.author?.name">
            By <span class="font-medium">{{ article.author.name }}</span>
          </span>
          <span v-if="article?.publishedAt">• {{ article.publishedAt }}</span>
        </div>

        <p v-if="article?.excerpt" class="mt-3 text-sm opacity-80">
          {{ article.excerpt }}
        </p>

        <div v-if="article?.categories?.length" class="mt-4 flex flex-wrap gap-1.5">
          <span
            v-for="c in article.categories"
            :key="c.id"
            class="max-w-full truncate rounded-full bg-gray-100 px-2.5 py-1 text-xs"
          >
            {{ c.title }}
          </span>
        </div>
      </header>

      <article class="prose max-w-none">
        <p v-if="!contentText" class="text-sm opacity-70">
          No content yet.
        </p>

        <pre
          v-else
          class="whitespace-pre-wrap break-words rounded-lg border bg-white p-4 text-sm leading-relaxed"
          data-testid="blog-details-content"
        >{{ contentText }}</pre>
      </article>
    </section>
  </main>
</template>
