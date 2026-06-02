import apiClient from './client';

export interface InitiateFundingRequest {
  amount: number;
  callbackUrl?: string;
}

export interface InitiateFundingResponse {
  authorizationUrl: string;
  reference: string;
}

export const initiateFunding = (walletId: string, data: InitiateFundingRequest) =>
  apiClient
    .post<InitiateFundingResponse>(`/wallets/${walletId}/fund`, data)
    .then((res) => res.data);

// Placeholder verify endpoint – will be added to backend later
export const verifyPayment = (reference: string) =>
  apiClient.get(`/paystack/verify/${reference}`).then((res) => res.data);