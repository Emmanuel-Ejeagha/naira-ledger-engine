import { z } from 'zod';

export const fundingSchema = z.object({
  amount: z.number().min(100, 'Minimum NGN 100'),
});

export const transferSchema = z.object({
  fromWalletId: z.string().uuid('Invalid wallet ID'),
  toWalletId: z.string().uuid('Invalid wallet ID'),
  amount: z.number().min(100, 'Minimum NGN 100'),
  idempotencyKey: z.string().optional(),
});

export type FundingFormData = z.infer<typeof fundingSchema>;
export type TransferFormData = z.infer<typeof transferSchema>;