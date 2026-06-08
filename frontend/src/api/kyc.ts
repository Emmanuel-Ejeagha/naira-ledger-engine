import apiClient from './client';
import type { KycInfo, SubmitKycRequest } from '@/types/kyc';

export const getKycStatus = (walletId: string) =>
  apiClient.get<KycInfo>(`/wallets/${walletId}`).then((res) => ({
    kycLevel: (res.data as any).kycLevel,  // extract kycLevel from wallet response
    canSubmit: (res.data as any).kycLevel === 'Unverified',
  }));

export const submitKyc = (data: SubmitKycRequest) =>
  apiClient.post('/kyc/submit', data);