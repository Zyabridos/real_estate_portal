import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());

  const apiTarget =
    env.VITE_API_PROXY_TARGET ||
    (process.env.DOCKER ? "http://backend:5000" : "http://localhost:5000");

  return {
    plugins: [vue()],
    server: {
      port: Number(env.VITE_PORT) || 3000,
      host: true,
      proxy: {
        "/api": {
          target: apiTarget,
          changeOrigin: true,
        },
      },
    },
  };
});
