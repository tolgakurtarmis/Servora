import type { ApiError } from './types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

export type RequestOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
  body?: unknown;
  token?: string | null;
  signal?: AbortSignal;
};

export class ApiRequestError extends Error {
  readonly status: number;
  readonly details?: ApiError;

  constructor(message: string, status: number, details?: ApiError) {
    super(message);
    this.name = 'ApiRequestError';
    this.status = status;
    this.details = details;
  }
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const headers = new Headers({
    Accept: 'application/json',
  });

  if (options.body !== undefined) {
    headers.set('Content-Type', 'application/json');
  }

  if (options.token) {
    headers.set('Authorization', `Bearer ${options.token}`);
  }

  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method: options.method ?? 'GET',
      headers,
      body: options.body === undefined ? undefined : JSON.stringify(options.body),
      signal: options.signal,
    });
  } catch {
    throw new ApiRequestError('API’ye bağlanılamadı. Servisin çalıştığından emin ol.', 0);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const contentType = response.headers.get('content-type') ?? '';
  const isJson = contentType.includes('application/json');
  const payload = isJson ? ((await response.json()) as unknown) : undefined;

  if (!response.ok) {
    const problem = payload as (ApiError & { errors?: Record<string, string[]> }) | undefined;
    const validationMessage = problem?.errors
      ? Object.values(problem.errors).flat().find(Boolean)
      : undefined;
    const message =
      validationMessage ??
      problem?.detail ??
      problem?.title ??
      `Request failed with status ${response.status}`;

    throw new ApiRequestError(message, response.status, problem);
  }

  return payload as T;
}

export function getApiBaseUrl(): string {
  return API_BASE_URL;
}
