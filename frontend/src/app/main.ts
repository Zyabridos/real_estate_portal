import '@/assets/main.css'
import 'primeicons/primeicons.css'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import I18NextVue from 'i18next-vue'
import i18n, { i18nInitialized } from '@/shared/i18n'

import { createHttpClient } from "@/shared/api/client/createHttpClient"
import { env } from "@/shared/config/env"
import { DEFAULT_HTTP_TIMEOUT_MS } from "@/shared/config/defaults.ts"

import App from '@/app/App.vue'
import router from '@/app/router'

export const http = createHttpClient({
  baseURL: env.apiBaseUrl,
  timeoutMs: DEFAULT_HTTP_TIMEOUT_MS,
});

async function bootstrap() {
  await i18nInitialized;

  const app = createApp(App);

  app.use(I18NextVue, { i18next: i18n });
  app.use(createPinia());
  app.use(router);

  app.mount('#app');
}

bootstrap();
