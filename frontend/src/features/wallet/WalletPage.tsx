import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getWalletById } from '@/api/wallet';
import { useWalletBalance } from '@/features/dashboard/hooks/useDashboard';
import { useAuthStore } from '@/stores/authStore';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { Copy, Check } from 'lucide-react';
import { toast } from 'sonner';

export default function WalletPage() {
  const { user } = useAuthStore();
  const walletId = localStorage.getItem('walletId');
  const [copied, setCopied] = useState(false);

  const { data: wallet, isLoading: walletLoading } = useQuery({
    queryKey: ['wallet', walletId],
    queryFn: () => getWalletById(walletId!),
    enabled: !!walletId,
  });

  const { data: balance, isLoading: balanceLoading } = useWalletBalance(walletId);

  const handleCopyTag = () => {
    if (wallet?.tag) {
      navigator.clipboard.writeText(wallet.tag);
      setCopied(true);
      toast.success('Wallet tag copied');
      setTimeout(() => setCopied(false), 2000);
    }
  };

  if (walletLoading || balanceLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (!wallet) {
    return <p className="text-danger">Wallet not found.</p>;
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">My Wallet</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Wallet Details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <span className="text-sm text-neutral">Wallet ID</span>
              <p className="font-mono text-sm break-all">{wallet.id}</p>
            </div>
            <div>
              <span className="text-sm text-neutral">Tag</span>
              <div className="flex items-center gap-2 mt-1">
                <Input value={wallet.tag ?? 'No tag'} readOnly className="max-w-xs" />
                <Button variant="outline" size="icon" onClick={handleCopyTag}>
                  {copied ? <Check className="h-4 w-4" /> : <Copy className="h-4 w-4" />}
                </Button>
              </div>
            </div>
            <div>
              <span className="text-sm text-neutral">KYC Level</span>
              <p className="font-medium">{wallet.kycLevel}</p>
            </div>
            <div>
              <span className="text-sm text-neutral">Status</span>
              <p className={`font-medium ${wallet.isActive ? 'text-success' : 'text-danger'}`}>
                {wallet.isActive ? 'Active' : 'Inactive'}
              </p>
            </div>
            <div>
              <span className="text-sm text-neutral">Created</span>
              <p className="text-sm">{new Date(wallet.createdAt).toLocaleDateString()}</p>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Balance</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-4xl font-bold text-primary-900">
              NGN {balance?.balance?.toFixed(2) ?? '0.00'}
            </p>
            <p className="text-xs text-neutral mt-2">Real-time balance</p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}