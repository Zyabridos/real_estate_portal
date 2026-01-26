import { createHttpClient } from '@/shared/api/client/createHttpClient';
import { env } from '@/shared/config/env';
import {DEFAULT_HTTP_TIMEOUT_MS} from "@/shared/config/defaults.ts";

export const http = createHttpClient({
  baseURL: env.apiBaseUrl,
  timeoutMs: DEFAULT_HTTP_TIMEOUT_MS,
});
