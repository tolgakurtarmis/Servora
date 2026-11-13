import type { Role } from '@/api/types';

/** Roles that can open the Users management screen. */
export const USER_MANAGER_ROLES: readonly Role[] = [
  'PlatformAdmin',
  'Owner',
  'Manager',
];

export function canManageUsers(role: Role | null | undefined): boolean {
  return Boolean(role && USER_MANAGER_ROLES.includes(role));
}

export function getAssignableRoles(actorRole: Role | null | undefined): Role[] {
  switch (actorRole) {
    case 'PlatformAdmin':
      return ['PlatformAdmin', 'Owner', 'Manager', 'Waiter', 'Kitchen', 'Cashier'];
    case 'Owner':
      return ['Owner', 'Manager', 'Waiter', 'Kitchen', 'Cashier'];
    case 'Manager':
      return ['Manager', 'Waiter', 'Kitchen', 'Cashier'];
    default:
      return [];
  }
}

export const ROLE_LABELS: Record<Role, string> = {
  PlatformAdmin: 'Platform Yöneticisi',
  Owner: 'İşletme Sahibi',
  Manager: 'Şube Yöneticisi',
  Waiter: 'Garson',
  Kitchen: 'Mutfak',
  Cashier: 'Kasa',
};
