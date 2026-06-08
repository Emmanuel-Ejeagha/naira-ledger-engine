import { useState } from 'react';
import QRCode from 'qrcode';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { toast } from 'sonner';
import { Download, Share2, QrCode } from 'lucide-react';
import { useAuthStore } from '../../stores/authStore';

const qrSchema = z.object({
  amount: z.number().min(0, 'Amount must be positive').optional(),
});

type QRFormData = z.infer<typeof qrSchema>;

export default function QRPage() {
  const walletId = useAuthStore((s) => s.walletId);
  const [qrDataUrl, setQrDataUrl] = useState<string | null>(null);
  const [isGenerating, setIsGenerating] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<QRFormData>({
    resolver: zodResolver(qrSchema),
  });

  const generateQR = async (data: QRFormData) => {
    setIsGenerating(true);
    try {
      // Construct payment URL (can be a deep link or just wallet info)
      const paymentData = JSON.stringify({
        walletId,
        amount: data.amount || undefined,
        currency: 'NGN',
      });
      const qr = await QRCode.toDataURL(paymentData, { width: 300, margin: 2 });
      setQrDataUrl(qr);
    } catch (err) {
      toast.error('Failed to generate QR');
    } finally {
      setIsGenerating(false);
    }
  };

  const handleDownload = () => {
    if (!qrDataUrl) return;
    const link = document.createElement('a');
    link.href = qrDataUrl;
    link.download = 'naira-qr.png';
    link.click();
  };

  const handleShare = async () => {
    if (!qrDataUrl || !navigator.share) return;
    try {
      // Create a blob from the data URL
      const res = await fetch(qrDataUrl);
      const blob = await res.blob();
      const file = new File([blob], 'naira-qr.png', { type: 'image/png' });
      await navigator.share({
        title: 'NairaLedger Payment QR',
        text: `Scan to pay me via NairaLedger (NGN)`,
        files: [file],
      });
    } catch (err) {
      toast.error('Share failed');
    }
  };

  return (
    <div className="max-w-md space-y-6">
      <h1 className="text-2xl font-bold">QR Payment</h1>
      <p className="text-sm text-neutral">
        Generate a QR code to receive payments from other NairaLedger users.
      </p>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Generate QR</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(generateQR)} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="amount">Amount (optional, NGN)</Label>
              <Input
                id="amount"
                type="number"
                placeholder="Leave empty for no fixed amount"
                {...register('amount', { valueAsNumber: true })}
              />
              {errors.amount && <p className="text-sm text-danger">{errors.amount.message}</p>}
            </div>
            <Button type="submit" className="w-full" disabled={isGenerating}>
              {isGenerating ? 'Generating...' : 'Generate QR'}
            </Button>
          </form>
        </CardContent>
      </Card>

      {/* Display QR */}
      {isGenerating && <Skeleton className="h-64 w-64 mx-auto" />}

      {qrDataUrl && !isGenerating && (
        <Card>
          <CardContent className="pt-6 flex flex-col items-center space-y-4">
            <img src={qrDataUrl} alt="Payment QR" className="border rounded-lg" />
            <div className="flex gap-2">
              <Button variant="outline" size="sm" onClick={handleDownload}>
                <Download className="h-4 w-4 mr-2" /> Download
              </Button>
              {navigator.share && (
                <Button variant="outline" size="sm" onClick={handleShare}>
                  <Share2 className="h-4 w-4 mr-2" /> Share
                </Button>
              )}
            </div>
            <p className="text-xs text-neutral text-center">
              This QR contains your wallet ID and optional amount. Others can scan it to pay you.
            </p>
          </CardContent>
        </Card>
      )}
    </div>
  );
}