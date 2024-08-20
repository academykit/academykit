// vite.config.ts
import { sentryVitePlugin } from "file:///C:/Users/saile/OneDrive/Documents/GitHub/academykit/academykit.client/node_modules/@sentry/vite-plugin/dist/esm/index.mjs";
import react from "file:///C:/Users/saile/OneDrive/Documents/GitHub/academykit/academykit.client/node_modules/@vitejs/plugin-react/dist/index.mjs";
import child_process from "node:child_process";
import fs, { readFileSync } from "node:fs";
import path from "node:path";
import { env } from "node:process";
import { defineConfig } from "file:///C:/Users/saile/OneDrive/Documents/GitHub/academykit/academykit.client/node_modules/vite/dist/node/index.js";
import eslintPlugin from "file:///C:/Users/saile/OneDrive/Documents/GitHub/academykit/academykit.client/node_modules/vite-plugin-eslint/dist/index.mjs";
var __vite_injected_original_dirname = "C:\\Users\\saile\\OneDrive\\Documents\\GitHub\\academykit\\academykit.client";
var dir = __vite_injected_original_dirname;
var baseFolder = env.APPDATA !== void 0 && env.APPDATA !== "" ? `${env.APPDATA}/ASP.NET/https` : `${env.HOME}/.aspnet/https`;
var certificateName = "academykit.client";
var certFilePath = path.join(baseFolder, `${certificateName}.pem`);
var keyFilePath = path.join(baseFolder, `${certificateName}.key`);
if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (0 !== child_process.spawnSync(
    "dotnet",
    [
      "dev-certs",
      "https",
      "--export-path",
      certFilePath,
      "--format",
      "Pem",
      "--no-password"
    ],
    { stdio: "inherit" }
  ).status) {
    throw new Error("Could not create certificate.");
  }
}
var vite_config_default = defineConfig({
  plugins: [
    react(),
    eslintPlugin({ fix: true }),
    sentryVitePlugin({
      org: "academy-kit",
      project: "academykit-web"
    })
  ],
  server: {
    https: {
      key: readFileSync(keyFilePath),
      cert: readFileSync(certFilePath)
    },
    host: true,
    port: 44414,
    strictPort: true,
    proxy: {
      "/api": {
        target: "https://127.0.0.1:7042",
        changeOrigin: true,
        secure: false,
        rewrite: (path2) => path2.replace(/.*\/api/, "/api")
      }
    }
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
      "@hoc": path.resolve(dir, "src/hoc")
    }
  },
  build: {
    sourcemap: true,
    rollupOptions: {
      input: {
        main: path.resolve(dir, "index.html"),
        connect: path.resolve(dir, "meet.html"),
        verify: path.resolve(dir, "zoomverify/verifyzoom.html")
      },
      onwarn(warning, warn) {
        if (warning.code === "MODULE_LEVEL_DIRECTIVE") {
          return;
        }
        warn(warning);
      }
    }
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJDOlxcXFxVc2Vyc1xcXFxzYWlsZVxcXFxPbmVEcml2ZVxcXFxEb2N1bWVudHNcXFxcR2l0SHViXFxcXGFjYWRlbXlraXRcXFxcYWNhZGVteWtpdC5jbGllbnRcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZmlsZW5hbWUgPSBcIkM6XFxcXFVzZXJzXFxcXHNhaWxlXFxcXE9uZURyaXZlXFxcXERvY3VtZW50c1xcXFxHaXRIdWJcXFxcYWNhZGVteWtpdFxcXFxhY2FkZW15a2l0LmNsaWVudFxcXFx2aXRlLmNvbmZpZy50c1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9pbXBvcnRfbWV0YV91cmwgPSBcImZpbGU6Ly8vQzovVXNlcnMvc2FpbGUvT25lRHJpdmUvRG9jdW1lbnRzL0dpdEh1Yi9hY2FkZW15a2l0L2FjYWRlbXlraXQuY2xpZW50L3ZpdGUuY29uZmlnLnRzXCI7aW1wb3J0IHsgc2VudHJ5Vml0ZVBsdWdpbiB9IGZyb20gXCJAc2VudHJ5L3ZpdGUtcGx1Z2luXCI7XG5pbXBvcnQgcmVhY3QgZnJvbSBcIkB2aXRlanMvcGx1Z2luLXJlYWN0XCI7XG5pbXBvcnQgY2hpbGRfcHJvY2VzcyBmcm9tIFwibm9kZTpjaGlsZF9wcm9jZXNzXCI7XG5pbXBvcnQgZnMsIHsgcmVhZEZpbGVTeW5jIH0gZnJvbSBcIm5vZGU6ZnNcIjtcbmltcG9ydCBwYXRoIGZyb20gXCJub2RlOnBhdGhcIjtcbmltcG9ydCB7IGVudiB9IGZyb20gXCJub2RlOnByb2Nlc3NcIjtcbmltcG9ydCB7IGRlZmluZUNvbmZpZyB9IGZyb20gXCJ2aXRlXCI7XG5pbXBvcnQgZXNsaW50UGx1Z2luIGZyb20gXCJ2aXRlLXBsdWdpbi1lc2xpbnRcIjtcblxuY29uc3QgZGlyID0gX19kaXJuYW1lO1xuXG5jb25zdCBiYXNlRm9sZGVyID1cblx0ZW52LkFQUERBVEEgIT09IHVuZGVmaW5lZCAmJiBlbnYuQVBQREFUQSAhPT0gXCJcIlxuXHRcdD8gYCR7ZW52LkFQUERBVEF9L0FTUC5ORVQvaHR0cHNgXG5cdFx0OiBgJHtlbnYuSE9NRX0vLmFzcG5ldC9odHRwc2A7XG5cbmNvbnN0IGNlcnRpZmljYXRlTmFtZSA9IFwiYWNhZGVteWtpdC5jbGllbnRcIjtcbmNvbnN0IGNlcnRGaWxlUGF0aCA9IHBhdGguam9pbihiYXNlRm9sZGVyLCBgJHtjZXJ0aWZpY2F0ZU5hbWV9LnBlbWApO1xuY29uc3Qga2V5RmlsZVBhdGggPSBwYXRoLmpvaW4oYmFzZUZvbGRlciwgYCR7Y2VydGlmaWNhdGVOYW1lfS5rZXlgKTtcblxuaWYgKCFmcy5leGlzdHNTeW5jKGNlcnRGaWxlUGF0aCkgfHwgIWZzLmV4aXN0c1N5bmMoa2V5RmlsZVBhdGgpKSB7XG5cdGlmIChcblx0XHQwICE9PVxuXHRcdGNoaWxkX3Byb2Nlc3Muc3Bhd25TeW5jKFxuXHRcdFx0XCJkb3RuZXRcIixcblx0XHRcdFtcblx0XHRcdFx0XCJkZXYtY2VydHNcIixcblx0XHRcdFx0XCJodHRwc1wiLFxuXHRcdFx0XHRcIi0tZXhwb3J0LXBhdGhcIixcblx0XHRcdFx0Y2VydEZpbGVQYXRoLFxuXHRcdFx0XHRcIi0tZm9ybWF0XCIsXG5cdFx0XHRcdFwiUGVtXCIsXG5cdFx0XHRcdFwiLS1uby1wYXNzd29yZFwiLFxuXHRcdFx0XSxcblx0XHRcdHsgc3RkaW86IFwiaW5oZXJpdFwiIH0sXG5cdFx0KS5zdGF0dXNcblx0KSB7XG5cdFx0dGhyb3cgbmV3IEVycm9yKFwiQ291bGQgbm90IGNyZWF0ZSBjZXJ0aWZpY2F0ZS5cIik7XG5cdH1cbn1cblxuZXhwb3J0IGRlZmF1bHQgZGVmaW5lQ29uZmlnKHtcblx0cGx1Z2luczogW1xuXHRcdHJlYWN0KCksXG5cdFx0ZXNsaW50UGx1Z2luKHsgZml4OiB0cnVlIH0pLFxuXHRcdHNlbnRyeVZpdGVQbHVnaW4oe1xuXHRcdFx0b3JnOiBcImFjYWRlbXkta2l0XCIsXG5cdFx0XHRwcm9qZWN0OiBcImFjYWRlbXlraXQtd2ViXCIsXG5cdFx0fSksXG5cdF0sXG5cdHNlcnZlcjoge1xuXHRcdGh0dHBzOiB7XG5cdFx0XHRrZXk6IHJlYWRGaWxlU3luYyhrZXlGaWxlUGF0aCksXG5cdFx0XHRjZXJ0OiByZWFkRmlsZVN5bmMoY2VydEZpbGVQYXRoKSxcblx0XHR9LFxuXHRcdGhvc3Q6IHRydWUsXG5cdFx0cG9ydDogNDQ0MTQsXG5cdFx0c3RyaWN0UG9ydDogdHJ1ZSxcblx0XHRwcm94eToge1xuXHRcdFx0XCIvYXBpXCI6IHtcblx0XHRcdFx0dGFyZ2V0OiBcImh0dHBzOi8vMTI3LjAuMC4xOjcwNDJcIixcblx0XHRcdFx0Y2hhbmdlT3JpZ2luOiB0cnVlLFxuXHRcdFx0XHRzZWN1cmU6IGZhbHNlLFxuXHRcdFx0XHRyZXdyaXRlOiAocGF0aCkgPT4gcGF0aC5yZXBsYWNlKC8uKlxcL2FwaS8sIFwiL2FwaVwiKSxcblx0XHRcdH0sXG5cdFx0fSxcblx0fSxcblx0cmVzb2x2ZToge1xuXHRcdGFsaWFzOiB7XG5cdFx0XHQvLyBcIkBcIjogcGF0aC5yZXNvbHZlKF9fZGlybmFtZSwgXCIuc3JjL1wiKSxcblx0XHRcdFwiQHBhZ2VzXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL3BhZ2VzXCIpLFxuXHRcdFx0XCJAY29uZmlnc1wiOiBwYXRoLnJlc29sdmUoZGlyLCBcInNyYy9jb25maWdzXCIpLFxuXHRcdFx0XCJAY29tcG9uZW50c1wiOiBwYXRoLnJlc29sdmUoZGlyLCBcInNyYy9Db21wb25lbnRzXCIpLFxuXHRcdFx0XCJAcm91dGVzXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL1JvdXRlc1wiKSxcblx0XHRcdFwiQHV0aWxzXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL3V0aWxzXCIpLFxuXHRcdFx0XCJAY29udGV4dFwiOiBwYXRoLnJlc29sdmUoZGlyLCBcInNyYy9jb250ZXh0XCIpLFxuXHRcdFx0XCJAYXNzZXRzXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL2Fzc2V0c1wiKSxcblx0XHRcdFwiQGhvb2tzXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL2hvb2tzXCIpLFxuXHRcdFx0XCJAaG9jXCI6IHBhdGgucmVzb2x2ZShkaXIsIFwic3JjL2hvY1wiKSxcblx0XHR9LFxuXHR9LFxuXHRidWlsZDoge1xuXHRcdHNvdXJjZW1hcDogdHJ1ZSxcblx0XHRyb2xsdXBPcHRpb25zOiB7XG5cdFx0XHRpbnB1dDoge1xuXHRcdFx0XHRtYWluOiBwYXRoLnJlc29sdmUoZGlyLCBcImluZGV4Lmh0bWxcIiksXG5cdFx0XHRcdGNvbm5lY3Q6IHBhdGgucmVzb2x2ZShkaXIsIFwibWVldC5odG1sXCIpLFxuXHRcdFx0XHR2ZXJpZnk6IHBhdGgucmVzb2x2ZShkaXIsIFwiem9vbXZlcmlmeS92ZXJpZnl6b29tLmh0bWxcIiksXG5cdFx0XHR9LFxuXHRcdFx0b253YXJuKHdhcm5pbmcsIHdhcm4pIHtcblx0XHRcdFx0Ly8gZGlzY3Vzc2lvbiB0byBzYWZlbHkgaWdub3JlIHRoaXMgd2FybmluZzogaHR0cHM6Ly9naXRodWIuY29tL1RhblN0YWNrL3F1ZXJ5L2lzc3Vlcy81MTc1XG5cdFx0XHRcdGlmICh3YXJuaW5nLmNvZGUgPT09IFwiTU9EVUxFX0xFVkVMX0RJUkVDVElWRVwiKSB7XG5cdFx0XHRcdFx0cmV0dXJuO1xuXHRcdFx0XHR9XG5cdFx0XHRcdHdhcm4od2FybmluZyk7XG5cdFx0XHR9LFxuXHRcdH0sXG5cdH0sXG59KTtcbiJdLAogICJtYXBwaW5ncyI6ICI7QUFBaVosU0FBUyx3QkFBd0I7QUFDbGIsT0FBTyxXQUFXO0FBQ2xCLE9BQU8sbUJBQW1CO0FBQzFCLE9BQU8sTUFBTSxvQkFBb0I7QUFDakMsT0FBTyxVQUFVO0FBQ2pCLFNBQVMsV0FBVztBQUNwQixTQUFTLG9CQUFvQjtBQUM3QixPQUFPLGtCQUFrQjtBQVB6QixJQUFNLG1DQUFtQztBQVN6QyxJQUFNLE1BQU07QUFFWixJQUFNLGFBQ0wsSUFB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
