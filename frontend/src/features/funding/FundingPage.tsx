import { useState, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { initiateFunding, verifyPayment } from '@/api/funding';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { fundingSchema, type FundingFormData } from '@/lib/validations/transactions';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';
import { CheckCircle, XCircle, ExternalLink, Loader2 } from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';
import BalanceCard from '../dashboard/components/BalanceCard';
import { useWalletBalance } from '@/hooks/useDashboard';

export default function FundingPage() {
  const walletId = useAuthStore((s) => s.walletId);
  const queryClient = useQueryClient();
  const { data: balance, isLoading: balanceLoading } = useWalletBalance(walletId);
  const [authorizationUrl, setAuthorizationUrl] = useState<string | null>(null);
  const [reference, setReference] = useState<string | null>(null);
  const [verifyStatus, setVerifyStatus] = useState<'pending' | 'success' | 'failed' | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FundingFormData>({
    resolver: zodResolver(fundingSchema),
    defaultValues: { amount: 1000 },
  });

  const initiateMutation = useMutation({
    mutationFn: (data: FundingFormData) => {
      if (!walletId) throw new Error('Wallet ID is missing');
      return initiateFunding(walletId, {
        walletId,
        amount: data.amount,
        callbackUrl: window.location.origin + '/fund'
      });
    },
    onSuccess: (data) => {
      setAuthorizationUrl(data.authorizationUrl);
      setReference(data.reference);
      toast.success('Payment link generated');
    },
    onError: (err: any) => {
      console.error('Funding initiation error:', err);  // 👈 View error in browser console
      const message = err.response?.data?.error || err.message || 'Initiation failed';
      toast.error(message);
    },
  });

  const verifyMutation = useMutation({
    mutationFn: () => verifyPayment(reference!),
    onSuccess: (data: any) => {
      if (data.status === 'success') {
        setVerifyStatus('success');
        toast.success('Payment verified!');
        queryClient.invalidateQueries({ queryKey: ['walletBalance', walletId] });
      } else {
        setVerifyStatus('failed');
        toast.error('Payment not successful');
      }
    },
    onError: () => {
      setVerifyStatus('failed');
      toast.error('Verification failed');
    },
  });

  const onSubmit = (data: FundingFormData) => {
    if (!walletId) {
      toast.error('Wallet not yet loaded. Please refresh the page.');
      return;
    }
    initiateMutation.mutate(data);
  };

  // Show a loading skeleton while walletId is being fetched
  if (!walletId) {
    return (
      <div className="space-y-6 max-w-lg">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-lg">
      <h1 className="text-2xl font-bold">Fund Your Wallet</h1>

      {/* Balance display */}
      {balanceLoading ? (
        <Skeleton className="h-16 w-full" />
      ) : (
        <BalanceCard balance={balance?.balance} isLoading={balanceLoading} label="Current Balance" />
      )}

      {/* Funding Form */}
      {!authorizationUrl ? (
        <Card>
          <CardHeader>
            <CardTitle>Initiate Payment</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="amount">Amount (NGN)</Label>
                <Input
                  id="amount"
                  type="number"
                  placeholder="1000"
                  {...register('amount', { valueAsNumber: true })}
                  disabled={initiateMutation.isPending}
                />
                {errors.amount && <p className="text-sm text-danger">{errors.amount.message}</p>}
              </div>
              <Button
                type="submit"
                className="w-full"
                disabled={initiateMutation.isPending || !walletId}
              >
                {initiateMutation.isPending ? 'Generating...' : 'Proceed to Pay'}
              </Button>
            </form>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="py-4 space-y-4">
            <Alert>
              <AlertDescription>
                <span className="text-red-600">
                  You will be redirected to Paystack to complete your payment of NGN {initiateMutation.variables?.amount}.
                </span>
              </AlertDescription>
            </Alert>
            <div className="flex gap-4">
              <a
                href={authorizationUrl}
                target="_blank"
                rel="noopener noreferrer"
                className="inline-flex items-center gap-2 px-4 py-2 bg-primary text-white rounded-md hover:bg-primary/90"
              >
                <ExternalLink className="h-4 w-4" /> Pay Now
              </a>
              <Button
                variant="outline"
                onClick={() => verifyMutation.mutate()}
                disabled={verifyMutation.isPending}
              >
                {verifyMutation.isPending ? <Loader2 className="animate-spin" /> : 'Check Status'}
              </Button>
            </div>
            {reference && <p className="text-sm text-neutral">Reference: {reference}</p>}
          </CardContent>
        </Card>
      )}

      {/* Verification Status */}
      {verifyStatus && (
        <div className={`flex items-center gap-2 ${verifyStatus === 'success' ? 'text-success' : 'text-danger'}`}>
          {verifyStatus === 'success' ? <CheckCircle className="h-5 w-5" /> : <XCircle className="h-5 w-5" />}
          <span className="font-medium">Payment {verifyStatus === 'success' ? 'successful' : 'failed'}</span>
        </div>
      )}
    </div>
  );
}