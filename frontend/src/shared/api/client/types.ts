import type { AxiosInstance, AxiosRequestConfig } from "axios";

export type RequestConfig = AxiosRequestConfig & {
  signal?: AbortSignal;
  timeout?: number;
};

export type HttpClientOptions = {
  baseURL: string;
  timeoutMs?: number;
  getAccessToken?: () => string | null;
  // evnt we will need this func for, for example, to redirect to login
  onUnauthorized?: () => void;
};

export type HttpClient = {
  get<TResponse>(url: string, config?: RequestConfig): Promise<TResponse>;
  post<TResponse, TBody = unknown>(
    url: string,
    body?: TBody,
    config?: RequestConfig
  ): Promise<TResponse>;
  put<TResponse, TBody = unknown>(
    url: string,
    body?: TBody,
    config?: RequestConfig
  ): Promise<TResponse>;
  patch<TResponse, TBody = unknown>(
    url: string,
    body?: TBody,
    config?: RequestConfig
  ): Promise<TResponse>;
  delete<TResponse>(url: string, config?: RequestConfig): Promise<TResponse>;
  raw: AxiosInstance; // Escape hatch for edge-cases (file upload, streaming, etc.)
};
