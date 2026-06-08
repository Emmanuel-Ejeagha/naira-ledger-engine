import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';

interface BalanceCardProps {
  balance: number | undefined;
  isLoading: boolean;
  label?: string;
}

export default function BalanceCard({ balance, isLoading, label = 'Available Balance' }: BalanceCardProps) {
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

  const balanceColor = balance && balance > 0 ? 'text-success' : 'text-foreground';

  return (
    <Card className="p-6 border-l-4 border-success">
      <CardContent className="flex flex-col space-y-2">
        <span className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
          {label}
        </span>
        <span className={`text-4xl font-bold ${balanceColor}`}>
          NGN {balance?.toFixed(2) ?? '0.00'}
        </span>
        <span className="text-xs text-muted-foreground">Updated in real-time</span>
      </CardContent>
    </Card>
  );
}