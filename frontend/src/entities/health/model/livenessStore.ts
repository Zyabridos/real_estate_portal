import { defineStore } from "pinia";
import type { UIState } from "@/shared/types/ui";
import type { ApiError } from "@/shared/types/errors";
import routes from "@/shared/routes.ts"

let activeController: AbortController | null = null;

// TODO: use common UIState evnt?
type CheckState = "idle" | "loading" | "ok" | "fail";

type UiHealthCheck = {
  key: "readiness" | "liveness";
  url: string;
  state: CheckState;
  status?: number;
  body?: string;
  error?: string;
};

type HealthState = {
  status: UIState;
  error: ApiError | null;

  lastCheckedAtIso: string | null;
  lastDurationMs: number | null;

  lastHttpStatus: number | null;
  lastBody: string | null;

  reqId: number;
};

const CHECK: Pick<UiHealthCheck, "key" | "url"> = {
  key: "liveness",
  url: routes.api.health.liveness(),
};

function mapUiStateToCheckState(s: UIState): CheckState {
  if (s === "loading") return "loading";
  if (s === "success") return "ok";
  if (s === "error") return "fail";
  return "idle";
}

function formatError(e: unknown): string {
  const err = e as Partial<ApiError> | undefined;

  if (e instanceof Error) return e.message;
  return "Unknown error";
}

async function safeReadText(res: Response): Promise<string> {
  try {
    return await res.text();
  } catch {
    return "";
  }
}

async function ping(url: string): Promise<{ status: number; body: string }> {
  if (activeController) activeController.abort();
  activeController = new AbortController();

  const res = await fetch(url, { signal: activeController.signal });
  const body = await safeReadText(res);

  return { status: res.status, body };
}

export const useLivenessStore = defineStore("liveness", {
  state: (): HealthState => ({
    status: "idle",
    error: null,

    lastCheckedAtIso: null,
    lastDurationMs: null,

    lastHttpStatus: null,
    lastBody: null,

    reqId: 0,
  }),

  getters: {
    lastCheckedAt: (s): Date | null => (s.lastCheckedAtIso ? new Date(s.lastCheckedAtIso) : null),

    uiCheck: (s): UiHealthCheck => ({
      key: CHECK.key,
      url: CHECK.url,
      state: mapUiStateToCheckState(s.status),
      status: s.lastHttpStatus ?? undefined,
      body: s.lastBody ?? undefined,
      error: s.error ? formatError(s.error) : undefined,
    }),
  },

  actions: {
    async check(): Promise<void> {
      const myReqId = ++this.reqId;

      this.status = "loading";
      this.error = null;

      const started = performance.now();

      try {
        const { status, body } = await ping(CHECK.url);

        if (myReqId !== this.reqId) return;

        this.lastHttpStatus = status;
        this.lastBody = body;
        this.lastDurationMs = Math.round(performance.now() - started);
        this.lastCheckedAtIso = new Date().toISOString();

        this.status = "success";
      } catch (e) {
        if (myReqId !== this.reqId) return;

        this.lastDurationMs = Math.round(performance.now() - started);
        this.lastCheckedAtIso = new Date().toISOString();

        this.status = "error";
        this.error =
          (e as ApiError) ?? {
            status: 0,
            title: "Unknown error",
          };
      }
    },

    reset(): void {
      this.status = "idle";
      this.error = null;

      this.lastCheckedAtIso = null;
      this.lastDurationMs = null;

      this.lastHttpStatus = null;
      this.lastBody = null;

      this.reqId = 0;
    },
  },
});
