import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getPendingKycWallets, approveKyc, rejectKyc } from '@/api/admin';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';
import { CheckCircle, XCircle } from 'lucide-react';

export default function KycApprovalPage() {
  const queryClient = useQueryClient();

  const { data: wallets, isLoading, isError } = useQuery({
    queryKey: ['admin-kyc-pending'],
    queryFn: getPendingKycWallets,
  });

  const approveMutation = useMutation({
    mutationFn: approveKyc,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-kyc-pending'] });
      toast.success('KYC approved');
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Approval failed');
    },
  });

  const rejectMutation = useMutation({
    mutationFn: rejectKyc,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-kyc-pending'] });
      toast.success('KYC rejected');
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Rejection failed');
    },
  });

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (isError) {
    return <p className="text-danger">Failed to load pending KYC wallets.</p>;
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">KYC Pending Approvals</h1>
      <Card>
        <CardContent className="p-0">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border text-left">
                <th className="py-3 px-4">Wallet ID</th>
                <th className="py-3 px-4">User ID</th>
                <th className="py-3 px-4">Tag</th>
                <th className="py-3 px-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              {wallets?.map((wallet) => (
                <tr key={wallet.id} className="border-b border-border hover:bg-muted/50">
                  <td className="py-3 px-4 font-mono text-xs">{wallet.id}</td>
                  <td className="py-3 px-4">{wallet.userId}</td>
                  <td className="py-3 px-4">{wallet.tag ?? '—'}</td>
                  <td className="py-3 px-4 flex gap-2">
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => approveMutation.mutate(wallet.id)}
                      disabled={approveMutation.isPending}
                    >
                      <CheckCircle className="h-4 w-4 mr-1" /> Approve
                    </Button>
                    <Button
                      size="sm"
                      variant="destructive"
                      onClick={() => rejectMutation.mutate(wallet.id)}
                      disabled={rejectMutation.isPending}
                    >
                      <XCircle className="h-4 w-4 mr-1" /> Reject
                    </Button>
                  </td>
                </tr>
              ))}
              {(!wallets || wallets.length === 0) && (
                <tr>
                  <td colSpan={4} className="py-8 text-center text-neutral">
                    No pending KYC wallets.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  );
}