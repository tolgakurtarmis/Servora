import type { Role } from '@/api/types';
import { USER_MANAGER_ROLES } from '@/auth/roles';
import type { NavItem } from './types';

/**
 * App sidebar navigation.
 * - Item with `path` and no `children` → direct link
 * - Item with `children` → expandable group (optional own `path`)
 * - Optional `roles` → visible only for listed roles
 */
export const navItems: NavItem[] = [
  {
    id: 'masa-takip',
    label: 'Masa Takip',
    path: '/masa-takip',
  },
  {
    id: 'raporlar',
    label: 'Raporlar',
    children: [
      { id: 'raporlar-gunluk', label: 'Günlük Özet', path: '/raporlar/gunluk' },
      { id: 'raporlar-satis', label: 'Satış Raporu', path: '/raporlar/satis' },
    ],
  },
  {
    id: 'tanimlamalar',
    label: 'Tanımlamalar',
    children: [
      { id: 'tanim-masalar', label: 'Masalar', path: '/tanimlamalar/masalar' },
      { id: 'tanim-menu', label: 'Menü', path: '/tanimlamalar/menu' },
      {
        id: 'tanim-kullanicilar',
        label: 'Kullanıcılar',
        path: '/tanimlamalar/kullanicilar',
        roles: USER_MANAGER_ROLES,
      },
    ],
  },
];

export function isNavItemVisible(item: NavItem, role: Role | null): boolean {
  if (item.roles && item.roles.length > 0) {
    if (!role || !item.roles.includes(role)) {
      return false;
    }
  }

  if (item.children?.length) {
    return item.children.some((child) => isNavItemVisible(child, role));
  }

  return true;
}

export function filterNavItems(items: NavItem[], role: Role | null): NavItem[] {
  return items
    .map((item) => {
      if (!item.children?.length) {
        return isNavItemVisible(item, role) ? item : null;
      }

      const children = filterNavItems(item.children, role);
      if (children.length === 0 && !item.path) {
        return null;
      }

      if (item.roles && item.roles.length > 0 && (!role || !item.roles.includes(role))) {
        return null;
      }

      return { ...item, children };
    })
    .filter((item): item is NavItem => item !== null);
}

export function collectNavPaths(items: NavItem[]): string[] {
  const paths: string[] = [];

  for (const item of items) {
    if (item.path) {
      paths.push(item.path);
    }
    if (item.children?.length) {
      paths.push(...collectNavPaths(item.children));
    }
  }

  return paths;
}

export function isNavItemActive(item: NavItem, pathname: string): boolean {
  if (item.path && (pathname === item.path || pathname.startsWith(`${item.path}/`))) {
    return true;
  }

  return item.children?.some((child) => isNavItemActive(child, pathname)) ?? false;
}
