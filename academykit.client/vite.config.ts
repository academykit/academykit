import { sentryVitePlugin } from "@sentry/vite-plugin";
import react from "@vitejs/plugin-react";
import child_process from "node:child_process";
import fs, { readFileSync } from "node:fs";
import path from "node:path";
import { env } from "node:process";
import { defineConfig } from "vite";
import eslintPlugin from "vite-plugin-eslint";

const dir = __dirname;

const baseFolder =
  env.APP_DATA !== undefined && env.APP_DATA !== ""
    ? `${env.APP_DATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

fs.mkdirSync(baseFolder, { recursive: true });
const certificateName = "academykit.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    0 !==
    child_process.spawnSync(
      "dotnet",
      [
        "dev-certs",
        "https",
        "--export-path",
        certFilePath,
        "--format",
        "Pem",
        "--no-password",
      ],
      { stdio: "inherit" }
    ).status
  ) {
    throw new Error("Could not create certificate.");
  }
}

export default defineConfig({
  plugins: [
    react(),
    eslintPlugin({ fix: true }),
    sentryVitePlugin({
      org: "academy-kit",
      project: "academykit-web",
    }),
  ],
  server: {
    https: {
      key: readFileSync(keyFilePath),
      cert: readFileSync(certFilePath),
    },
    host: true,
    port: 44414,
    strictPort: true,
    proxy: {
      "/api": {
        target: "https://127.0.0.1:7042",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/.*\/api/, "/api"),
      },
    },
  },
  resolve: {
    alias: {
      // "@": path.resolve(__dirname, ".src/"),
      "@pages": path.resolve(dir, "src/pages"),
      "@configs": path.resolve(dir, "src/configs"),
      "@components": path.resolve(dir, "src/Components"),
      "@routes": path.resolve(dir, "src/Routes"),
      "@utils": path.resolve(dir, "src/utils"),
      "@context": path.resolve(dir, "src/context"),
      "@assets": path.resolve(dir, "src/assets"),
      "@hooks": path.resolve(dir, "src/hooks"),
      "@hoc": path.resolve(dir, "src/hoc"),
    },
  },
  build: {
    sourcemap: true,
    rollupOptions: {
      input: {
        main: path.resolve(dir, "index.html"),
        connect: path.resolve(dir, "meet.html"),
        verify: path.resolve(dir, "zoomverify/verifyzoom.html"),
      },
      onwarn(warning, warn) {
        // discussion to safely ignore this warning: https://github.com/TanStack/query/issues/5175
        if (warning.code === "MODULE_LEVEL_DIRECTIVE") {
          return;
        }
        warn(warning);
      },
    },
  },
});
