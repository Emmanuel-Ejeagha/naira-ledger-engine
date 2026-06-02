import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  return {
    plugins: [react()],
    server: {
      port: 3000,
      proxy: {
        '/api': {
          target: env.VITE_API_BASE_URL || 'https://naira-ledger-engine-1.onrender.com',
          changeOrigin: true,
          secure: true,
        },
        '/hubs': {
          target: env.VITE_API_BASE_URL || 'https://naira-ledger-engine-1.onrender.com',
          ws: true,
          changeOrigin: true,
          secure: true,
        },
      },
    },
  };
});