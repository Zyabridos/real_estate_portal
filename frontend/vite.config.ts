import { defineConfig, loadEnv, mergeConfig } from "vite";
import { createViteSharedConfig } from "./vite.shared";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, __dirname);

  const port = Number(env.VITE_PORT) || 3000;
  const apiPrefix = env.VITE_API_PREFIX || "/api";

  const apiTarget =
    env.VITE_API_PROXY_TARGET ||
    (process.env.DOCKER ? "http://backend:5000" : "http://localhost:5000");

  return mergeConfig(createViteSharedConfig(), {
    server: {
      port,
      host: true,
      allowedHosts: ["realestateproject.casa", "www.realestateproject.casa"],
      proxy: {
        [apiPrefix]: {
          target: apiTarget,
          changeOrigin: true,
        },
      },
    },
  });
});
