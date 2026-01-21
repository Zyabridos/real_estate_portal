import { fileURLToPath, URL } from "node:url";
import { defineConfig, configDefaults } from "vitest/config";
import { mergeConfig } from "vite";
import { createViteSharedConfig } from "./vite.shared";

export default mergeConfig(
  createViteSharedConfig(),
  defineConfig({
    test: {
      environment: "jsdom",
      include: ["tests/unit/**/*.{test,spec}.ts"],
      exclude: [...configDefaults.exclude, "tests/e2e/**"],
      root: fileURLToPath(new URL("./", import.meta.url)),
    },
  })
);
