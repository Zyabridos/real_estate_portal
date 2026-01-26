<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { blogService, BlogServiceError } from "@/shared/api/blogService";
import type { ArticleListItemDto, CategoryDto } from "@/shared/types/blog";
import type { UIState } from "@/shared/types/ui";
import { ErrorState, EmptyState, LoadingState } from "@/shared/ui/states";
import BlogArticleCard from "@/pages/blog/components/BlogArticleCard.vue";

const route = useRoute();
const router = useRouter();

const state = ref<UIState>("idle");
const errorMessage = ref<string | null>(null);

const categoriesState = ref<UIState>("idle");
const categoriesError = ref<string | null>(null);

const articles = ref<ArticleListItemDto[]>([]);
const categories = ref<CategoryDto[]>([]);

const selectedCategory = ref<string | null>(null);

const isReady = computed(() => state.value === "success");
const isEmpty = computed(() => isReady.value && articles.value.length === 0);

function toErrorMessage(err: unknown): string {
  if (err instanceof BlogServiceError) return err.message;
  if (err instanceof Error) return err.message;
  return "Unknown error while loading blog data.";
}

async function loadCategories(): Promise<void> {
  categoriesState.value = "loading";
  categoriesError.value = null;

  try {
    const data = await blogService.getCategories();
    categories.value = data;
    categoriesState.value = "success";
  } catch (err) {
    categoriesError.value = toErrorMessage(err);
    categoriesState.value = "error";
  }
}

async function loadArticles(categorySlug?: string | null): Promise<void> {
  state.value = "loading";
  errorMessage.value = null;

  try {
    const data = await blogService.getArticles(categorySlug ?? null);
    articles.value = data;
    state.value = data.length === 0 ? "empty" : "success";
  } catch (err) {
    errorMessage.value = toErrorMessage(err);
    state.value = "error";
  }
}

function readCategoryFromUrl(): string | null {
  const raw = route.query.category;
  if (typeof raw !== "string" || raw.trim() === "") return null;
  return raw;
}

function writeCategoryToUrl(category: string | null): void {
  const nextQuery = { ...route.query };
  if (!category) {
    delete nextQuery.category;
  } else {
    nextQuery.category = category;
  }
  void router.replace({ query: nextQuery });
}

onMounted(async () => {
  selectedCategory.value = readCategoryFromUrl();
  await Promise.all([loadCategories(), loadArticles(selectedCategory.value)]);
});

watch(
  () => route.query.category,
  (v) => {
    const fromUrl = typeof v === "string" && v.trim() !== "" ? v : null;

    // sync select with URL
    if (fromUrl !== selectedCategory.value) {
      selectedCategory.value = fromUrl;
    }
  }
);

watch(
  () => selectedCategory.value,
  (v) => {
    void loadArticles(v);

    // keep URL in sync
    const currentUrl = readCategoryFromUrl();
    if (v !== currentUrl) {
      writeCategoryToUrl(v);
    }
  }
);
</script>

<template>
  <main class="mx-auto w-full max-w-5xl px-4 py-6">
    <header class="mb-6 flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
      <div>
        <h1 class="text-2xl font-semibold">Blog</h1>
        <p class="mt-1 text-sm opacity-80">Articles from Sanity CMS</p>
      </div>

      <div class="w-full md:w-72">
        <label class="mb-1 block text-sm font-medium">Category</label>

        <select
          class="w-full rounded-lg border px-3 py-2 text-sm"
          :disabled="categoriesState !== 'success'"
          v-model="selectedCategory"
          data-testid="blog-category-select"
        >
          <option :value="null">All</option>
          <option v-for="c in categories" :key="c.id" :value="c.slug">
            {{ c.title }}
          </option>
        </select>

        <p v-if="categoriesState === 'error'" class="mt-1 text-xs text-red-600">
          {{ categoriesError ?? "Failed to load categories." }}
        </p>
      </div>
    </header>

    <LoadingState v-if="state === 'loading'" />
    <ErrorState
      v-else-if="state === 'error'"
      :message="errorMessage ?? $t('errors:messages.unexpected')"
      :onRetry="() => loadArticles(selectedCategory)"
    />
    <EmptyState v-else-if="state === 'empty'" />

    <section v-else-if="isReady" class="grid gap-4 md:grid-cols-2" data-testid="blog-articles-list">
      <BlogArticleCard v-for="item in articles" :key="item.id" :item="item" />
    </section>

    <section v-else class="rounded-lg border p-4">
      <p class="text-sm">Preparing…</p>
    </section>
  </main>
</template>
