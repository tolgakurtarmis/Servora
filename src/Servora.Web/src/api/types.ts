export type Role =
  | 'PlatformAdmin'
  | 'Owner'
  | 'Manager'
  | 'Waiter'
  | 'Kitchen'
  | 'Cashier';

export type AuthResponse = {
  accessToken: string;
  expiresAt: string;
  refreshToken: string;
  refreshExpiresAt: string;
};

export type LoginRequest = {
  userName: string;
  password: string;
};

export type ApiError = {
  title?: string;
  detail?: string;
  status?: number;
};
