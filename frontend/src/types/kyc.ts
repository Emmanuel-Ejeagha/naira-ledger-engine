export interface KycInfo {
  kycLevel: string;
  canSubmit: boolean;
}

export interface SubmitKycRequest {
  walletId: string;
  fullName: string;
  idNumber: string;
  idType: string;
}