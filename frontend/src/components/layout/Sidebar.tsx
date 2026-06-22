import { LogOut } from 'lucide-react';
import { NavLink } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';
import { useSidebarStore } from '@/stores/sidebarStore';
import {
  LayoutDashboard, Wallet, ArrowRightLeft, History, ShieldCheck,
  Bell, QrCode, Settings, ShieldAlert, ChevronLeft, ChevronRight,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import Tooltip from '@/components/ui/Tooltip';

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

export default function Sidebar() {
  const logout = useAuthStore((s) => s.logout);
  const isAdmin = useAuthStore((s) => s.roles.includes('Admin'));
  const { isCollapsed, toggleCollapse } = useSidebarStore();

  const allLinks = [...links, ...(isAdmin ? adminLinks : [])];

  return (
    <aside
      className={`hidden md:flex flex-col bg-primary text-white transition-all duration-300 ease-in-out ${
        isCollapsed ? 'w-[72px]' : 'w-60'
      }`}
    >
      {/* Logo area */}
      <div className="flex items-center justify-between p-4">
        {!isCollapsed && <span className="text-xl font-bold">NairaLedger</span>}
        <Button
          variant="ghost"
          size="icon"
          className="text-white hover:bg-white/10 ml-auto"
          onClick={toggleCollapse}
          aria-expanded={!isCollapsed}
          aria-label={isCollapsed ? "Expand sidebar" : "Collapse sidebar"}
        >
          {isCollapsed ? <ChevronRight className="h-4 w-4" /> : <ChevronLeft className="h-4 w-4" />}
        </Button>
      </div>

      {/* Navigation */}
      <nav className="flex-1 space-y-1 px-2" role="navigation" aria-label="Main navigation">
        {allLinks.map((link) => {
          const Icon = link.icon;
          return (
            <Tooltip key={link.to} content={link.label}>
              <NavLink
                to={link.to}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    isActive ? 'bg-white/10' : 'hover:bg-white/5'
                  }`
                }
              >
                <Icon className="h-4 w-4 shrink-0" />
                {!isCollapsed && <span>{link.label}</span>}
              </NavLink>
            </Tooltip>
          );
        })}
      </nav>

      {/* Logout */}
      <div className="p-2">
        <Tooltip content="Logout">
          <Button
            variant="ghost"
            className="w-full text-white hover:bg-white/10 justify-start gap-3"
            onClick={logout}
          >
            <LogOut className="h-4 w-4 shrink-0" />
            {!isCollapsed && 'Logout'}
          </Button>
        </Tooltip>
      </div>
    </aside>
  );
}