import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '@/stores/authStore';
import apiClient from '@/api/client';


export const fetchMyWallet = (token: string) =>
  apiClient
    .get<{ id: string; tag: string; kycLevel: string; isActive: boolean }>('/wallets/me', {
      headers: { Authorization: `Bearer ${token}` },
    })
    .then((res) => res.data);

export function useUserWallet() {
  const accessToken = useAuthStore((s) => s.accessToken);
  const setWalletId = useAuthStore((s) => s.setWalletId);

  const query = useQuery({
    queryKey: ['my-wallet'],
    queryFn: () => fetchMyWallet(accessToken!),
    enabled: !!accessToken,
    staleTime: 5 * 60 * 1000, 
  });

  useEffect(() => {
    if (query.data) {
      localStorage.setItem('walletId', query.data.id);
      setWalletId(query.data.id);
    }
  }, [query.data, setWalletId]);

  return query;
}