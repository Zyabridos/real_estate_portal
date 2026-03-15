import { defineStore } from "pinia";

import type { ApiError } from "@/shared/types/errors";
import type { UIState } from "@/shared/types/ui";

let activeController: AbortController | null = null;

type CheckState = "idle" | "loading" | "ok" | "fail";

type UiHealthCheck = {
  key: "readiness" | "liveness";
  url: string;
  state: CheckState;
  status?: number;
  body?: string;
  error?: string;
};

type ReadinessResponse = {
  status: number;
  body: string;
};

const CHECK_URL = "/api/health/readiness";

function mapUiStateToCheckState(state: UIState): CheckState {
  if (state === "loading") return "loading";
  if (state === "success") return "ok";
  if (state === "error") return "fail";
  return "idle";
}

function formatError(error: unknown): string {
  if (error instanceof Error) return error.message;
  return "Unknown error";
}

async function safeReadText(response: Response): Promise<string> {
  try {
    return await response.text();
  } catch {
    return "";
  }
}

async function ping(url: string, signal: AbortSignal): Promise<ReadinessResponse> {
  const response = await fetch(url, { signal });
  const body = await safeReadText(response);

  return {
    status: response.status,
    body,
  };
}

export const useReadinessStore = defineStore("readiness", {
  state: () => ({
    lastCheckedAtIso: null as string | null,
    lastDurationMs: null as number | null,

    lastHttpStatus: null as number | null,
    lastBody: null as string | null,

    checkStatus: "idle" as UIState,
    checkError: null as ApiError | null,
  }),

  getters: {
    lastCheckedAt: (state): Date | null =>
      state.lastCheckedAtIso ? new Date(state.lastCheckedAtIso) : null,

    uiCheck: (state): UiHealthCheck => ({
      key: "readiness",
      url: CHECK_URL,
      state: mapUiStateToCheckState(state.checkStatus),
      status: state.lastHttpStatus ?? undefined,
      body: state.lastBody ?? undefined,
      error: state.checkError ? formatError(state.checkError) : undefined,
    }),

    isLoading: (state): boolean => state.checkStatus === "loading",
  },

  actions: {
    async check(): Promise<void> {
      activeController?.abort();

      const controller = new AbortController();
      activeController = controller;

      this.checkStatus = "loading";
      this.checkError = null;

      const startedAt = performance.now();

      try {
        const result = await ping(CHECK_URL, controller.signal);

        if (activeController !== controller) return;

        this.lastHttpStatus = result.status;
        this.lastBody = result.body;
        this.lastDurationMs = Math.round(performance.now() - startedAt);
        this.lastCheckedAtIso = new Date().toISOString();
        this.checkStatus = "success";
      } catch (error) {
        if (activeController !== controller) return;

        this.lastDurationMs = Math.round(performance.now() - startedAt);
        this.lastCheckedAtIso = new Date().toISOString();
        this.checkStatus = "error";
        this.checkError = error as ApiError;
      }
    },

    async refresh(): Promise<void> {
      await this.check();
    },

    cancelCheckRequest(): void {
      activeController?.abort();
      activeController = null;
    },

    reset(): void {
      this.cancelCheckRequest();

      this.lastCheckedAtIso = null;
      this.lastDurationMs = null;

      this.lastHttpStatus = null;
      this.lastBody = null;

      this.checkStatus = "idle";
      this.checkError = null;
    },
  },
});
