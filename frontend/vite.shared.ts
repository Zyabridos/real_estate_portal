import { fileURLToPath, URL } from "node:url";
import vue from "@vitejs/plugin-vue";
import type { UserConfig } from "vite";

export function createViteSharedConfig(): UserConfig {
  return {
    plugins: [vue()],
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
  };
}
