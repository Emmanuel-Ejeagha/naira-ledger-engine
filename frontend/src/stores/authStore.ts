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
  roles: string[];
  emailConfirmed: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, fullName: string, password: string) => Promise<any>;
  logout: () => void;
  hydrate: () => void;
  setWalletId: (id: string) => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: localStorage.getItem('accessToken'),
  refreshToken: localStorage.getItem('refreshToken'),
  walletId: null,
  roles: [],
  emailConfirmed: false,
  isLoading: true,

  hydrate: () => {
    const accessToken = localStorage.getItem('accessToken');
    const refreshToken = localStorage.getItem('refreshToken');
    if (accessToken) {
      try {
        const payload = JSON.parse(atob(accessToken.split('.')[1]));
        const rolesClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        const roles = rolesClaim ? (Array.isArray(rolesClaim) ? rolesClaim : [rolesClaim]) : [];
        set({
          user: {
            userId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
            email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
            fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
          },
          accessToken,
          refreshToken,
          roles,
          emailConfirmed: payload['email_verified'] === 'true' || false, 
          isLoading: false,
        });
      } catch {
        set({ isLoading: false, roles: [] });
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      }
    } else {
      set({ isLoading: false, roles: [] });
    }
  },

  login: async (email, password) => {
    const { data } = await apiClient.post('/auth/login', { email, password });
    const { accessToken, refreshToken } = data;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    const payload = JSON.parse(atob(accessToken.split('.')[1]));
    const rolesClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const roles = rolesClaim ? (Array.isArray(rolesClaim) ? rolesClaim : [rolesClaim]) : [];
    set({
      accessToken,
      refreshToken,
      user: {
        userId: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      },
      roles,
      emailConfirmed: payload['email_verified'] === 'true' || false, 
      walletId: null,   // <-- reset wallet on login
    });
    localStorage.removeItem('walletId');   // clear any leftover wallet ID
  },

  register: async (email, fullName, password) => {
    const { data } = await apiClient.post('/auth/register', { email, fullName, password });
    return data;
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    set({ user: null, accessToken: null, refreshToken: null, roles: [], walletId: null, emailConfirmed: false });
  },

  setWalletId: (id: string) => {
    localStorage.setItem('walletId', id);
    set({ walletId: id });
  },
}));