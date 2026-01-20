<script setup lang="ts">
</script>

<template>
  <section class="mx-auto w-full max-w-5xl px-4 py-8" data-testid="blog-page">
    <header class="mb-6">
      <h1 class="text-2xl font-semibold tracking-tight" data-testid="blog-title">
        Blog
      </h1>
      <p class="mt-2 text-sm text-muted-foreground" data-testid="blog-subtitle">
        Editorial content will be loaded from Sanity CMS (scaffolded).
      </p>
    </header>

    <main>
      <!-- Stable container for future Playwright assertions -->
      <div
        class="grid gap-4"
        aria-label="Blog list"
        data-testid="blog-list"
      >
        <div
          class="rounded-2xl border border-border bg-card p-5 shadow-sm"
          data-testid="blog-empty-card"
        >
          <div class="flex items-start justify-between gap-4">
            <div>
              <h2 class="text-base font-semibold" data-testid="blog-empty-title">
                Coming soon
              </h2>
              <p class="mt-2 text-sm text-muted-foreground" data-testid="blog-empty-text">
                This page is ready for Sanity integration. No GROQ queries are executed in this PR.
              </p>
            </div>

            <span
              class="inline-flex items-center rounded-full border border-border bg-background px-3 py-1 text-xs text-muted-foreground"
              data-testid="blog-badge"
            >
              scaffold
            </span>
          </div>

          <div class="mt-4 flex flex-wrap gap-2" data-testid="blog-actions">
            <span
              class="inline-flex items-center rounded-lg bg-muted px-3 py-1 text-xs text-muted-foreground"
              data-testid="blog-action-hint-1"
            >
              Next PR: GROQ list query
            </span>
            <span
              class="inline-flex items-center rounded-lg bg-muted px-3 py-1 text-xs text-muted-foreground"
              data-testid="blog-action-hint-2"
            >
              Next PR: category filter
            </span>
          </div>
        </div>
      </div>
    </main>
  </section>
</template>
