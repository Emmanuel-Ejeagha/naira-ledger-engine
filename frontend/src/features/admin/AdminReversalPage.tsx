import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { reverseTransaction } from '@/api/admin';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { toast } from 'sonner';

export default function AdminReversalPage() {
  const [transactionId, setTransactionId] = useState('');

  const mutation = useMutation({
    mutationFn: (id: string) => reverseTransaction(id),
    onSuccess: (data: any) => {
      toast.success(`Transaction reversed: ${data.reversalTransactionId}`);
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Reversal failed');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!transactionId.trim()) return;
    mutation.mutate(transactionId.trim());
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
            <Button type="submit" className="w-full" disabled={mutation.isPending}>
              {mutation.isPending ? 'Reversing...' : 'Reverse Transaction'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}