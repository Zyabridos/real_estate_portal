import './assets/main.css'
import { createHttpClient } from "@/shared/api/client/createHttpClient.ts";
import { env } from "./env.ts";
import { DEFAULT_HTTP_TIMEOUT_MS} from "@/defaults.ts";
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from '@/App.vue'
import router from './router'

const app = createApp(App)
export const http = createHttpClient({
  baseURL: env.apiBaseUrl,
  timeoutMs: DEFAULT_HTTP_TIMEOUT_MS,
});

app.use(createPinia())
app.use(router)

app.mount('#app')
