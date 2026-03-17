<script setup lang="ts">
import { computed } from "vue";
import { RouterLink, type RouteLocationRaw } from "vue-router";

import i18n from "@/shared/i18n";

type Variant = "invalidId" | "notFound";
type EntityType = "agency" | "property" | "broker" | "lead";

const props = defineProps<{
  entity: EntityType;
  variant: Variant;
  requestedId: string;
  backTo: RouteLocationRaw;
  onRefresh?: () => void;
}>();

const sharedBaseKey = "errors:common.entityDetails";
const contentBaseKey = computed(
  () => `${sharedBaseKey}.${props.variant}.${props.entity}`,
);

const badge = computed(() => i18n.t(`${contentBaseKey.value}.badge`));
const title = computed(() => i18n.t(`${contentBaseKey.value}.title`));
const description = computed(() => i18n.t(`${contentBaseKey.value}.description`));

const requestedIdLabel = computed(() => i18n.t(`${sharedBaseKey}.requestedIdLabel`));

const requestedIdValue = computed(() => {
  const value = props.requestedId?.trim();

  return value ? value : i18n.t(`${sharedBaseKey}.requestedIdEmpty`);
});

const tipsTitle = computed(() => i18n.t(`${sharedBaseKey}.whatYouCanDoTitle`));
const tip1 = computed(() => i18n.t(`${sharedBaseKey}.tip1`));
const tip2 = computed(() => i18n.t(`${sharedBaseKey}.tip2`));
const tip3 = computed(() => i18n.t(`${sharedBaseKey}.tip3`));
const note = computed(() => i18n.t(`${sharedBaseKey}.note`));

const showRefresh = computed(() => typeof props.onRefresh === "function");
</script>

<template>
  <section
    class="w-full"
    data-testid="error-state"
    role="alert"
    aria-live="assertive"
    :aria-label="$t('errors:common.entityDetails.ariaLabel')"
  >
    <div class="rounded-3xl border border-slate-200 bg-white p-8 shadow-sm">
      <div class="flex flex-col gap-8 md:flex-row md:items-start md:justify-between">
        <div class="max-w-2xl">
          <div
            class="inline-flex items-center gap-2 rounded-full border border-slate-200 bg-slate-50 px-3 py-1 text-xs text-slate-600"
            role="status"
            aria-live="polite"
          >
            <span class="h-2 w-2 rounded-full bg-slate-900" aria-hidden="true" />
            {{ badge }}
          </div>

          <h2
            class="mt-4 text-2xl font-semibold tracking-tight text-slate-900"
            data-testid="error-title"
          >
            {{ title }}
          </h2>

          <p class="mt-3 text-slate-600" data-testid="error-message">
            {{ description }}
          </p>

          <div class="mt-4 rounded-2xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-700">
            <div class="text-xs font-medium text-slate-500">
              {{ requestedIdLabel }}
            </div>

            <div class="mt-1 break-all font-mono text-xs text-slate-900 md:text-sm">
              {{ requestedIdValue }}
            </div>
          </div>

          <div class="mt-6 flex flex-wrap gap-3">
            <RouterLink
              :to="props.backTo"
              class="inline-flex items-center justify-center rounded-xl bg-slate-900 px-4 py-2.5 text-sm font-medium text-white hover:bg-slate-800"
              :aria-label="$t('common:actions.backToListAria')"
            >
              {{ $t("common:actions.backToList") }}
            </RouterLink>

            <button
              v-if="showRefresh"
              type="button"
              class="inline-flex items-center justify-center rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm font-medium text-slate-900 hover:bg-slate-50"
              data-testid="retry-button"
              :aria-label="$t('common:actions.refreshAria')"
              @click="props.onRefresh?.()"
            >
              {{ $t("common:actions.refresh") }}
            </button>
          </div>
        </div>

        <aside
          class="w-full max-w-md"
          :aria-label="$t('errors:common.entityDetails.asideAriaLabel')"
        >
          <div class="rounded-2xl border border-slate-200 bg-slate-50 p-6">
            <h3 class="text-sm font-semibold text-slate-900">
              {{ tipsTitle }}
            </h3>

            <ul class="mt-3 space-y-3 text-sm text-slate-700">
              <li class="flex gap-3">
                <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" aria-hidden="true" />
                <span>{{ tip1 }}</span>
              </li>

              <li class="flex gap-3">
                <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" aria-hidden="true" />
                <span>{{ tip2 }}</span>
              </li>

              <li class="flex gap-3">
                <span class="mt-1 h-2 w-2 flex-none rounded-full bg-slate-900" aria-hidden="true" />
                <span>{{ tip3 }}</span>
              </li>
            </ul>

            <div class="mt-5 rounded-xl border border-slate-200 bg-white px-4 py-3 text-xs text-slate-600">
              {{ note }}
            </div>
          </div>
        </aside>
      </div>
    </div>
  </section>
</template>
