<script setup lang="ts">
import { useRoute } from "vue-router";
import routes from "@/shared/routes";

const route = useRoute();

// TODO: remove hardcoded texts

const requestedPath =
  typeof route.fullPath === "string" && route.fullPath.length > 0
    ? route.fullPath
    : "(unknown)";
</script>

<template>
  <section class="w-full">
    <div class="mx-auto max-w-5xl px-6 py-12">
      <div class="rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
        <div class="flex flex-col gap-8 md:flex-row md:items-start md:justify-between">
          <div class="max-w-2xl">
            <div class="inline-flex items-center gap-2 rounded-full border border-slate-200 bg-slate-50 px-3 py-1 text-xs text-slate-600">
              <span class="h-2 w-2 rounded-full bg-slate-900" />
              404 • Page not found
            </div>

            <h1 class="mt-4 text-3xl font-semibold tracking-tight text-slate-900">
              We couldn’t find that page
            </h1>

            <p class="mt-3 text-slate-600">
              The page you’re looking for doesn’t exist or may have been moved.
              Check the URL or use the navigation below.
            </p>

            <div class="mt-4 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-700">
              <div class="text-xs font-medium text-slate-500">Requested path</div>
              <div class="mt-1 font-mono text-xs md:text-sm text-slate-900 break-all">
                {{ requestedPath }}
              </div>
            </div>

            <div class="mt-6 flex flex-wrap gap-3">
              <RouterLink
                :to="routes.app.home()"
                class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-slate-800"
              >
                Go to Home
              </RouterLink>
            </div>
          </div>

          <!-- Right side card -->
          <aside class="w-full max-w-md">
            <div class="rounded-2xl border border-slate-200 bg-slate-50 p-6">
              <h2 class="text-sm font-semibold text-slate-900">What you can do</h2>

              <ul class="mt-3 space-y-3 text-sm text-slate-700">
                <li class="flex gap-3">
                  <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" />
                  <span>Double-check the URL for typos.</span>
                </li>
                <li class="flex gap-3">
                  <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" />
                  <span>Use the main navigation at the top.</span>
                </li>
                <li class="flex gap-3">
                  <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" />
                  <span>Start from Properties and refine filters.</span>
                </li>
              </ul>

              <div class="mt-5 rounded-xl border border-slate-200 bg-white px-4 py-3 text-xs text-slate-600">
                If you believe this is a bug, you will be able to send us a report eventually.
              </div>
            </div>
          </aside>
        </div>
      </div>
    </div>
  </section>
</template>
