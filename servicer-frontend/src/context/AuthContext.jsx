import { createContext, useContext, useEffect, useState, useCallback } from "react";
import { authApi } from "../api/services";
import { tokenStore } from "../api/client";
import { TOKEN_CLAIMS } from "../api/endpoints";

const AuthContext = createContext(null);

function decodeToken(token) {
  try {
    const payload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
    return JSON.parse(decodeURIComponent(escape(atob(payload))));
  } catch {
    return null;
  }
}

const pick = (claims, keys) => {
  for (const k of keys) if (claims[k] !== undefined) return claims[k];
  return undefined;
};

function userFromToken(token) {
  const claims = decodeToken(token);
  if (!claims) return null;
  if (claims.exp && claims.exp * 1000 < Date.now()) return null;
  let role = pick(claims, TOKEN_CLAIMS.role);
  if (Array.isArray(role)) role = role[0];
  return {
    id: pick(claims, TOKEN_CLAIMS.userId),
    name: pick(claims, TOKEN_CLAIMS.name) || "User",
    role: role || "User", // "Admin" | "Worker" | "User"
  };
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const t = tokenStore.get();
    return t ? userFromToken(t) : null;
  });

  const logout = useCallback(() => {
    tokenStore.clear();
    setUser(null);
  }, []);

  useEffect(() => {
    window.addEventListener("servicer:unauthorized", logout);
    return () => window.removeEventListener("servicer:unauthorized", logout);
  }, [logout]);

  const login = async (email, password) => {
    const res = await authApi.login(email, password);
    const token = res?.token || res?.Token || res?.accessToken;
    if (!token) throw new Error("Login response did not contain a token.");
    tokenStore.set(token);
    const u = userFromToken(token);
    // Fall back to fields on the response body if the token has no role claim.
    if (u && res.role && u.role === "User") u.role = res.role;
    setUser(u);
    return u;
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);

export const homeFor = (user) =>
  user?.role === "Admin" ? "/admin" : user?.role === "Worker" ? "/worker" : "/workers";
