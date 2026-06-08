import apiClient from './client';

export interface PendingKycWallet {
  id: string;
  userId: string;
  tag: string | null;
  kycLevel: string;
  createdAt: string;
}

export const getPendingKycWallets = () =>
  apiClient.get<PendingKycWallet[]>('/admin/kyc/pending').then((res) => res.data);

export const approveKyc = (walletId: string) =>
  apiClient.post(`/admin/kyc/approve/${walletId}`).then((res) => res.data);

export const rejectKyc = (walletId: string) =>
  apiClient.post(`/admin/kyc/reject/${walletId}`).then((res) => res.data);

export const reverseTransaction = (transactionId: string) =>
  apiClient.post(`/admin/transactions/${transactionId}/reverse`).then((res) => res.data);