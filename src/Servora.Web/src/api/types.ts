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

export type UserResponse = {
  id: string;
  name: string;
  userName: string;
  role: Role;
  tenantId: string | null;
  isActive: boolean;
};

export type CreateUserRequest = {
  name: string;
  userName: string;
  password: string;
  role: Role;
  tenantId?: string | null;
  branchId?: string | null;
};

export type CreateUserResponse = {
  id: string;
};

export type UpdateUserRequest = {
  name: string;
  isActive: boolean;
  role: Role;
  password?: string | null;
};

export type ApiError = {
  title?: string;
  detail?: string;
  status?: number;
};
