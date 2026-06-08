import { useState, useCallback, useEffect } from 'react';
import useSignalR from '../dashboard/hooks/useSignalR';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Bell, Check, CheckCheck, Loader2 } from 'lucide-react';

interface Notification {
  id: string;
  message: string;
  timestamp: Date;
  isRead: boolean;
}

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>([]);

  const onNotification = useCallback((message: string) => {
    const newNotification: Notification = {
      id: crypto.randomUUID(),
      message,
      timestamp: new Date(),
      isRead: false,
    };
    setNotifications((prev) => [newNotification, ...prev]);
  }, []);

  const { current: connection } = useSignalR(onNotification);

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  const markAllRead = () => {
    setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
  };

  const markOneRead = (id: string) => {
    setNotifications((prev) => prev.map((n) => (n.id === id ? { ...n, isRead: true } : n)));
  };

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold flex items-center gap-2">
          <Bell className="h-6 w-6" /> Notifications
          {unreadCount > 0 && (
            <Badge className="ml-2 bg-danger text-white">{unreadCount} new</Badge>
          )}
        </h1>
        <div className="flex gap-2">
          {unreadCount > 0 && (
            <Button variant="outline" size="sm" onClick={markAllRead}>
              <CheckCheck className="h-4 w-4 mr-2" /> Mark all read
            </Button>
          )}
          <div className="text-sm text-neutral flex items-center gap-1">
            {connection?.state === 'Connected' ? (
              <span className="text-success">● Connected</span>
            ) : (
              <span className="text-warning flex items-center"><Loader2 className="animate-spin h-3 w-3 mr-1" /> Reconnecting</span>
            )}
          </div>
        </div>
      </div>

      {notifications.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center">
            <Bell className="h-8 w-8 mx-auto text-neutral mb-2" />
            <p className="text-neutral">No notifications yet.</p>
            <p className="text-sm text-neutral/60">Real‑time alerts about transfers and account activity will appear here.</p>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0">
            <ul className="divide-y divide-border">
              {notifications.map((notif) => (
                <li
                  key={notif.id}
                  className={`flex items-start gap-4 p-4 hover:bg-primary/5 cursor-pointer ${notif.isRead ? 'opacity-60' : ''}`}
                  onClick={() => markOneRead(notif.id)}
                >
                  <div className={`mt-1 h-2 w-2 rounded-full ${notif.isRead ? 'bg-border' : 'bg-info'}`} />
                  <div className="flex-1">
                    <p className="text-sm">{notif.message}</p>
                    <p className="text-xs text-neutral mt-1">
                      {notif.timestamp.toLocaleTimeString()} &bull; {notif.timestamp.toLocaleDateString()}
                    </p>
                  </div>
                  {!notif.isRead && (
                    <Button variant="ghost" size="icon" className="h-6 w-6" onClick={(e) => { e.stopPropagation(); markOneRead(notif.id); }}>
                      <Check className="h-3 w-3" />
                    </Button>
                  )}
                </li>
              ))}
            </ul>
          </CardContent>
        </Card>
      )}
    </div>
  );
}