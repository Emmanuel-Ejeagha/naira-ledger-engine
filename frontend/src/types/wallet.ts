export interface Wallet {
  id: string;
  userId: string;
  tag: string | null;
  kycLevel: string;
  isActive: boolean;
  createdAt: string;
}

export interface WalletBalance {
  walletId: string;
  balance: number;
  currency: string;
}