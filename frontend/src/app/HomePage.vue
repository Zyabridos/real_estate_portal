<script setup lang="ts">
import { computed, onMounted } from "vue";
import i18n from "@/shared/i18n";
import { useReadinessStore } from "@/entities/health/model/readinessStore";
import { useLivenessStore } from "@/entities/health/model/livenessStore";

type CheckState = "idle" | "loading" | "ok" | "fail";

type UiHealthCheck = {
  key: "readiness" | "liveness";
  name: string;
  url: string;
  state: CheckState;
  status?: number;
  body?: string;
  error?: string;
};

type OverallKey = "loading" | "ok" | "fail" | "idle";

const readiness = useReadinessStore();
const liveness = useLivenessStore();

const checks = computed<UiHealthCheck[]>(() => [
  {
    ...readiness.uiCheck,
    name: i18n.t("home:readinessLabel"),
  },
  {
    ...liveness.uiCheck,
    name: i18n.t("home:livenessLabel"),
  },
]);

const overall = computed<CheckState>(() => {
  const states = checks.value.map((c) => c.state);

  if (states.some((s) => s === "loading")) return "loading";
  if (states.some((s) => s === "fail")) return "fail";
  if (states.length && states.every((s) => s === "ok")) return "ok";
  return "idle";
});

const overallKey = computed<OverallKey>(() => {
  if (overall.value === "loading") return "loading";
  if (overall.value === "ok") return "ok";
  if (overall.value === "fail") return "fail";
  return "idle";
});

const overallLabel = computed(() => {
  if (overallKey.value === "loading") return i18n.t("states:loading.health");
  if (overallKey.value === "ok") return i18n.t("home:healthOk");
  if (overallKey.value === "fail") return i18n.t("home:healthFail");
  return i18n.t("home:healthIdle");
});

const lastCheckedAt = computed<Date | null>(() => {
  const a = readiness.lastCheckedAt ?? null;
  const b = liveness.lastCheckedAt ?? null;

  if (!a && !b) return null;
  if (!a) return b;
  if (!b) return a;

  return a > b ? a : b;
});

const lastDurationMs = computed<number | null>(() => {
  const a = readiness.lastDurationMs ?? null;
  const b = liveness.lastDurationMs ?? null;

  if (a === null && b === null) return null;
  if (a === null) return b;
  if (b === null) return a;

  return Math.max(a, b);
});

function dotClass(state: CheckState): string {
  if (state === "loading") return "bg-slate-400 animate-pulse";
  if (state === "ok") return "bg-emerald-500";
  if (state === "fail") return "bg-rose-500";
  return "bg-slate-300";
}

function badgeClass(state: CheckState): string {
  if (state === "loading") return "border-slate-200 bg-slate-50 text-slate-700";
  if (state === "ok") return "border-emerald-200 bg-emerald-50 text-emerald-800";
  if (state === "fail") return "border-rose-200 bg-rose-50 text-rose-800";
  return "border-slate-200 bg-slate-50 text-slate-700";
}

function stateLabel(state: CheckState): string {
  if (state === "loading") return i18n.t("states:loading.health");
  if (state === "ok") return i18n.t("home:healthStatusOkShort");
  if (state === "fail") return i18n.t("home:healthStatusFailShort");
  return i18n.t("home:healthIdleShort");
}

const lastCheckedLabel = computed(() => {
  if (!lastCheckedAt.value) return null;

  const t = lastCheckedAt.value;
  const hh = String(t.getHours()).padStart(2, "0");
  const mm = String(t.getMinutes()).padStart(2, "0");
  const ss = String(t.getSeconds()).padStart(2, "0");

  return `${hh}:${mm}:${ss}`;
});

async function refreshHealth(): Promise<void> {
  await Promise.allSettled([readiness.check(), liveness.check()]);
}

onMounted(async () => {
  await Promise.allSettled([readiness.check(), liveness.check()]);
});
</script>

