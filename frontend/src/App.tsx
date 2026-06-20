import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from 'sonner';
import SignalRProvider from './components/SignalRProvider';
import PublicLayout from './components/layout/PublicLayout';
import AppLayout from './components/layout/AppLayout';
import ProtectedRoute from './components/layout/ProtectedRoute';
import AdminRoute from './components/layout/AdminRoute';
import LandingPage from './pages/LandingPage';
import LoginPage from './features/auth/Login';
import RegisterPage from './features/auth/Register';
import ForgotPasswordPage from './pages/ForgotPassword';
import ResetPasswordPage from './pages/ResetPassword';
import VerifyEmailSentPage from './pages/VerifyEmailSent';
import VerifyEmailResultPage from './pages/VerifyEmailResult';
import UnverifiedPage from './pages/Unverified';
import DashboardPage from './features/dashboard/DashboardPage';
import WalletPage from './features/wallet/WalletPage';
import FundingPage from './features/funding/FundingPage';
import TransferPage from './features/transfer/TransferPage';
import TransactionsPage from './features/transactions/TransactionsPage';
import KycPage from './features/kyc/KycPage';
import NotificationsPage from './features/notifications/NotificationsPage';
import QRPage from './features/qr/QRPage';
import SettingsPage from './features/settings/SettingsPage';
import KycApprovalPage from './features/admin/KycApprovalPage';
import AdminReversalPage from './features/admin/AdminReversalPage';
import CreateAdminPage from './features/admin/CreateAdminPage';

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <SignalRProvider>
          <Routes>
            {/* Public routes */}
            <Route element={<PublicLayout />}>
              <Route index element={<LandingPage />} />
              <Route path="login" element={<LoginPage />} />
              <Route path="register" element={<RegisterPage />} />
              <Route path="forgot-password" element={<ForgotPasswordPage />} />
              <Route path="reset-password" element={<ResetPasswordPage />} />
              <Route path="verify-email-sent" element={<VerifyEmailSentPage />} />
              <Route path="verify-email" element={<VerifyEmailResultPage />} />
              <Route path="unverified" element={<UnverifiedPage />} />
            </Route>

            {/* Protected routes */}
            <Route element={<ProtectedRoute />}>
              <Route element={<AppLayout />}>
                <Route path="dashboard" element={<DashboardPage />} />
                <Route path="wallet" element={<WalletPage />} />
                <Route path="fund" element={<FundingPage />} />
                <Route path="transfer" element={<TransferPage />} />
                <Route path="transactions" element={<TransactionsPage />} />
                <Route path="kyc" element={<KycPage />} />
                <Route path="notifications" element={<NotificationsPage />} />
                <Route path="qr" element={<QRPage />} />
                <Route path="settings" element={<SettingsPage />} />

                {/* Admin routes */}
                <Route element={<AdminRoute />}>
                  <Route path="admin/kyc" element={<KycApprovalPage />} />
                  <Route path="admin/reversal" element={<AdminReversalPage />} />
                  <Route path="admin/create-admin" element={<CreateAdminPage />} />
                </Route>
              </Route>
            </Route>

            {/* Fallback */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </SignalRProvider>
      </BrowserRouter>
      <Toaster position="top-right" richColors closeButton />
    </QueryClientProvider>
  );
}