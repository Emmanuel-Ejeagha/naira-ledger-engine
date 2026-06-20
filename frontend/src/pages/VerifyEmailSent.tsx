import { useState, useEffect, useRef } from 'react';
import { useLocation, Link } from 'react-router-dom';
import { sendVerificationEmail } from '@/api/auth';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { MailCheck, Loader2, ArrowLeft } from 'lucide-react';
import { toast } from 'sonner';

const COOLDOWN_SECONDS = 30;

function maskEmail(email: string) {
  const [local, domain] = email.split('@');
  if (!local || !domain) return email;
  const first = local.charAt(0);
  const last = local.charAt(local.length - 1);
  const masked = first + '*'.repeat(Math.max(local.length - 2, 1)) + last;
  return `${masked}@${domain}`;
}

export default function VerifyEmailSentPage() {
  const location = useLocation();
  const email = (location.state?.email as string) || '';
  const serverMessage = (location.state?.message as string) || '';
  const [sending, setSending] = useState(false);
  const [cooldown, setCooldown] = useState(0);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    if (cooldown > 0) {
      intervalRef.current = setInterval(() => {
        setCooldown((prev) => {
          if (prev <= 1) {
            clearInterval(intervalRef.current!);
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    }
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, [cooldown]);

  const handleResend = async () => {
    if (!email || cooldown > 0) return;
    setSending(true);
    try {
      await sendVerificationEmail(email);
      toast.success('Verification email resent!');
      setCooldown(COOLDOWN_SECONDS);
    } catch (err: any) {
      toast.error(err.response?.data?.error || 'Failed to resend email');
    } finally {
      setSending(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <Card className="w-full max-w-md text-center">
        <CardHeader>
          <CardTitle className="flex items-center justify-center gap-2">
            <MailCheck className="h-6 w-6 text-success" /> Check your email
          </CardTitle>
          <CardDescription>
            {serverMessage && <p className="mb-2">{serverMessage}</p>}
            <p>
              We sent a verification link to{' '}
              <strong>{email ? maskEmail(email) : 'your email'}</strong>. Click the link to verify your account.
            </p>
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">
            Didn't receive it? Check your spam folder or request a new one.
          </p>
          <Button
            onClick={handleResend}
            disabled={sending || cooldown > 0 || !email}
            className="w-full"
          >
            {sending ? (
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            ) : cooldown > 0 ? (
              `Resend available in ${cooldown}s`
            ) : null}
            {cooldown > 0 ? '' : sending ? '' : 'Resend Verification Email'}
          </Button>
          <p className="text-sm">
            <Link to="/login" className="inline-flex items-center gap-1 text-info hover:underline">
              <ArrowLeft className="h-3 w-3" /> Return to Login
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}