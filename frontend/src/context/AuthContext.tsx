// Built with help from Claude
import { createContext, useContext, useState, type ReactNode } from "react";
import { api, getToken, setToken } from "../services/api";
import type { AuthResponse } from "../types";

interface User {
  email: string;
  fullName: string;
}

interface AuthContextValue {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  register: (
    fullName: string,
    email: string,
    password: string,
  ) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);
const USER_KEY = "agritrack_user";

// On page load, restore the user if a token is still saved
function loadUser(): User | null {
  if (!getToken()) return null;
  try {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(loadUser);

  function save(response: AuthResponse) {
    const newUser = { email: response.email, fullName: response.fullName };
    setToken(response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(newUser));
    setUser(newUser);
  }

  async function login(email: string, password: string) {
    const response = await api<AuthResponse>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    save(response);
  }

  async function register(fullName: string, email: string, password: string) {
    const response = await api<AuthResponse>("/api/auth/register", {
      method: "POST",
      body: JSON.stringify({ fullName, email, password }),
    });
    save(response);
  }

  function logout() {
    setToken(null);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}
