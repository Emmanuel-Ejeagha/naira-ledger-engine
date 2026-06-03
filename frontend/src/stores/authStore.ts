import { create } from 'zustand';
import apiClient from '../api/client';

interface User {
  userId: string;
  email: string;
  fullName: string;
}

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null;
  walletId: string | null; 
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, fullName: string, password: string) => Promise<void>;
  logout: () => void;
  hydrate: () => void;
  setWalletId: (id: string) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  walletId: null,
  setWalletId: (id: string) => set({ walletId: id }),
  accessToken: localStorage.getItem('accessToken'),
  refreshToken: localStorage.getItem('refreshToken'),
  isLoading: true,

  hydrate: () => {
    const accessToken = localStorage.getItem('accessToken');
    const refreshToken = localStorage.getItem('refreshToken');
    if (accessToken) {
      try {
        const payload = JSON.parse(atob(accessToken.split('.')[1]));
        set({
          user: {
            userId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
            email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
            fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
          },
          accessToken,
          refreshToken,
          isLoading: false,
        });
      } catch {
        set({ isLoading: false });
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      }
    } else {
      set({ isLoading: false });
    }
  },

  login: async (email, password) => {
    const { data } = await apiClient.post('/auth/login', { email, password });
    const { accessToken, refreshToken } = data;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    const payload = JSON.parse(atob(accessToken.split('.')[1]));
    set({
      accessToken,
      refreshToken,
      user: {
        userId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      },
    });
  },

  register: async (email, fullName, password) => {
    await apiClient.post('/auth/register', { email, fullName, password });
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    set({ user: null, accessToken: null, refreshToken: null });
  },
}));