<template>
  <section class="relative w-full overflow-hidden" :aria-label="$t('home:ariaLabel')">
    <div class="absolute inset-0 -z-10 bg-gradient-to-b from-slate-50 via-white to-slate-50" />
    <div
      class="pointer-events-none absolute -top-24 left-1/2 -z-10 h-[420px] w-[420px] -translate-x-1/2 rounded-full
             bg-[radial-gradient(circle_at_center,rgba(99,102,241,0.18),rgba(255,255,255,0)_60%)] blur-2xl"
    />
    <div
      class="pointer-events-none absolute -bottom-28 right-[-120px] -z-10 h-[520px] w-[520px] rounded-full
             bg-[radial-gradient(circle_at_center,rgba(16,185,129,0.14),rgba(255,255,255,0)_60%)] blur-2xl"
    />

    <div class="grid gap-6 lg:grid-cols-12">
      <header
        class="rounded-3xl border border-slate-200/60 bg-white/70 p-8 shadow-sm backdrop-blur lg:col-span-7"
      >
        <div class="mb-6 h-1 w-full rounded-full bg-gradient-to-r from-indigo-500/70 via-sky-500/50 to-emerald-500/60" />

        <div class="flex flex-col gap-6">
          <div class="flex items-center gap-3">
            <div
              class="grid h-12 w-12 place-items-center rounded-2xl bg-gradient-to-br from-slate-900 to-slate-700 text-white shadow-sm"
              aria-hidden="true"
            >
              <span class="text-lg font-semibold">N</span>
            </div>

            <div class="min-w-0">
              <p class="text-sm text-slate-500">{{ $t("home:subtitle") }}</p>
              <h1 class="truncate text-2xl font-semibold text-slate-900">
                {{ $t("home:title") }}
              </h1>
            </div>
          </div>

          <div>
            <p class="text-sm font-semibold text-slate-500">{{ $t("home:aboutTitle") }}</p>
            <p class="mt-2 max-w-prose leading-relaxed text-slate-700">
              {{ $t("home:aboutText") }}
            </p>
          </div>

          <div class="grid gap-3 sm:grid-cols-2">
            <div class="rounded-3xl border border-slate-200/70 bg-slate-50/70 p-5">
              <p class="text-xs font-semibold text-slate-500">{{ $t("home:stackTitle") }}</p>
              <p class="mt-2 text-sm text-slate-800">
                {{ $t("home:stackValue") }}
              </p>
            </div>

            <div class="rounded-3xl border border-slate-200/70 bg-slate-50/70 p-5">
              <p class="text-xs font-semibold text-slate-500">{{ $t("home:focusTitle") }}</p>
              <p class="mt-2 text-sm text-slate-800">
                {{ $t("home:focusValue") }}
              </p>
            </div>
          </div>
        </div>
      </header>

      <aside
        class="rounded-3xl border border-slate-200/60 bg-white/70 p-8 shadow-sm backdrop-blur lg:col-span-5"
      >
        <div class="flex items-start justify-between gap-4">
          <div>
            <h2 class="text-lg font-semibold text-slate-900">{{ $t("home:healthTitle") }}</h2>
            <p class="mt-1 text-sm text-slate-600">{{ $t("home:healthHint") }}</p>

            <div class="mt-3 flex flex-wrap items-center gap-2">
              <span
                class="inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-semibold"
                role="status"
                aria-live="polite"
              >
                <span class="h-2.5 w-2.5 rounded-full" :class="dotClass(overall)" />
                <span>{{ overallLabel }}</span>
              </span>

              <span
                v-if="lastCheckedLabel"
                class="rounded-full border border-slate-200/70 bg-white/60 px-3 py-1 text-xs text-slate-600"
              >
                ⏱ {{ lastCheckedLabel }}<span v-if="lastDurationMs"> · {{ lastDurationMs }}ms</span>
              </span>
            </div>
          </div>

          <button
            type="button"
            class="rounded-2xl border border-slate-200 bg-white/70 px-4 py-2 text-sm font-semibold text-slate-900 shadow-sm
                   hover:bg-white focus:outline-none focus:ring-2 focus:ring-slate-200"
            @click="refreshHealth"
            :aria-label="$t('common:actions.refresh')"
          >
            {{ $t("common:actions.refresh") }}
          </button>
        </div>

        <div class="mt-6 space-y-3">
          <article
            v-for="c in checks"
            :key="c.key"
            class="rounded-3xl border border-slate-200/70 bg-white/60 p-5"
          >
            <div class="flex items-start justify-between gap-3">
              <div class="min-w-0">
                <div class="flex items-center gap-2">
                  <span class="h-2.5 w-2.5 rounded-full" :class="dotClass(c.state)" />
                  <p class="truncate text-sm font-semibold text-slate-900">{{ c.name }}</p>
                </div>
                <p class="mt-1 truncate text-xs text-slate-500">{{ c.url }}</p>
              </div>

              <div class="flex items-center gap-2">
                <span
                  class="inline-flex items-center rounded-full border px-3 py-1 text-xs font-semibold"
                  :class="badgeClass(c.state)"
                >
                  {{ stateLabel(c.state) }}
                </span>

                <span
                  v-if="c.state === 'ok' || c.state === 'fail'"
                  class="rounded-full border border-slate-200 bg-slate-50 px-2.5 py-1 text-xs font-semibold text-slate-700"
                >
                  {{ c.status ?? $t("home:statusUnknown") }}
                </span>
              </div>
            </div>

            <div class="mt-3 text-xs text-slate-700">
              <template v-if="c.state === 'ok' || c.state === 'fail'">
                <p class="font-medium">
                  {{ $t("home:httpStatus") }}: {{ c.status ?? $t("home:statusUnknown") }}
                </p>

                <p v-if="c.error" class="mt-1 text-rose-700">
                  {{ $t("home:healthError", { message: c.error }) }}
                </p>

                <details v-else class="mt-3">
                  <summary class="cursor-pointer select-none text-slate-600 hover:text-slate-900">
                    {{ $t("home:responseBody") }}
                  </summary>
                  <pre
                    class="mt-2 overflow-auto rounded-2xl bg-slate-950 p-4 text-[11px] leading-relaxed text-slate-100"
                  >{{ c.body || $t("home:emptyBody") }}</pre>
                </details>
              </template>

              <template v-else>
                <p class="text-slate-600">{{ $t("home:healthNotChecked") }}</p>
              </template>
            </div>
          </article>
        </div>
      </aside>
    </div>
  </section>
</template>
