import { useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { useSidebarStore } from '@/stores/sidebarStore';
import {
  LayoutDashboard, Wallet, ArrowRightLeft, History, ShieldCheck,
  Bell, QrCode, Settings, ShieldAlert, LogOut, X,
} from 'lucide-react';
import { Button } from '@/components/ui/button';

const links = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/wallet', label: 'Wallet', icon: Wallet },
  { to: '/fund', label: 'Fund', icon: ArrowRightLeft },
  { to: '/transfer', label: 'Transfer', icon: ArrowRightLeft },
  { to: '/transactions', label: 'Transactions', icon: History },
  { to: '/kyc', label: 'KYC', icon: ShieldCheck },
  { to: '/notifications', label: 'Notifications', icon: Bell },
  { to: '/qr', label: 'QR Code', icon: QrCode },
  { to: '/settings', label: 'Settings', icon: Settings },
];

const adminLinks = [
  { to: '/admin/kyc', label: 'KYC Approval', icon: ShieldAlert },
  { to: '/admin/reversal', label: 'Reversal', icon: History },
];

export default function MobileSidebar() {
  const logout = useAuthStore((s) => s.logout);
  const isAdmin = useAuthStore((s) => s.roles.includes('Admin'));
  const { isMobileOpen, closeMobile } = useSidebarStore();
  const allLinks = [...links, ...(isAdmin ? adminLinks : [])];

  // Close on Escape key
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') closeMobile();
    };
    if (isMobileOpen) {
      document.addEventListener('keydown', handleKeyDown);
      document.body.style.overflow = 'hidden';
    }
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
      document.body.style.overflow = '';
    };
  }, [isMobileOpen, closeMobile]);

  if (!isMobileOpen) return null;

  return (
    <div className="fixed inset-0 z-50 md:hidden">
      {/* Overlay */}
      <div className="absolute inset-0 bg-black/50" onClick={closeMobile} />

      {/* Drawer */}
      <aside
        className="…"
        role="dialog"
        aria-modal="true"
        aria-label="Mobile navigation"
      >        
      {/* Header */}
        <div className="flex items-center justify-between mb-4">
          <span className="text-xl font-bold">NairaLedger</span>
          <Button variant="ghost" size="icon" className="text-white hover:bg-white/10" onClick={closeMobile} aria-label="Close menu">
            <X className="h-5 w-5" />
          </Button>
        </div>

        {/* Navigation */}
        <nav className="flex-1 space-y-1">
          {allLinks.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              onClick={closeMobile}
              className={({ isActive }) =>
                `flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                  isActive ? 'bg-white/10' : 'hover:bg-white/5'
                }`
              }
            >
              <link.icon className="h-4 w-4" />
              {link.label}
            </NavLink>
          ))}
        </nav>

        {/* Logout */}
        <Button
          variant="ghost"
          className="text-white hover:bg-white/10 justify-start gap-3 mt-4"
          onClick={() => { logout(); closeMobile(); }}
        >
          <LogOut className="h-4 w-4" />
          Logout
        </Button>
      </aside>
    </div>
  );
}