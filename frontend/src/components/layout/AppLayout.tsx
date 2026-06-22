import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import MobileSidebar from './MobileSidebar';
import Header from './Header';
import Footer from './Footer';
import { useSidebarStore } from '@/stores/sidebarStore';
import { useUserWallet } from '@/hooks/useUserWallet';

export default function AppLayout() {
  const isCollapsed = useSidebarStore((s) => s.isCollapsed);
  useUserWallet(); 

  return (
    <div className="flex h-screen">
      <Sidebar />
      <MobileSidebar />
      <div className="flex-1 flex flex-col overflow-hidden">
        <Header />
        <main className="flex-1 overflow-y-auto p-6 bg-background">
          <Outlet />
        </main>
        <Footer />
      </div>
    </div>
  );
}