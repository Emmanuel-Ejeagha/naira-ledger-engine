import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'sonner';
import LoginPage from './features/auth/Login';
import RegisterPage from './features/auth/Register';
import ProtectedRoute from './components/layout/ProtectedRoute';
import AppLayout from './components/layout/AppLayout';
import DashboardPage from './features/dashboard/DashboardPage';
import AdminRoute from './components/layout/AdminRoute';
import AdminReversalPage from './features/admin/AdminReversalPage';
import CreateAdminPage from './features/admin/CreateAdminPage';
import KycApprovalPage from './features/admin/KycApprovalPage';
import FundingPage from './features/funding/FundingPage';
import KycPage from './features/kyc/KycPage';
import NotificationsPage from './features/notifications/NotificationsPage';
import QRPage from './features/qr/QRPage';
import SettingsPage from './features/settings/SettingsPage';
import TransactionsPage from './features/transactions/TransactionsPage';
import TransferPage from './features/transfer/TransferPage';
import WalletPage from './features/wallet/WalletPage';
import SignalRProvider from './components/SignalRProvider';


const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <SignalRProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* All protected routes share this wrapper */}
          <Route element={<ProtectedRoute />}>
            <Route element={<AppLayout />}>
              {/* Regular user routes */}
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/wallet" element={<WalletPage />} />
              <Route path="/fund" element={<FundingPage />} />
              <Route path="/transfer" element={<TransferPage />} />
              <Route path="/transactions" element={<TransactionsPage />} />
              <Route path="/kyc" element={<KycPage />} />
              <Route path="/notifications" element={<NotificationsPage />} />
              <Route path="/qr" element={<QRPage />} />
              <Route path="/settings" element={<SettingsPage />} />

              {/* Admin routes wrapped in AdminRoute */}
              <Route element={<AdminRoute children={undefined} />}>
                <Route path="/admin/kyc" element={<KycApprovalPage />} />
                <Route path="/admin/reversal" element={<AdminReversalPage />} />
                <Route path="/admin/create-admin" element={<CreateAdminPage />} />
              </Route>
            </Route>
          </Route>

          {/* Fallback */}
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
        </SignalRProvider>
      </BrowserRouter>
      <Toaster position="top-right" richColors closeButton />
    </QueryClientProvider>
  );
}