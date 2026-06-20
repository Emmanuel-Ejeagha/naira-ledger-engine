import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { useEffect } from 'react';
import { Loader2 } from 'lucide-react';

export default function ProtectedRoute() {
  const { accessToken, emailConfirmed, isLoading, hydrate } = useAuthStore();
  const location = useLocation();

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  if (!accessToken) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (!emailConfirmed) {
    return <Navigate to="/unverified" replace />;
  }

  return <Outlet />;
}