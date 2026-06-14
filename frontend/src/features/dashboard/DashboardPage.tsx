import { useAuthStore } from '@/stores/authStore';
import BalanceCard from './components/BalanceCard';
import RecentTransactions from './components/RecentTransactions';
import { useRecentTransactions, useWalletBalance } from './hooks/useDashboard';

export default function DashboardPage() {
  const { user } = useAuthStore();
  const walletId = useAuthStore((s) => s.walletId);
  const { data: balance, isLoading: balanceLoading } = useWalletBalance(walletId);
  const {
    data: transactions,
    isLoading: txLoading,
    isError: txError,
  } = useRecentTransactions(walletId);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Welcome back, {user?.fullName}</h1>
        <p className="text-neutral">Here's your account overview.</p>
      </div>

      <BalanceCard balance={balance?.balance} isLoading={balanceLoading} />

      <div>
        <h3 className="text-lg font-semibold mb-3">Recent Activity</h3>
        <RecentTransactions
          transactions={transactions ?? []}
          isLoading={txLoading}
          isError={txError}
        />
      </div>
    </div>
  );
}