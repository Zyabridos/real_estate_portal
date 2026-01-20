import type { HttpClient, HttpClientOptions, RequestConfig } from "@/shared/api/client/types";
import { createAxiosClient } from "@/shared/api/client/axiosClient";

export const createHttpClient = (opts: HttpClientOptions): HttpClient => {
  const axiosInstance = createAxiosClient(opts);

  return {
    raw: axiosInstance,

    async get<TResponse>(url: string, config?: RequestConfig): Promise<TResponse> {
      const res = await axiosInstance.get<TResponse>(url, config);
      return res.data;
    },

    async post<TResponse, TBody = unknown>(url: string, body?: TBody, config?: RequestConfig) {
      const res = await axiosInstance.post<TResponse>(url, body, config);
      return res.data;
    },

    async put<TResponse, TBody = unknown>(url: string, body?: TBody, config?: RequestConfig) {
      const res = await axiosInstance.put<TResponse>(url, body, config);
      return res.data;
    },

    async patch<TResponse, TBody = unknown>(url: string, body?: TBody, config?: RequestConfig) {
      const res = await axiosInstance.patch<TResponse>(url, body, config);
      return res.data;
    },

    async delete<TResponse>(url: string, config?: RequestConfig) {
      const res = await axiosInstance.delete<TResponse>(url, config);
      return res.data;
    },
  };
}
