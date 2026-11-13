import { useEffect } from "react";

const STATUS_STYLES = {
  Pending: "bg-amber-50 text-amber-800 ring-amber-200",
  Accepted: "bg-sky-50 text-sky-800 ring-sky-200",
  Rejected: "bg-slate-100 text-slate-600 ring-slate-200",
  InProgress: "bg-indigo-50 text-indigo-800 ring-indigo-200",
  AwaitingPayment: "bg-gold-100 text-aub-900 ring-gold-300",
  Completed: "bg-emerald-50 text-emerald-800 ring-emerald-200",
  Cancelled: "bg-slate-100 text-slate-600 ring-slate-200",
  CannotComplete: "bg-red-50 text-red-800 ring-red-200",
};

export function StatusBadge({ status }) {
  const label = String(status).replace(/([a-z])([A-Z])/g, "$1 $2");
  return (
    <span
      className={`inline-flex rounded-full px-2.5 py-1 text-xs font-bold ring-1 ring-inset ${
        STATUS_STYLES[status] || "bg-slate-100 text-slate-700 ring-slate-200"
      }`}
    >
      {label}
    </span>
  );
}

export function Spinner({ label = "Loading..." }) {
  return (
    <div className="flex items-center justify-center gap-3 py-12 text-slate-500">
      <span className="h-5 w-5 animate-spin rounded-full border-2 border-slate-300 border-t-brand-600" />
      {label}
    </div>
  );
}

export function ErrorBox({ message }) {
  if (!message) return null;
  return (
    <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm font-medium text-red-700">
      {message}
    </div>
  );
}

export function Field({ label, error, children }) {
  return (
    <label className="block space-y-1">
      <span className="text-sm font-medium text-slate-700">{label}</span>
      {children}
      {error && <span className="block text-xs text-red-600">{error}</span>}
    </label>
  );
}

export function Modal({ title, onClose, children }) {
  useEffect(() => {
    const onKey = (e) => e.key === "Escape" && onClose();
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onClose]);

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-aub-950/55 p-4 backdrop-blur-sm"
      onMouseDown={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="w-full max-w-md rounded-2xl border border-white/70 bg-white p-6 shadow-[0_24px_80px_rgba(32,17,38,0.25)]">
        <div className="mb-4 flex items-start justify-between">
          <h2 className="text-lg font-semibold">{title}</h2>
          <button onClick={onClose} className="grid h-8 w-8 place-items-center rounded-lg text-slate-400 transition hover:bg-ice-100 hover:text-aub-900" aria-label="Close">
            ✕
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}

export function Stars({ value, onChange }) {
  return (
    <div className="flex gap-1">
      {[1, 2, 3, 4, 5].map((n) => (
        <button
          key={n}
          type="button"
          disabled={!onChange}
          onClick={() => onChange?.(n)}
          className={`text-xl ${n <= value ? "text-amber-400" : "text-slate-300"} ${onChange ? "hover:text-amber-500" : "cursor-default"}`}
          aria-label={`${n} star`}
        >
          ★
        </button>
      ))}
    </div>
  );
}

export function EmptyState({ title, hint }) {
  return (
    <div className="card py-14 text-center">
      <span className="mx-auto grid h-14 w-14 place-items-center rounded-2xl bg-aub-50 font-display text-2xl font-bold text-aub-700">✦</span>
      <p className="mt-4 font-display text-lg font-bold text-aub-900">{title}</p>
      {hint && <p className="mx-auto mt-1 max-w-sm text-sm leading-6 text-slate-500">{hint}</p>}
    </div>
  );
}
