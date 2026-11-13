import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '@/auth/AuthContext';
import { AppShell } from '@/layout/AppShell';

export function AuthenticatedLayout() {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return (
    <AppShell showSidebar>
      <Outlet />
    </AppShell>
  );
}
