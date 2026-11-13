import type { AuthResponse } from '@/api/types';

const ACCESS_TOKEN_KEY = 'servora.accessToken';
const REFRESH_TOKEN_KEY = 'servora.refreshToken';
const ACCESS_EXPIRES_KEY = 'servora.accessExpiresAt';
const REFRESH_EXPIRES_KEY = 'servora.refreshExpiresAt';

export type StoredAuth = {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  refreshExpiresAt: string;
};

export function loadAuth(): StoredAuth | null {
  const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);
  const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
  const expiresAt = localStorage.getItem(ACCESS_EXPIRES_KEY);
  const refreshExpiresAt = localStorage.getItem(REFRESH_EXPIRES_KEY);

  if (!accessToken || !refreshToken || !expiresAt || !refreshExpiresAt) {
    return null;
  }

  return {
    accessToken,
    refreshToken,
    expiresAt,
    refreshExpiresAt,
  };
}

export function saveAuth(auth: AuthResponse): void {
  localStorage.setItem(ACCESS_TOKEN_KEY, auth.accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, auth.refreshToken);
  localStorage.setItem(ACCESS_EXPIRES_KEY, auth.expiresAt);
  localStorage.setItem(REFRESH_EXPIRES_KEY, auth.refreshExpiresAt);
}

export function clearAuth(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(ACCESS_EXPIRES_KEY);
  localStorage.removeItem(REFRESH_EXPIRES_KEY);
}
