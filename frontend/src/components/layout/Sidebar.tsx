import { NavLink } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { LogOut, LayoutDashboard, Wallet, ArrowRightLeft, History, ShieldCheck, Bell, QrCode, Settings } from 'lucide-react';
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

export default function Sidebar() {
  const logout = useAuthStore((s) => s.logout);

  return (
    <aside className="w-60 bg-primary text-white flex flex-col p-4">
      <div className="text-xl font-bold mb-8">NairaLedger</div>
      <nav className="flex-1 space-y-1">
        {links.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
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
      <Button
        variant="ghost"
        className="text-white hover:bg-white/10 justify-start gap-3 mt-4"
        onClick={logout}
      >
        <LogOut className="h-4 w-4" />
        Logout
      </Button>
    </aside>
  );
}