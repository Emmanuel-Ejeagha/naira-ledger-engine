import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuthStore } from '@/stores/authStore';

const useSignalR = (onNotification: (message: string) => void) => {
  const token = useAuthStore((s) => s.accessToken);
  const [isConnected, setIsConnected] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!token) {
      // If no token, disconnect any existing connection
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
        setIsConnected(false);
      }
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_HUB_URL || '/hubs/notifications'}?access_token=${token}`)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]) // retry delays in ms
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on('ReceiveNotification', (message: string) => {
      onNotification(message);
    });

    connection.onreconnecting(() => setIsConnected(false));
    connection.onreconnected(() => setIsConnected(true));
    connection.onclose(() => setIsConnected(false));

    // Start connection
    connection
      .start()
      .then(() => {
        setIsConnected(true);
      })
      .catch((err) => {
        console.error('SignalR connection error:', err);
        // Do not toast; it will automatically retry.
      });

    connectionRef.current = connection;

    return () => {
      connection.stop();
      connectionRef.current = null;
      setIsConnected(false);
    };
  }, [token, onNotification]);

  return { isConnected, connection: connectionRef };
};

export default useSignalR;