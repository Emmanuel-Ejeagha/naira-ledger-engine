import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'sonner';
import ProtectedRoute from '@/components/layout/ProtectedRoute';
import AppLayout from '@/components/layout/AppLayout';
import LoginPage from '@/features/auth/Login';
import RegisterPage from '@/features/auth/Register';
import DashboardPage from '@/features/dashboard/DashboardPage';
import WalletPage from '@/features/wallet/WalletPage';
import FundingPage from './features/funding/FundingPage';
import TransferPage from './features/transfer/TransferPage';
import TransactionsPage from '@/features/transactions/TransactionsPage';
import KycPage from '@/features/kyc/KycPage';
import NotificationsPage from '@/features/notifications/NotificationsPage';
import QRPage from '@/features/qr/QRPage';
import SettingsPage from '@/features/settings/SettingsPage';


const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route element={<ProtectedRoute />}>
            <Route element={<AppLayout />}>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/wallet" element={<WalletPage />} />
              <Route path="/" element={<Navigate to="/dashboard" replace />} />
              <Route path="/fund" element={<FundingPage />} />
              <Route path="/transfer" element={<TransferPage />} />
              <Route path="/transactions" element={<TransactionsPage />} />
              <Route path="/kyc" element={<KycPage />} />
              <Route path="/notifications" element={<NotificationsPage />} />
              <Route path="/qr" element={<QRPage />} />
<Route path="/settings" element={<SettingsPage />} />
            </Route>
          </Route>
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </BrowserRouter>
      <Toaster position="top-right" richColors closeButton />
    </QueryClientProvider>
  );
}