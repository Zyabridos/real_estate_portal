import { defineConfig, loadEnv, mergeConfig } from "vite";
import { createViteSharedConfig } from "./vite.shared";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd());

  const apiTarget =
    env.VITE_API_PROXY_TARGET ||
    (process.env.DOCKER ? "http://backend:5000" : "http://localhost:5000");

  return mergeConfig(createViteSharedConfig(), {
    server: {
      port: Number(env.VITE_PORT) || 3000,
      allowedHosts: ["realestateproject.casa", "www.realestateproject.casa"],
      host: true,
      proxy: {
        [env.VITE_API_PREFIX || "/api"]: {
          target: apiTarget,
          changeOrigin: true,
        },
      },
    },
  });
});
