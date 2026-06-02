import { NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Sidebar = () => {
  const { user, logout } = useAuth();

  return (
    <aside className="sidebar">
      <div className="sidebar-brand">NairaLedger</div>
      <nav className="sidebar-nav">
        <NavLink to="/dashboard" className={({ isActive }) => isActive ? 'active' : ''}>Dashboard</NavLink>
        <NavLink to="/wallet" className={({ isActive }) => isActive ? 'active' : ''}>Wallet</NavLink>
        <NavLink to="/fund" className={({ isActive }) => isActive ? 'active' : ''}>Fund</NavLink>
        <NavLink to="/transfer" className={({ isActive }) => isActive ? 'active' : ''}>Transfer</NavLink>
        <NavLink to="/transactions" className={({ isActive }) => isActive ? 'active' : ''}>Transactions</NavLink>
        <NavLink to="/kyc" className={({ isActive }) => isActive ? 'active' : ''}>KYC</NavLink>
        <NavLink to="/notifications" className={({ isActive }) => isActive ? 'active' : ''}>Notifications</NavLink>
        <NavLink to="/profile" className={({ isActive }) => isActive ? 'active' : ''}>Profile</NavLink>
      </nav>
      <div className="sidebar-footer">
        <span>{user?.email}</span>
        <button onClick={logout} className="btn-logout">Logout</button>
      </div>
    </aside>
  );
};

export default Sidebar;