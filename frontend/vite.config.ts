import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 3000,              // frontend port
    proxy: {
      '/api': {
        target: 'https://localhost:8081',  
        changeOrigin: true,
        secure: true,                     
      },
      '/hubs': {
        target: 'https://localhost:8081',
        ws: true,                          
        changeOrigin: true,
        secure: true,
      },
    },
  },
})
