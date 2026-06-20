import { useEffect, useState } from 'react';
import { useSearchParams, Link } from 'react-router-dom';
import { verifyEmail } from '@/api/auth';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { CheckCircle, XCircle, Loader2 } from 'lucide-react';

export default function VerifyEmailResultPage() {
  const [searchParams] = useSearchParams();
  const userId = searchParams.get('userId');
  const token = searchParams.get('token');
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('');

  useEffect(() => {
    if (!userId || !token) {
      setStatus('error');
      setMessage('Invalid verification link.');
      return;
    }
    verifyEmail(userId, token)
      .then(() => {
        setStatus('success');
        setMessage('Email verified successfully!');
      })
      .catch((err) => {
        setStatus('error');
        setMessage(err.response?.data?.error || 'Verification failed. The link may have expired.');
      });
  }, [userId, token]);

  const icon = status === 'loading' ? <Loader2 className="h-12 w-12 animate-spin" /> :
               status === 'success' ? <CheckCircle className="h-12 w-12 text-success" /> :
               <XCircle className="h-12 w-12 text-destructive" />;

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <Card className="w-full max-w-md text-center">
        <CardHeader>
          <CardTitle className="flex items-center justify-center gap-2">{icon}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-lg">{message}</p>
          {status === 'success' && (
            <Link
              to="/login"
              className="inline-flex items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground hover:bg-primary/90"
            >
              Proceed to Login
            </Link>
          )}
        </CardContent>
      </Card>
    </div>
  );
}