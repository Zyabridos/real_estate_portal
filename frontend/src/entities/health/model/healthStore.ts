import { defineStore } from "pinia";

import i18n from "@/shared/i18n";
import routes from "@/shared/routes";

import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

export type HealthCheckKey = "readiness" | "liveness";
export type HealthCheckState = "idle" | "loading" | "ok" | "fail";

export type HealthCheckResult = {
  key: HealthCheckKey;
  url: string;
  state: HealthCheckState;
  status?: number;
  body?: string;
  error?: string;
};

let activeController: AbortController | null = null;

const useHealthStore = defineStore("health", {
  state: () => ({
    checks: [
      { key: "readiness", url: routes.api.health.readiness(), state: "idle" },
      { key: "liveness", url: routes.api.health.liveness(), state: "idle" },
    ] as HealthCheckResult[],

    lastCheckedAt: null as Date | null,
    lastDurationMs: null as number | null,

    listStatus: "idle" as UIState,
    listError: null as ApiError | null,
  }),

  getters: {
    isLoading: (state) => state.listStatus === "loading",

    overall: (state): HealthCheckState => {
      const list = state.checks;
      if (list.some((c) => c.state === "loading")) return "loading";
      if (list.every((c) => c.state === "ok")) return "ok";
      if (list.some((c) => c.state === "fail")) return "fail";
      return "idle";
    },
  },

  actions: {
    async fetchHealth() {
      // cancel previous request (same pattern as brokersStore)
      activeController?.abort();
      const controller = new AbortController();
      activeController = controller;

      this.listStatus = "loading";
      this.listError = null;

      const startedAt = performance.now();

      // optimistic UI
      this.checks = this.checks.map((c) => ({
        ...c,
        state: "loading",
        status: undefined,
        body: undefined,
        error: undefined,
      }));

      try {
        const results = await Promise.all(
          this.checks.map(async (c) => {
            try {
              const res = await fetch(c.url, { method: "GET", signal: controller.signal });
              const text = await res.text();

              return {
                ...c,
                status: res.status,
                body: text || i18n.t("pages:home.emptyBody"),
                state: res.ok ? "ok" : "fail",
              } as HealthCheckResult;
            } catch (e) {
              return {
                ...c,
                state: "fail",
                error: String(e),
              } as HealthCheckResult;
            }
          }),
        );

        // if another request has started, ignore this one
        if (activeController !== controller) return;

        this.checks = results;
        this.lastCheckedAt = new Date();
        this.lastDurationMs = Math.round(performance.now() - startedAt);

        this.listStatus = this.overall === "ok" ? "success" : "error";
      } catch (err) {
        if (activeController !== controller) return;

        this.checks = this.checks.map((c) => ({ ...c, state: "fail", error: String(err) }));
        this.listStatus = "error";
        this.listError = err as ApiError;
      }
    },

    async refresh() {
      return this.fetchHealth();
    },

    cancelRequest() {
      activeController?.abort();
      activeController = null;
    },

    reset() {
      this.cancelRequest();

      this.checks = [
        { key: "readiness", url: routes.api.health.readiness(), state: "idle" },
        { key: "liveness", url: routes.api.health.liveness(), state: "idle" },
      ];

      this.lastCheckedAt = null;
      this.lastDurationMs = null;

      this.listStatus = "idle";
      this.listError = null;
    },
  },
});

export default useHealthStore;
