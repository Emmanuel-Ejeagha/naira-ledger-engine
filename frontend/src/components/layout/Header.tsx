import { Link, useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { Button } from '@/components/ui/button';
import { Bell, LogOut, User } from 'lucide-react';

export default function Header() {
  const user = useAuthStore((s) => s.user);
  const token = useAuthStore((s) => s.accessToken);
  const navigate = useNavigate();

  const handleLogout = () => {
    useAuthStore.getState().logout();
    navigate('/login');
  };

  if (!token) {
    return null;
  }

  return (
    <header className="h-16 border-b border-border bg-card flex items-center justify-between px-6">
      <div className="flex items-center gap-4">
        {/* Mobile sidebar trigger can be added here */}
      </div>
      <div className="flex items-center gap-4">
        <Link to="/notifications" className="relative" aria-label="Notifications">
          <Bell className="h-5 w-5 text-muted-foreground hover:text-foreground transition-colors" />
        </Link>
        <div className="flex items-center gap-2">
          <User className="h-4 w-4 text-muted-foreground" />
          <span className="text-sm font-medium">{user?.email}</span>
        </div>
        <Button
          variant="ghost"
          size="sm"
          onClick={handleLogout}
          className="text-muted-foreground hover:text-foreground"
        >
          <LogOut className="h-4 w-4 mr-1" /> Logout
        </Button>
      </div>
    </header>
  );
}