import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { useEffect } from 'react';

export default function ProtectedRoute() {
  const { accessToken, isLoading, hydrate } = useAuthStore();
  const location = useLocation();

  useEffect(() => {
    hydrate();
  }, [hydrate]);

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="flex flex-col items-center gap-2">
          <div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
          <p className="text-sm text-neutral">Loading...</p>
        </div>
      </div>
    );
  }

  if (!accessToken) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
}