import { type TransactionDto } from '@/types/transaction';
import { Skeleton } from '@/components/ui/skeleton';

interface RecentTransactionsProps {
  transactions: TransactionDto[];
  isLoading: boolean;
  isError: boolean;
}

export default function RecentTransactions({ transactions, isLoading, isError }: RecentTransactionsProps) {
  if (isLoading) {
    return (
      <div className="space-y-3">
        {[...Array(3)].map((_, i) => (
          <Skeleton key={i} className="h-10 w-full" />
        ))}
      </div>
    );
  }

  if (isError) {
    return <p className="text-sm text-danger">Failed to load recent activity.</p>;
  }

  if (transactions.length === 0) {
    return (
      <div className="text-center py-8">
        <p className="text-neutral">No transactions yet.</p>
      </div>
    );
  }

  return (
    <ul className="divide-y divide-border">
      {transactions.map((tx) => (
        <li key={tx.transactionId} className="flex items-center justify-between py-3">
          <div>
            <p className="text-sm font-medium">{tx.type}</p>
            <p className="text-xs text-neutral">{new Date(tx.createdAt).toLocaleDateString()}</p>
          </div>
          <div className="text-right">
            <p className="text-sm font-semibold">NGN {Math.abs(tx.amount).toFixed(2)}</p>
            <p className={`text-xs ${tx.status === 'Completed' ? 'text-success' : 'text-warning'}`}>
              {tx.status}
            </p>
          </div>
        </li>
      ))}
    </ul>
  );
}