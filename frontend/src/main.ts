import './assets/main.css'
import { createHttpClient } from "@/shared/api/client/createHttpClient.ts";
import { env } from "./env.ts";
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from '@/App.vue'
import router from './router'

const app = createApp(App)
export const http = createHttpClient({
  baseURL: env.apiBaseUrl,
  timeoutMs: 15_000,
});

app.use(createPinia())
app.use(router)

app.mount('#app')
