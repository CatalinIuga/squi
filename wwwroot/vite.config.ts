import vue from "@vitejs/plugin-vue";
import autoprefixer from "autoprefixer";
import path from "path";
import tailwind from "tailwindcss";
import { defineConfig } from "vite";

const __dirname = path.dirname("./src");

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue({
    template: {
      compilerOptions: {
        isCustomElement: (tag) => tag.startsWith("ag-"),
      },
    },
  })],
  resolve: {
    alias: {
      "@": path.resolve(__dirname + "/src"),
    },
  },
  css: {
    postcss: {
      plugins: [tailwind(), autoprefixer()],
    },
  },
});
