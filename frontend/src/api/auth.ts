import apiClient from './client';

export const changePassword = (data: { currentPassword: string; newPassword: string }) =>
  apiClient.post('/auth/change-password', data);

export const sendVerificationEmail = (email: string) => apiClient.post('/auth/send-verification-email', { email });

export const verifyEmail = (userId: string, token: string) => apiClient.get('/auth/verify-email', { params: { userId, token } });