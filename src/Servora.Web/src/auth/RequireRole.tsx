import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import type { Role } from '@/api/types';
import { useAuth } from '@/auth/AuthContext';

type RequireRoleProps = {
  roles: readonly Role[];
  children: ReactNode;
  fallbackTo?: string;
};

export function RequireRole({
  roles,
  children,
  fallbackTo = '/masa-takip',
}: RequireRoleProps) {
  const { isAuthenticated, role } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  if (!role || !roles.includes(role)) {
    return <Navigate to={fallbackTo} replace />;
  }

  return children;
}
