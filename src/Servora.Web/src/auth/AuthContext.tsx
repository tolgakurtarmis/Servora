import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from 'react';
import * as authApi from '@/api/authApi';
import type { AuthResponse, LoginRequest, Role } from '@/api/types';
import { parseAccessTokenClaims } from './jwt';
import { clearAuth, loadAuth, saveAuth, type StoredAuth } from './tokenStorage';

const REFRESH_SKEW_MS = 60_000;

type AuthContextValue = {
  auth: StoredAuth | null;
  isAuthenticated: boolean;
  role: Role | null;
  userId: string | null;
  tenantId: string | null;
  branchId: string | null;
  login: (request: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
  getAccessToken: () => Promise<string>;
  setAuth: (auth: AuthResponse | null) => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

type AuthProviderProps = {
  children: ReactNode;
};

function toStoredAuth(auth: AuthResponse): StoredAuth {
  return {
    accessToken: auth.accessToken,
    refreshToken: auth.refreshToken,
    expiresAt: auth.expiresAt,
    refreshExpiresAt: auth.refreshExpiresAt,
  };
}

function isExpired(expiresAt: string, skewMs = 0): boolean {
  const expiresMs = Date.parse(expiresAt);
  if (Number.isNaN(expiresMs)) {
    return true;
  }

  return expiresMs <= Date.now() + skewMs;
}

function claimsFromAuth(auth: StoredAuth | null) {
  if (!auth?.accessToken) {
    return {
      role: null as Role | null,
      userId: null as string | null,
      tenantId: null as string | null,
      branchId: null as string | null,
    };
  }

  const claims = parseAccessTokenClaims(auth.accessToken);
  return {
    role: claims?.role ?? null,
    userId: claims?.userId ?? null,
    tenantId: claims?.tenantId ?? null,
    branchId: claims?.branchId ?? null,
  };
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [auth, setAuthState] = useState<StoredAuth | null>(() => loadAuth());
  const authRef = useRef(auth);
  authRef.current = auth;
  const refreshPromiseRef = useRef<Promise<string> | null>(null);

  const setAuth = useCallback((next: AuthResponse | null) => {
    if (!next) {
      clearAuth();
      setAuthState(null);
      return;
    }

    saveAuth(next);
    setAuthState(toStoredAuth(next));
  }, []);

  const login = useCallback(
    async (request: LoginRequest) => {
      const response = await authApi.login(request);
      setAuth(response);
    },
    [setAuth],
  );

  const logout = useCallback(async () => {
    const refreshToken = authRef.current?.refreshToken;
    try {
      if (refreshToken) {
        await authApi.logout(refreshToken);
      }
    } finally {
      setAuth(null);
    }
  }, [setAuth]);

  const getAccessToken = useCallback(async () => {
    const current = authRef.current;
    if (!current?.accessToken || !current.refreshToken) {
      throw new Error('Bu işlem için giriş yapmalısın');
    }

    if (!isExpired(current.expiresAt, REFRESH_SKEW_MS)) {
      return current.accessToken;
    }

    if (isExpired(current.refreshExpiresAt)) {
      setAuth(null);
      throw new Error('Oturum süresi doldu. Tekrar giriş yap.');
    }

    if (!refreshPromiseRef.current) {
      refreshPromiseRef.current = authApi
        .refresh(current.refreshToken)
        .then((response) => {
          setAuth(response);
          return response.accessToken;
        })
        .catch((error) => {
          setAuth(null);
          throw error instanceof Error
            ? error
            : new Error('Oturum yenilenemedi. Tekrar giriş yap.');
        })
        .finally(() => {
          refreshPromiseRef.current = null;
        });
    }

    return refreshPromiseRef.current;
  }, [setAuth]);

  const claims = useMemo(() => claimsFromAuth(auth), [auth]);

  const value = useMemo<AuthContextValue>(
    () => ({
      auth,
      isAuthenticated: Boolean(auth?.accessToken),
      role: claims.role,
      userId: claims.userId,
      tenantId: claims.tenantId,
      branchId: claims.branchId,
      login,
      logout,
      getAccessToken,
      setAuth,
    }),
    [auth, claims, login, logout, getAccessToken, setAuth],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
