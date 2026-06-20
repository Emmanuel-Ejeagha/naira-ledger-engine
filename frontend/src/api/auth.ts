import apiClient from './client';

export const changePassword = (data: { currentPassword: string; newPassword: string }) =>
  apiClient.post('/auth/change-password', data);

export const sendVerificationEmail = (email: string) => apiClient.post('/auth/send-verification-email', { email });

export const verifyEmail = (userId: string, token: string) => apiClient.get('/auth/verify-email', { params: { userId, token } });

export const forgotPassword = (email: string) =>
  apiClient.post('/auth/forgot-password', { email });

export const resetPassword = (userId: string, token: string, newPassword: string, email: string) =>
  apiClient.post('/auth/reset-password', { userId, token, newPassword, email });