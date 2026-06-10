import { useState } from 'react';
import { useAuthStore } from '@/stores/authStore';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { toast } from 'sonner';
import { useMutation } from '@tanstack/react-query';
import apiClient from '@/api/client';

const profileSchema = z.object({
  fullName: z.string().min(2, 'Name is required'),
  email: z.string().email(),
});

const passwordSchema = z.object({
  currentPassword: z.string().min(8, 'Current password required'),
  newPassword: z.string().min(8, 'Must be at least 8 characters'),
  confirmNewPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmNewPassword, {
  message: 'Passwords do not match',
  path: ['confirmNewPassword'],
});

type ProfileFormData = z.infer<typeof profileSchema>;
type PasswordFormData = z.infer<typeof passwordSchema>;

export default function SettingsPage() {
  const { user } = useAuthStore();
  const [activeTab, setActiveTab] = useState<'profile' | 'security'>('profile');

  const profileForm = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      fullName: user?.fullName || '',
      email: user?.email || '',
    },
  });

  const passwordForm = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  });

  const updateProfileMutation = useMutation({
    mutationFn: (data: ProfileFormData) => apiClient.put('/auth/profile', data),
    onSuccess: () => toast.success('Profile updated'),
    onError: (err: any) => toast.error(err.response?.data?.error || 'Update failed'),
  });

  const changePasswordMutation = useMutation({
    mutationFn: (data: PasswordFormData) =>
      apiClient.post('/auth/change-password', {
        currentPassword: data.currentPassword,
        newPassword: data.newPassword,
      }),
    onSuccess: () => {
      toast.success('Password changed');
      passwordForm.reset();
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Change failed'),
  });

  const onProfileSubmit = (data: ProfileFormData) => updateProfileMutation.mutate(data);
  const onPasswordSubmit = (data: PasswordFormData) => changePasswordMutation.mutate(data);

  return (
    <div className="max-w-2xl space-y-6">
      <h1 className="text-2xl font-bold">Settings</h1>

      {/* Tab buttons */}
      <div className="flex gap-2 border-b pb-2">
        <button
          onClick={() => setActiveTab('profile')}
          className={`px-4 py-2 text-sm font-medium rounded-t-md ${
            activeTab === 'profile'
              ? 'border-b-2 border-primary text-primary bg-background'
              : 'text-muted-foreground hover:text-foreground'
          }`}
        >
          Profile
        </button>
        <button
          onClick={() => setActiveTab('security')}
          className={`px-4 py-2 text-sm font-medium rounded-t-md ${
            activeTab === 'security'
              ? 'border-b-2 border-primary text-primary bg-background'
              : 'text-muted-foreground hover:text-foreground'
          }`}
        >
          Security
        </button>
      </div>

      {/* Tab content */}
      {activeTab === 'profile' && (
        <Card>
          <CardHeader>
            <CardTitle>Profile Information</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={profileForm.handleSubmit(onProfileSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="fullName">Full Name</Label>
                <Input
                  id="fullName"
                  {...profileForm.register('fullName')}
                  disabled={updateProfileMutation.isPending}
                />
                {profileForm.formState.errors.fullName && (
                  <p className="text-sm text-destructive">{profileForm.formState.errors.fullName.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input id="email" value={user?.email} disabled />
                <p className="text-xs text-muted-foreground">Email cannot be changed.</p>
              </div>
              <Button type="submit" disabled={updateProfileMutation.isPending}>
                {updateProfileMutation.isPending ? 'Saving…' : 'Update Profile'}
              </Button>
            </form>
          </CardContent>
        </Card>
      )}

      {activeTab === 'security' && (
        <Card>
          <CardHeader>
            <CardTitle>Change Password</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={passwordForm.handleSubmit(onPasswordSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="currentPassword">Current Password</Label>
                <Input
                  id="currentPassword"
                  type="password"
                  {...passwordForm.register('currentPassword')}
                  disabled={changePasswordMutation.isPending}
                />
                {passwordForm.formState.errors.currentPassword && (
                  <p className="text-sm text-destructive">{passwordForm.formState.errors.currentPassword.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="newPassword">New Password</Label>
                <Input
                  id="newPassword"
                  type="password"
                  {...passwordForm.register('newPassword')}
                  disabled={changePasswordMutation.isPending}
                />
                {passwordForm.formState.errors.newPassword && (
                  <p className="text-sm text-destructive">{passwordForm.formState.errors.newPassword.message}</p>
                )}
              </div>
              <div className="space-y-2">
                <Label htmlFor="confirmNewPassword">Confirm New Password</Label>
                <Input
                  id="confirmNewPassword"
                  type="password"
                  {...passwordForm.register('confirmNewPassword')}
                  disabled={changePasswordMutation.isPending}
                />
                {passwordForm.formState.errors.confirmNewPassword && (
                  <p className="text-sm text-destructive">{passwordForm.formState.errors.confirmNewPassword.message}</p>
                )}
              </div>
              <Button type="submit" disabled={changePasswordMutation.isPending}>
                {changePasswordMutation.isPending ? 'Changing…' : 'Change Password'}
              </Button>
            </form>
          </CardContent>
        </Card>
      )}
    </div>
  );
}