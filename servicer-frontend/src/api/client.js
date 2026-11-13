const BASE_URL = (import.meta.env.VITE_API_URL || "http://localhost:5069").replace(/\/$/, "");
const TOKEN_KEY = "servicer_token";

export const tokenStore = {
  get: () => localStorage.getItem(TOKEN_KEY),
  set: (token) => localStorage.setItem(TOKEN_KEY, token),
  clear: () => localStorage.removeItem(TOKEN_KEY),
};

export class ApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

async function readError(response) {
  const text = await response.text();
  try {
    const data = JSON.parse(text);
    if (data.errors) return Object.values(data.errors).flat().join(" ");
    return data.message || data.title || data.detail || text;
  } catch {
    return text || `Request failed (${response.status})`;
  }
}

export async function api(path, { method = "GET", body } = {}) {
  const headers = { Accept: "application/json" };
  const token = tokenStore.get();
  if (token) headers.Authorization = `Bearer ${token}`;
  if (body !== undefined) headers["Content-Type"] = "application/json";

  let response;
  try {
    response = await fetch(`${BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    throw new ApiError("Cannot reach the API. Start the backend and check VITE_API_URL.", 0);
  }

  if (response.status === 401) {
    tokenStore.clear();
    window.dispatchEvent(new Event("servicer:unauthorized"));
    throw new ApiError("Session expired. Please log in again.", 401);
  }
  if (!response.ok) throw new ApiError(await readError(response), response.status);
  if (response.status === 204) return null;
  const text = await response.text();
  return text ? JSON.parse(text) : null;
}
