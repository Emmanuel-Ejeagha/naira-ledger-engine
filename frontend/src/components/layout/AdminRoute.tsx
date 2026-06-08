import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';

export default function AdminRoute({ children }: { children: JSX.Element }) {
  const { user, roles, isLoading } = useAuthStore();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-neutral">Loading...</p>
      </div>
    );
  }

  if (!user || !roles.includes('Admin')) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
}