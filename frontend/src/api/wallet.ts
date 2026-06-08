import apiClient from './client';
import type { Wallet, WalletBalance } from '../types/wallet';

export const getWalletById = (walletId: string) =>
  apiClient.get<Wallet>(`/wallets/${walletId}`).then((res) => res.data);

export const getWalletBalance = (walletId: string) =>
  apiClient.get<WalletBalance>(`/wallets/${walletId}/balance`).then((res) => res.data);