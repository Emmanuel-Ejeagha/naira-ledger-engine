import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';

export default function AdminRoute() {
  const { user, roles, isLoading } = useAuthStore();

  if (isLoading) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  if (!user || !roles.includes('Admin')) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
}