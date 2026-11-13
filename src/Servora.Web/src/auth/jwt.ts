import type { Role } from '@/api/types';

export type AccessTokenClaims = {
  userId: string | null;
  userName: string | null;
  role: Role | null;
  tenantId: string | null;
  branchId: string | null;
};

const KNOWN_ROLES = new Set<Role>([
  'PlatformAdmin',
  'Owner',
  'Manager',
  'Waiter',
  'Kitchen',
  'Cashier',
]);

function decodeBase64Url(value: string): string {
  const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
  const padded = normalized.padEnd(normalized.length + ((4 - (normalized.length % 4)) % 4), '=');
  return atob(padded);
}

function asRole(value: unknown): Role | null {
  if (typeof value !== 'string') {
    return null;
  }

  return KNOWN_ROLES.has(value as Role) ? (value as Role) : null;
}

function firstString(...values: unknown[]): string | null {
  for (const value of values) {
    if (typeof value === 'string' && value.length > 0) {
      return value;
    }
  }

  return null;
}

export function parseAccessTokenClaims(accessToken: string): AccessTokenClaims | null {
  try {
    const payloadPart = accessToken.split('.')[1];
    if (!payloadPart) {
      return null;
    }

    const payload = JSON.parse(decodeBase64Url(payloadPart)) as Record<string, unknown>;
    const roleClaim =
      payload.role ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    return {
      userId: firstString(payload.sub, payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
      userName: firstString(
        payload.unique_name,
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      ),
      role: asRole(Array.isArray(roleClaim) ? roleClaim[0] : roleClaim),
      tenantId: firstString(payload.tenant_id),
      branchId: firstString(payload.branch_id),
    };
  } catch {
    return null;
  }
}
