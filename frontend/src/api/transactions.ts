import apiClient from './client';
import type { TransactionDto } from '../types/transaction';
import type { PagedResponse } from '../types/pagination';

export interface TransactionFilters {
  walletId: string;
  pageSize?: number;
  cursor?: string;
  dateFrom?: string;
  dateTo?: string;
  type?: string;
  search?: string;
}

export const getTransactions = (filters: TransactionFilters) =>
  apiClient
    .get<PagedResponse<TransactionDto>>('/transactions', { params: filters })
    .then((res) => res.data);