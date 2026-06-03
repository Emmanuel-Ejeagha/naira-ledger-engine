import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/authStore';
import apiClient from '@/api/client';

export const fetchMyWallet = () =>
  apiClient.get<{ id: string; tag: string; kycLevel: string; isActive: boolean }>('/wallets/me').then((res) => res.data);

export function useUserWallet() {
  const { accessToken, setWalletId } = useAuthStore();

  const query = useQuery({
    queryKey: ['my-wallet'],
    queryFn: fetchMyWallet,
    enabled: !!accessToken,
    staleTime: 5 * 60 * 1000, // 5 minutes
    onSuccess: (data) => {
      localStorage.setItem('walletId', data.id); // keep for backward compatibility
      setWalletId(data.id);
    },
  });

  useEffect(() => {
    if (query.data) {
      localStorage.setItem('walletId', query.data.id);
      setWalletId(query.data.id);
    }
  }, [query.data, setWalletId]);

  return query;
}