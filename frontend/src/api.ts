// Built with help from Claude
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5204'
const TOKEN_KEY = 'agritrack_token'

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN_KEY, token)
  else localStorage.removeItem(TOKEN_KEY)
}

export class ApiError extends Error {
  status: number
  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

// The backend returns errors in 3 shapes: {message}, a list of strings, or validation errors
function extractMessage(data: unknown, fallback: string): string {
  if (Array.isArray(data)) return data.join(' ')
  if (data && typeof data === 'object') {
    const d = data as { message?: string; errors?: Record<string, string[]>; title?: string }
    if (d.message) return d.message
    if (d.errors) return Object.values(d.errors).flat().join(' ')
    if (d.title) return d.title
  }
  return fallback
}

export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = getToken()

  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
  })

  // Token expired or invalid: forget it and go back to login
  if (response.status === 401 && token) {
    setToken(null)
    localStorage.removeItem('agritrack_user')
    window.location.href = '/login'
  }

  if (!response.ok) {
    const data = await response.json().catch(() => null)
    throw new ApiError(response.status, extractMessage(data, `Request failed (${response.status})`))
  }

  if (response.status === 204) return undefined as T
  return response.json()
}