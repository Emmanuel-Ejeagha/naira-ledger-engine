import { useQuery } from '@tanstack/react-query';
import { getWalletBalance } from '@/api/wallet';
import { getTransactions } from '@/api/transactions';

export function useWalletBalance(walletId: string | null) {
  return useQuery({
    queryKey: ['walletBalance', walletId],
    queryFn: () => getWalletBalance(walletId!),
    enabled: !!walletId,
  });
}

export function useRecentTransactions(walletId: string | null, limit = 5) {
  return useQuery({
    queryKey: ['transactions', walletId, 'recent', limit],
    queryFn: () => getTransactions({ walletId: walletId!, pageSize: limit }),
    enabled: !!walletId,
    select: (data) => data.items,
  });
}