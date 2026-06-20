import { useState } from 'react';
import { useLocation, Link } from 'react-router-dom';
import { sendVerificationEmail } from '@/api/auth';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { MailCheck, Loader2, AlertTriangle } from 'lucide-react';
import { toast } from 'sonner';

export default function UnverifiedPage() {
  const location = useLocation();
  const email = (location.state?.email as string) || '';
  const [sending, setSending] = useState(false);

  const handleResend = async () => {
    if (!email) return;
    setSending(true);
    try {
      await sendVerificationEmail(email);
      toast.success('Verification email sent!');
    } catch (err: any) {
      toast.error(err.response?.data?.error || 'Failed to resend email');
    } finally {
      setSending(false);
    }
  };

  // Edge case: user navigated here directly without an email
  if (!email) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-background px-4">
        <Card className="w-full max-w-md text-center">
          <CardHeader>
            <CardTitle className="flex items-center justify-center gap-2">
              <AlertTriangle className="h-6 w-6 text-destructive" /> Missing Email Address
            </CardTitle>
            <CardDescription>
              We couldn't find your email address. Please log in again to continue.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link
              to="/login"
              className="inline-flex items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90"
            >
              Go to Login
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <Card className="w-full max-w-md text-center">
        <CardHeader>
          <CardTitle className="flex items-center justify-center gap-2">
            <MailCheck className="h-6 w-6 text-warning" /> Email Verification Required
          </CardTitle>
          <CardDescription>
            Your email address <strong>{email}</strong> has not been verified yet.
            Please check your inbox and click the verification link, or request a new one.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-muted-foreground">
            If you didn't receive the email, check your spam folder.
          </p>
          <Button onClick={handleResend} disabled={sending}>
            {sending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
            Resend Verification Email
          </Button>
          <p className="text-sm">
            Already verified?{' '}
            <Link to="/login" className="text-info hover:underline">
              Log in
            </Link>
          </p>
        </CardContent>
      </Card>
    </div>
  );
}