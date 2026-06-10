import apiClient from './client';

export const changePassword = (data: { currentPassword: string; newPassword: string }) =>
  apiClient.post('/auth/change-password', data);