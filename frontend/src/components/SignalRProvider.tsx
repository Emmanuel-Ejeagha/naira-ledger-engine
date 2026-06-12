import { createContext, useContext, useEffect, useRef, useState, ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import { toast } from 'sonner';
import { useAuthStore } from '@/stores/authStore';
import { useNotificationStore } from '@/stores/notificationStore';

interface SignalRContextType {
  isConnected: boolean;
  connection: signalR.HubConnection | null;
}

const SignalRContext = createContext<SignalRContextType>({ isConnected: false, connection: null });
export const useSignalRContext = () => useContext(SignalRContext);

export default function SignalRProvider({ children }: { children: ReactNode }) {
  const token = useAuthStore((s) => s.accessToken);
  const addNotification = useNotificationStore((s) => s.addNotification);
  const [isConnected, setIsConnected] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!token) {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
        setIsConnected(false);
      }
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_HUB_URL || '/hubs/notifications'}?access_token=${token}`, {
        transport: signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on('ReceiveNotification', (message: string) => {
      toast(message, {
      style: { color: 'var(--success)', fontWeight: 500 },
    });
      addNotification(message);
    });

    connection.onreconnecting(() => setIsConnected(false));
    connection.onreconnected(() => setIsConnected(true));
    connection.onclose(() => setIsConnected(false));

    connection.start()
      .then(() => setIsConnected(true))
      .catch(console.error);

    connectionRef.current = connection;

    return () => {
      connection.stop();
      connectionRef.current = null;
      setIsConnected(false);
    };
  }, [token, addNotification]);

  return (
    <SignalRContext.Provider value={{ isConnected, connection: connectionRef.current }}>
      {children}
    </SignalRContext.Provider>
  );
}