import { useNotificationStore } from '@/stores/notificationStore';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Bell, CheckCheck, Loader2 } from 'lucide-react';
import { useSignalRContext } from '../../components/SignalRProvider';


export default function NotificationsPage() {
  const { isConnected } = useSignalRContext();
  const { notifications, markAsRead, markAllAsRead } = useNotificationStore();
  const unreadCount = notifications.filter((n) => !n.isRead).length;

  return (
    <div className="space-y-6 max-w-2xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold flex items-center gap-2">
          <Bell className="h-6 w-6" /> Notifications
          {unreadCount > 0 && (
            <Badge className="ml-2 bg-destructive text-white">{unreadCount} new</Badge>
          )}
        </h1>
        <div className="flex gap-2">
          {unreadCount > 0 && (
            <Button variant="outline" size="sm" onClick={markAllAsRead}>
              <CheckCheck className="h-4 w-4 mr-2" /> Mark all read
            </Button>
          )}
          <span className="text-sm text-muted-foreground flex items-center gap-1">
            {isConnected ? (
              <span className="text-success">● Connected</span>
            ) : (
              <span className="text-warning flex items-center"><Loader2 className="animate-spin h-3 w-3 mr-1" /> Reconnecting</span>
            )}
          </span>
        </div>
      </div>

      {notifications.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center">
            <Bell className="h-8 w-8 mx-auto text-muted-foreground mb-2" />
            <p className="text-muted-foreground">No notifications yet.</p>
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-0">
            <ul className="divide-y divide-border">
              {notifications.map((notif) => (
                <li
                  key={notif.id}
                  className={`flex items-start gap-4 p-4 hover:bg-muted/50 cursor-pointer ${notif.isRead ? 'opacity-60' : ''}`}
                  onClick={() => markAsRead(notif.id)}
                >
                  <div className={`mt-1 h-2 w-2 rounded-full ${notif.isRead ? 'bg-border' : 'bg-info'}`} />
                  <div className="flex-1">
                    <p className="text-sm">{notif.message}</p>
                    <p className="text-xs text-muted-foreground mt-1">
                      {new Date(notif.timestamp).toLocaleString()}
                    </p>
                  </div>
                </li>
              ))}
            </ul>
          </CardContent>
        </Card>
      )}
    </div>
  );
}