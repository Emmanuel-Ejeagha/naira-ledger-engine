import { Outlet, Link } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { Button } from '@/components/ui/button';
import Footer from './Footer';

export default function PublicLayout() {
  const token = useAuthStore((s) => s.accessToken);

  return (
    <div className="min-h-screen flex flex-col bg-background">
      {/* Minimal Header */}
      <header className="border-b border-border">
        <div className="max-w-7xl mx-auto flex items-center justify-between px-6 py-3">
          <Link to="/" className="flex items-center gap-2">
            <div className="w-8 h-8 bg-primary rounded-md flex items-center justify-center">
              <span className="text-white font-bold text-sm">NL</span>
            </div>
            <span className="text-lg font-semibold">NairaLedger</span>
          </Link>
          <div className="flex items-center gap-3">
            {token ? (
              <>
                <Link to="/dashboard" className="text-sm font-medium hover:text-primary">Dashboard</Link>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => {
                    useAuthStore.getState().logout();
                    window.location.href = '/';
                  }}
                >
                  Logout
                </Button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-sm font-medium hover:text-primary">Login</Link>
                <Link to="/register">
                  <Button size="sm">Get Started</Button>
                </Link>
              </>
            )}
          </div>
        </div>
      </header>

      {/* Page content */}
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
}