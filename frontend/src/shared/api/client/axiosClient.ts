import axios from "axios";
import type {
  AxiosInstance,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";
import type { HttpClientOptions } from "@/shared/api/client/types";
import { DEFAULT_HTTP_TIMEOUT_MS } from "@/defaults";
import { normalizeAxiosError } from "@/shared/api/errors/normalizeAxiosError";

export const createAxiosClient = (opts: HttpClientOptions): AxiosInstance => {
  const instance = axios.create({
    baseURL: opts.baseURL,
    timeout: opts.timeoutMs ?? DEFAULT_HTTP_TIMEOUT_MS,
    headers: {
      Accept: "application/json",
    },
  });

  // reserve place for auth later (hopefully)
  instance.interceptors.request.use((config: InternalAxiosRequestConfig) => {
    const token = opts.getAccessToken?.();
    if (token) {
      config.headers = config.headers ?? {};
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  // Normalize errors
  instance.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: unknown) => {
      const apiError = normalizeAxiosError(error);

      if (apiError.kind === "Unauthorized") {
        opts.onUnauthorized?.();
      }

      return Promise.reject(apiError);
    }
  );

  return instance;
}
