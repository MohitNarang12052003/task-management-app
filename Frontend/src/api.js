const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:8080";

async function request(path, options = {}) {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: { "Content-Type": "application/json", ...options.headers },
    ...options,
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: "Unknown error" }));
    throw new Error(err.message || `HTTP ${res.status}`);
  }

  if (res.status === 204) return null;
  return res.json();
}

export const taskApi = {
  getAll: () => request("/api/tasks"),
  getById: (id) => request(`/api/tasks/${id}`),
  create: (data) =>
    request("/api/tasks", { method: "POST", body: JSON.stringify(data) }),
  update: (id, data) =>
    request(`/api/tasks/${id}`, { method: "PUT", body: JSON.stringify(data) }),
  delete: (id) => request(`/api/tasks/${id}`, { method: "DELETE" }),
};
