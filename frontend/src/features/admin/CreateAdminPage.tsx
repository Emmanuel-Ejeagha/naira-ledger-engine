import { useMutation } from '@tanstack/react-query';
import apiClient from '@/api/client';
import { useForm } from 'react-hook-form';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { toast } from 'sonner';

export default function CreateAdminPage() {
  const { register, handleSubmit, reset } = useForm();
  const mutation = useMutation({
    mutationFn: (data: any) => apiClient.post('/admin/users/create-admin', data),
    onSuccess: () => {
      toast.success('Admin created');
      reset();
    },
    onError: (err: any) => toast.error(err.response?.data?.error || 'Creation failed'),
  });

  return (
    <div className="max-w-md">
      <h1 className="text-2xl font-bold mb-4">Create Admin User</h1>
      <Card>
        <CardHeader><CardTitle>New Admin</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit((data) => mutation.mutate(data))} className="space-y-4">
            <div>
              <Label htmlFor="email">Email</Label>
              <Input id="email" type="email" {...register('email', { required: true })} />
            </div>
            <div>
              <Label htmlFor="fullName">Full Name</Label>
              <Input id="fullName" {...register('fullName', { required: true })} />
            </div>
            <div>
              <Label htmlFor="password">Password</Label>
              <Input id="password" type="password" {...register('password', { required: true, minLength: 8 })} />
            </div>
            <Button type="submit" disabled={mutation.isPending}>Create Admin</Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}