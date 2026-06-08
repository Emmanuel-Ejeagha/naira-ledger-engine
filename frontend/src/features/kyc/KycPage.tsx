import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getKycStatus, submitKyc } from '@/api/kyc';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { toast } from 'sonner';
import { ShieldCheck, AlertTriangle, CheckCircle, Clock, Upload } from 'lucide-react';
import { useAuthStore } from '../../stores/authStore';

const kycSchema = z.object({
  fullName: z.string().min(2, 'Full name is required'),
  idNumber: z.string().min(5, 'ID number is required'),
  idType: z.string().min(1, 'ID type is required'),
});

type KycFormData = z.infer<typeof kycSchema>;

export default function KycPage() {
  const walletId = useAuthStore((s) => s.walletId);
  const queryClient = useQueryClient();

  const { data: kyc, isLoading, isError } = useQuery({
    queryKey: ['kyc', walletId],
    queryFn: () => getKycStatus(walletId),
    enabled: !!walletId,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<KycFormData>({
    resolver: zodResolver(kycSchema),
  });

  const mutation = useMutation({
    mutationFn: (data: KycFormData) =>
      submitKyc({ walletId, ...data }),
    onSuccess: () => {
      toast.success('KYC submitted successfully');
      queryClient.invalidateQueries({ queryKey: ['kyc', walletId] });
      reset();
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Submission failed');
    },
  });

  const getStatusBadge = (level: string) => {
    switch (level) {
      case 'Tier1':
        return <Badge className="bg-warning/10 text-warning"><Clock className="h-3 w-3 mr-1" /> Tier 1 (Pending)</Badge>;
      case 'Tier2':
      case 'Tier3':
        return <Badge className="bg-success/10 text-success"><CheckCircle className="h-3 w-3 mr-1" /> Verified</Badge>;
      default:
        return <Badge className="bg-neutral/10 text-neutral"><AlertTriangle className="h-3 w-3 mr-1" /> Unverified</Badge>;
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="destructive">
        <AlertDescription>Failed to load KYC status.</AlertDescription>
      </Alert>
    );
  }

  return (
    <div className="space-y-6 max-w-lg">
      <h1 className="text-2xl font-bold">KYC Verification</h1>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <ShieldCheck className="h-5 w-5" /> Status
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center gap-3">
            <span className="text-sm text-neutral">Current Level:</span>
            {kyc && getStatusBadge(kyc.kycLevel)}
          </div>
          {kyc?.kycLevel === 'Unverified' && (
            <p className="mt-2 text-sm text-neutral">
              Submit your KYC documents to increase your transaction limits and unlock all features.
            </p>
          )}
        </CardContent>
      </Card>

      {kyc?.canSubmit && (
        <Card>
          <CardHeader>
            <CardTitle>Submit KYC Documents</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit((data) => mutation.mutate(data))} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="fullName">Full Name</Label>
                <Input id="fullName" placeholder="Your legal name" {...register('fullName')} />
                {errors.fullName && <p className="text-sm text-danger">{errors.fullName.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="idNumber">ID Number</Label>
                <Input id="idNumber" placeholder="e.g., passport number" {...register('idNumber')} />
                {errors.idNumber && <p className="text-sm text-danger">{errors.idNumber.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="idType">ID Type</Label>
                <select
                  id="idType"
                  className="w-full h-10 rounded-md border border-input px-3 text-sm"
                  {...register('idType')}
                >
                  <option value="">Select...</option>
                  <option value="National ID">National ID</option>
                  <option value="Passport">Passport</option>
                  <option value="Driver's License">Driver's License</option>
                </select>
                {errors.idType && <p className="text-sm text-danger">{errors.idType.message}</p>}
              </div>
              <Button type="submit" className="w-full" disabled={mutation.isPending}>
                {mutation.isPending ? 'Submitting...' : (
                  <>
                    <Upload className="h-4 w-4 mr-2" /> Submit
                  </>
                )}
              </Button>
            </form>
          </CardContent>
        </Card>
      )}
    </div>
  );
}