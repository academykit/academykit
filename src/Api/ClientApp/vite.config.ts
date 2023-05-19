import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import fs, { readFileSync } from 'fs'
import path , { join } from 'path';
// @ts-ignore
const dir = __dirname;

const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ''
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

const certificateName = process.env.npm_package_name

const certFilePath = join(baseFolder, `${certificateName}.pem`);
const keyFilePath = join(baseFolder, `${certificateName}.key`);

export default defineConfig({
  plugins: [react()],
  server: {
    https: {
      key: readFileSync(keyFilePath),
      cert: readFileSync(certFilePath)
    },
    port: 44414,
    strictPort : true,
    proxy: {
      '/api' : {
        target: 'https://localhost:7042',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '/api')
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
      "@hoc": path.resolve(dir, "src/hoc"),
    },
  },
  build: {
    sourcemap: false,
    rollupOptions: {
      input: {
        main: path.resolve(dir, "index.html"),
        connect: path.resolve(dir, "meet.html"),
        verify: path.resolve(dir, "zoomverify/verifyzoom.html"),

      },
    },
  },
});
