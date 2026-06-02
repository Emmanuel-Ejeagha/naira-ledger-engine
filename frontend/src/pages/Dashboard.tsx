import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import apiClient from '../api/client';

interface WalletSummary {
  walletId: string;
  balance: number;
  tag: string;
}

interface RecentTransaction {
  transactionId: string;
  reference: string;
  type: string;
  amount: number;
  status: string;
  createdAt: string;
}

const Dashboard = () => {
  const { user } = useAuth();
  const [wallet, setWallet] = useState<WalletSummary | null>(null);
  const [recentTx, setRecentTx] = useState<RecentTransaction[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Assume we have a user's wallet endpoint; since not yet, we'll use localStorage walletId
        const storedWalletId = localStorage.getItem('walletId');
        if (storedWalletId) {
          const [walletRes, txRes] = await Promise.all([
            apiClient.get(`/wallets/${storedWalletId}`),
            apiClient.get(`/transactions`, { params: { walletId: storedWalletId, pageSize: 5 } }),
          ]);
          setWallet({
            walletId: storedWalletId,
            balance: walletRes.data.balance ?? 0,
            tag: walletRes.data.tag ?? 'Main',
          });
          setRecentTx(txRes.data.items);
        }
      } catch (err) {
        console.error('Dashboard fetch error', err);
      }
    };
    fetchData();
  }, []);

  return (
    <div>
      <h2>Dashboard</h2>
      <p>Welcome back, {user?.fullName}!</p>
      {wallet && (
        <div className="balance-card">
          <h3>{wallet.tag} Wallet</h3>
          <p className="balance-amount">NGN {wallet.balance.toFixed(2)}</p>
          <Link to="/wallet">View Details</Link>
        </div>
      )}
      <div className="recent-activity">
        <h3>Recent Activity</h3>
        {recentTx.length === 0 ? (
          <p>No transactions yet.</p>
        ) : (
          <ul>
            {recentTx.map((tx) => (
              <li key={tx.transactionId}>
                {tx.type} – NGN {tx.amount} – {tx.status}
              </li>
            ))}
          </ul>
        )}
        <Link to="/transactions">See all transactions</Link>
      </div>
    </div>
  );
};

export default Dashboard;