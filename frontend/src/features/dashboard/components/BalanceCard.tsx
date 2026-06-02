import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';

interface BalanceCardProps {
  balance: number | undefined;
  isLoading: boolean;
}

export default function BalanceCard({ balance, isLoading }: BalanceCardProps) {
  if (isLoading) {
    return (
      <Card className="p-6">
        <CardContent className="space-y-2">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-9 w-40" />
          <Skeleton className="h-4 w-32" />
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="p-6">
      <CardContent className="flex flex-col space-y-2">
        <span className="text-sm font-medium text-neutral">Available Balance</span>
        <span className="text-4xl font-bold text-primary-900">
          NGN {balance?.toFixed(2) ?? '0.00'}
        </span>
        <span className="text-xs text-neutral">Updated in real-time</span>
      </CardContent>
    </Card>
  );
}