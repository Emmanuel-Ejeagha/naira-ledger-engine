import { useAuthStore } from '@/stores/authStore';

export default function Header() {
  const user = useAuthStore((s) => s.user);
  return (
    <header className="h-16 border-b border-border bg-white flex items-center justify-end px-6">
      <span className="text-sm font-medium">{user?.email}</span>
    </header>
  );
}