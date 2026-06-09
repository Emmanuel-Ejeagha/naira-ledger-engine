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
        target: 'https://naira-ledger-engine-1.onrender.com',  
        changeOrigin: true,
        secure: true,                     
      },
      '/hubs': {
        target: 'https://naira-ledger-engine-1.onrender.com',
        ws: true,                          
        changeOrigin: true,
        secure: true,
      },
    },
  },
})
