import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { executeTransfer, type TransferRequest } from '@/api/transfer';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { transferSchema, type TransferFormData } from '@/lib/validations/transactions';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';
import { CheckCircle, ArrowLeftRight } from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';
import { useWalletBalance } from '@/hooks/useDashboard';

type Step = 'form' | 'confirm' | 'receipt';
type TransferFormDataWithKey = TransferFormData & { idempotencyKey?: string };

export default function TransferPage() {
  const walletId = useAuthStore((s) => s.walletId);
  const { data: balance } = useWalletBalance(walletId);
  const [step, setStep] = useState<Step>('form');
  const [formData, setFormData] = useState<TransferFormData | null>(null);
  const [transactionId, setTransactionId] = useState<string | null>(null);
  const [reversalWindow, setReversalWindow] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<TransferFormData>({
    resolver: zodResolver(transferSchema),
    defaultValues: {
      fromWalletId: walletId ?? '',
      toWalletId: '',
      amount: undefined,
    },
  });

  const transferMutation = useMutation({
    mutationFn: (data: TransferFormData) => executeTransfer(data as TransferRequest),
    onSuccess: (data) => {
      setTransactionId(data.transactionId);
      setReversalWindow(true);
      setStep('receipt');
      toast.success('Transfer completed');
    },
    onError: (err: any) => {
      const message = err.response?.data?.error || err.message || 'Transfer failed';
      toast.error(message);
      setStep('form');
    },
  });

  const onSubmit = (data: TransferFormData) => {
    setFormData({ ...data, idempotencyKey: crypto.randomUUID() });
    setStep('confirm');
  };

  const handleConfirm = () => {
  if (formData) {
    const request: TransferRequest = {
      fromWalletId: formData.fromWalletId,
      toWalletId: formData.toWalletId,
      amount: formData.amount,
      idempotencyKey: String(formData.idempotencyKey), // force plain string
    };
    transferMutation.mutate(request);
  }
};


  const handleCancel = () => {
    setStep('form');
    setFormData(null);
  };

  const isReversible = reversalWindow && step === 'receipt';

  // Guard: show skeleton until wallet ID is loaded
  if (!walletId) {
    return (
      <div className="max-w-lg space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  return (
    <div className="max-w-lg space-y-6">
      <h1 className="text-2xl font-bold">Transfer NGN</h1>

      {step === 'form' && (
        <Card>
          <CardHeader>
            <CardTitle>Transfer Details</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="fromWalletId">Your Wallet ID</Label>
                <Input
                  id="fromWalletId"
                  value={walletId}
                  {...register('fromWalletId')}
                  readOnly
                  className="bg-muted"
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="toWalletId">Recipient Wallet ID</Label>
                <Input
                  id="toWalletId"
                  placeholder="Recipient's wallet ID"
                  {...register('toWalletId')}
                />
                {errors.toWalletId && (
                  <p className="text-sm text-destructive">{errors.toWalletId.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="amount">Amount (NGN)</Label>
                <Input
                  id="amount"
                  type="number"
                  placeholder="1000"
                  {...register('amount', { valueAsNumber: true })}
                />
                {errors.amount && (
                  <p className="text-sm text-destructive">{errors.amount.message}</p>
                )}
                {balance && (
                  <p className="text-xs text-success font-semibold">
                    Balance: NGN {balance.balance.toFixed(2)}
                  </p>
                )}
              </div>
              <Button type="submit" className="w-full">
                Review Transfer
              </Button>
            </form>
          </CardContent>
        </Card>
      )}

      {step === 'confirm' && formData && (
        <Card>
          <CardHeader>
            <CardTitle>Confirm Transfer</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <dl className="grid grid-cols-2 gap-2 text-sm">
              <dt className="text-muted-foreground">From</dt>
              <dd className="font-mono text-xs">{formData.fromWalletId}</dd>
              <dt className="text-muted-foreground">To</dt>
              <dd className="font-mono text-xs">{formData.toWalletId}</dd>
              <dt className="text-muted-foreground">Amount</dt>
              <dd className="font-semibold">NGN {formData.amount.toFixed(2)}</dd>
              <dt className="text-muted-foreground">Key</dt>
              <dd className="text-xs">{formData.idempotencyKey}</dd>
            </dl>
            <Alert>
              <AlertDescription>
                This action cannot be undone. Transfers can be reversed within 30 minutes.
              </AlertDescription>
            </Alert>
            <div className="flex gap-4">
              <Button
                onClick={handleConfirm}
                disabled={transferMutation.isPending}
                className="flex-1"
              >
                {transferMutation.isPending ? 'Processing...' : 'Confirm Transfer'}
              </Button>
              <Button variant="outline" onClick={handleCancel} className="flex-1">
                Cancel
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {step === 'receipt' && (
        <Card>
          <CardHeader>
            <CardTitle>Transfer Successful</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex items-center gap-2 text-success">
              <CheckCircle className="h-6 w-6" />
              <span className="font-medium">Transaction completed</span>
            </div>
            <dl className="grid grid-cols-2 gap-2 text-sm">
              <dt className="text-muted-foreground">Transaction ID</dt>
              <dd className="font-mono text-xs">{transactionId}</dd>
              {formData && (
                <>
                  <dt className="text-muted-foreground">Amount</dt>
                  <dd>NGN {formData.amount.toFixed(2)}</dd>
                  <dt className="text-muted-foreground">To</dt>
                  <dd className="font-mono text-xs">{formData.toWalletId}</dd>
                </>
              )}
              {isReversible && (
                <div className="col-span-2 mt-2">
                  <Alert variant="default" className="border-amber-500 text-amber-700">
                    <AlertDescription>
                      This transfer can be reversed within the next 30 minutes.
                    </AlertDescription>
                  </Alert>
                </div>
              )}
            </dl>
            <Button
              onClick={() => {
                setStep('form');
                reset();
              }}
              className="w-full"
            >
              New Transfer
            </Button>
          </CardContent>
        </Card>
      )}
    </div>
  );
}