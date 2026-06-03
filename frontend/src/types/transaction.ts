export interface TransactionDto {
  transactionId: string;
  reference: string;
  type: string;
  status: string;
  amount: number;
  createdAt: string;
}