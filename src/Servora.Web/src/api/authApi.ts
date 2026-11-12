import { apiRequest } from './client';
import type { AuthResponse, LoginRequest } from './types';

export async function login(request: LoginRequest): Promise<AuthResponse> {
  return apiRequest<AuthResponse>('/auth/login', {
    method: 'POST',
    body: request,
  });
}

export async function refresh(refreshToken: string): Promise<AuthResponse> {
  return apiRequest<AuthResponse>('/auth/refresh', {
    method: 'POST',
    body: { refreshToken },
  });
}

export async function logout(refreshToken: string): Promise<void> {
  await apiRequest<void>('/auth/logout', {
    method: 'POST',
    body: { refreshToken },
  });
}
