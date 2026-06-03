import apiClient from './client';

export interface TransferRequest {
  fromWalletId: string;
  toWalletId: string;
  amount: number;
  idempotencyKey: string;
}

export interface TransferResponse {
  transactionId: string;
  message: string;
}

export const executeTransfer = (data: TransferRequest) =>
  apiClient.post<TransferResponse>('/transfers', data).then((res) => res.data);