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
import { ShieldCheck, Clock, CheckCircle, Upload, AlertTriangle } from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';

const kycFormSchema = z.object({
  fullName: z.string().min(2, 'Full name is required'),
  idNumber: z.string().min(5, 'ID number is required'),
  idType: z.string().min(1, 'Please select an ID type'),
});

type KycFormData = z.infer<typeof kycFormSchema>;

export default function KycPage() {
  const walletId = localStorage.getItem('walletId') || '';
  const accessToken = useAuthStore((s) => s.accessToken);
  const queryClient = useQueryClient();

  const { data: kyc, isLoading, isError } = useQuery({
    queryKey: ['kyc', walletId],
    queryFn: () => getKycStatus(walletId, accessToken!),
    enabled: !!walletId && !!accessToken,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<KycFormData>({
    resolver: zodResolver(kycFormSchema),
    defaultValues: { fullName: '', idNumber: '', idType: '' },
  });

  const mutation = useMutation({
    mutationFn: (data: KycFormData) =>
      submitKyc({ walletId, fullName: data.fullName, idNumber: data.idNumber, idType: data.idType }),
    onSuccess: () => {
      toast.success('KYC submitted for review');
      queryClient.invalidateQueries({ queryKey: ['kyc', walletId] });
      reset();
    },
    onError: (err: any) => {
      toast.error(err.response?.data?.error || 'Submission failed');
    },
  });

  const getStatusBadge = (level: string | number) => {
    const levelStr = String(level);
    switch (levelStr) {
      case 'Tier1':
      case '1':
        return <Badge className="bg-warning/10 text-warning"><Clock className="h-3 w-3 mr-1" /> Pending Review</Badge>;
      case 'Tier2':
      case '2':
      case 'Tier3':
      case '3':
        return <Badge className="bg-success/10 text-success"><CheckCircle className="h-3 w-3 mr-1" /> Verified</Badge>;
      default:
        return <Badge className="bg-neutral/10 text-neutral"><AlertTriangle className="h-3 w-3 mr-1" /> Unverified</Badge>;
    }
  };

  if (isLoading || !accessToken) {
    return (
      <div className="space-y-4 max-w-lg">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-32 w-full" />
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="destructive" className="max-w-lg">
        <AlertDescription>Failed to load KYC status.</AlertDescription>
      </Alert>
    );
  }

  const isUnverified = kyc && (kyc.kycLevel === 'Unverified' || kyc.kycLevel === 0);

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
            <span className="text-sm text-muted-foreground">Current Level:</span>
            {kyc && getStatusBadge(kyc.kycLevel)}
          </div>
          {isUnverified && (
            <p className="mt-2 text-sm text-muted-foreground">
              Submit your KYC documents to upgrade your account and unlock higher limits.
            </p>
          )}
        </CardContent>
      </Card>

      {isUnverified && (
        <Card>
          <CardHeader>
            <CardTitle>Submit KYC Details</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit((data) => mutation.mutate(data))} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="fullName">Full Legal Name</Label>
                <Input id="fullName" placeholder="Your full name" {...register('fullName')} disabled={mutation.isPending} />
                {errors.fullName && <p className="text-sm text-destructive">{errors.fullName.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="idNumber">ID Number</Label>
                <Input id="idNumber" placeholder="National ID / Passport number" {...register('idNumber')} disabled={mutation.isPending} />
                {errors.idNumber && <p className="text-sm text-destructive">{errors.idNumber.message}</p>}
              </div>
              <div className="space-y-2">
                <Label htmlFor="idType">ID Type</Label>
                <select
                  id="idType"
                  className="w-full h-10 rounded-md border border-input px-3 text-sm"
                  {...register('idType')}
                  disabled={mutation.isPending}
                >
                  <option value="">Select ID type</option>
                  <option value="National ID">National ID</option>
                  <option value="Passport">Passport</option>
                  <option value="Driver's License">Driver's License</option>
                </select>
                {errors.idType && <p className="text-sm text-destructive">{errors.idType.message}</p>}
              </div>
              <Button type="submit" className="w-full" disabled={mutation.isPending}>
                {mutation.isPending ? 'Submitting...' : (
                  <>
                    <Upload className="h-4 w-4 mr-2" /> Submit for Approval
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