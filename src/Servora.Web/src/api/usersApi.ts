import { apiRequest } from './client';
import type {
  CreateUserRequest,
  CreateUserResponse,
  UpdateUserRequest,
  UserResponse,
} from './types';

export async function listUsers(
  token: string,
  tenantId?: string | null,
): Promise<UserResponse[]> {
  const query =
    tenantId && tenantId.length > 0
      ? `?tenantId=${encodeURIComponent(tenantId)}`
      : '';

  return apiRequest<UserResponse[]>(`/users${query}`, {
    method: 'GET',
    token,
  });
}

export async function createUser(
  token: string,
  request: CreateUserRequest,
): Promise<CreateUserResponse> {
  return apiRequest<CreateUserResponse>('/users', {
    method: 'POST',
    token,
    body: request,
  });
}

export async function updateUser(
  token: string,
  userId: string,
  request: UpdateUserRequest,
): Promise<string> {
  return apiRequest<string>(`/users/${userId}`, {
    method: 'PUT',
    token,
    body: request,
  });
}

export async function deleteUser(token: string, userId: string): Promise<string> {
  return apiRequest<string>(`/users/${userId}`, {
    method: 'DELETE',
    token,
  });
}
