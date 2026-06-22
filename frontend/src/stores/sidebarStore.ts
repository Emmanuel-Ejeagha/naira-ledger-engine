import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface SidebarState {
  isCollapsed: boolean;
  isMobileOpen: boolean;
  toggleCollapse: () => void;
  openMobile: () => void;
  closeMobile: () => void;
}

export const useSidebarStore = create<SidebarState>()(
  persist(
    (set) => ({
      isCollapsed: false,
      isMobileOpen: false,
      toggleCollapse: () => set((s) => ({ isCollapsed: !s.isCollapsed })),
      openMobile: () => set({ isMobileOpen: true }),
      closeMobile: () => set({ isMobileOpen: false }),
    }),
    {
      name: 'naira-sidebar',
      partialize: (state) => ({ isCollapsed: state.isCollapsed }), // persist only collapse state
    }
  )
);