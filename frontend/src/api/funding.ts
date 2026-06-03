import apiClient from './client';

export interface InitiateFundingRequest {
  walletId: string;        // REQUIRED in the body
  amount: number;
  callbackUrl?: string;
}

export interface InitiateFundingResponse {
  authorizationUrl: string;
  reference: string;
}

export const initiateFunding = (walletId: string, data: InitiateFundingRequest) =>
  apiClient
    .post<InitiateFundingResponse>(`/wallets/${walletId}/fund`, {
      walletId: data.walletId,          // send in body
      amount: data.amount,
      callbackUrl: data.callbackUrl ?? window.location.origin + '/fund',
    })
    .then((res) => res.data);

// Placeholder for verification (not used yet)
export const verifyPayment = (reference: string) =>
  apiClient.get(`/paystack/verify/${reference}`).then((res) => res.data);