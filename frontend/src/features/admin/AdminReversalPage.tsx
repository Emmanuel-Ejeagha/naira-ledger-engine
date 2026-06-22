import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { reverseTransaction } from '@/api/admin';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Loader2 } from 'lucide-react';
import { toast } from 'sonner';

export default function AdminReversalPage() {
  const [transactionId, setTransactionId] = useState('');
  const [showConfirm, setShowConfirm] = useState(false);

  const mutation = useMutation({
    mutationFn: (id: string) => reverseTransaction(id),
    onSuccess: (data: any) => {
      toast.success(`Transaction reversed: ${data.reversalTransactionId}`);
      setTransactionId('');
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Reversal failed');
    },
    onSettled: () => setShowConfirm(false),
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!transactionId.trim()) return;
    setShowConfirm(true);
  };

  return (
    <div className="max-w-md space-y-6">
      <h1 className="text-2xl font-bold">Reverse Transaction</h1>
      <Card>
        <CardHeader>
          <CardTitle>Enter Transaction ID</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="transactionId">Transaction ID</Label>
              <Input
                id="transactionId"
                placeholder="UUID of the transaction"
                value={transactionId}
                onChange={(e) => setTransactionId(e.target.value)}
                required
              />
            </div>
            <Button
              type="submit"
              className="w-full"
              disabled={!transactionId.trim()}
            >
              Reverse Transaction
            </Button>
          </form>
        </CardContent>
      </Card>

      {/* Confirmation Dialog */}
      <AlertDialog open={showConfirm} onOpenChange={setShowConfirm}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirm Reversal</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to reverse transaction <strong>{transactionId}</strong>? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={mutation.isPending}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={() => mutation.mutate(transactionId.trim())}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Reversing…
                </>
              ) : (
                'Yes, Reverse'
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}