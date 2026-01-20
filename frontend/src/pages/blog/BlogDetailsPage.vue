
<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";

const route = useRoute();
const slug = computed(() => String(route.params.slug ?? ""));
</script>

<template>
  <section class="mx-auto w-full max-w-5xl px-4 py-8" data-testid="blog-details-page">
    <header class="mb-6">
      <div class="flex items-start justify-between gap-4">
        <div>
          <h1
            class="text-2xl font-semibold tracking-tight"
            data-testid="blog-details-title"
          >
            Article
          </h1>

          <p class="mt-2 text-sm text-muted-foreground" data-testid="blog-details-subtitle">
            Slug:
            <code
              class="ml-1 rounded-md border border-border bg-muted px-2 py-0.5 font-mono text-xs"
              data-testid="blog-details-slug"
            >
              {{ slug }}
            </code>
          </p>
        </div>

        <span
          class="inline-flex items-center rounded-full border border-border bg-background px-3 py-1 text-xs text-muted-foreground"
          data-testid="blog-details-badge"
        >
          scaffold
        </span>
      </div>
    </header>

    <main>
      <!-- Stable container for future Playwright assertions -->
      <article
        class="rounded-2xl border border-border bg-card p-5 shadow-sm"
        aria-label="Blog article"
        data-testid="blog-article"
      >
        <div class="flex flex-col gap-3">
          <h2 class="text-base font-semibold" data-testid="blog-article-placeholder-title">
            Coming soon
          </h2>

          <p class="text-sm text-muted-foreground" data-testid="blog-article-placeholder-text">
            Article content (Portable Text) will be rendered here in a future PR.
          </p>

          <div class="mt-2 flex flex-wrap gap-2" data-testid="blog-article-hints">
            <span
              class="inline-flex items-center rounded-lg bg-muted px-3 py-1 text-xs text-muted-foreground"
              data-testid="blog-article-hint-1"
            >
              Next PR: GROQ by slug
            </span>
            <span
              class="inline-flex items-center rounded-lg bg-muted px-3 py-1 text-xs text-muted-foreground"
              data-testid="blog-article-hint-2"
            >
              Next PR: Portable Text renderer
            </span>
          </div>
        </div>
      </article>
    </main>
  </section>
</template>
