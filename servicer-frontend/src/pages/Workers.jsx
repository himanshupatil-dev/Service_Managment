import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { appointmentApi, workerApi } from "../api/services";
const CATEGORIES = ["Plumber", "Electrician", "Welder", "Painter", "Mechanic"];
import { EmptyState, ErrorBox, Field, Modal, Spinner, Stars } from "../components/ui";

export default function Workers() {
  const [params, setParams] = useSearchParams();
  const [workers, setWorkers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selected, setSelected] = useState(null);

  const category = params.get("category") || "";
  const city = params.get("city") || "";
  const minRating = Number(params.get("minRating") || 0);

  useEffect(() => {
    let cancelled = false;
    setLoading(true);
    workerApi
      .search({ category, city, minRating })
      .then((data) => !cancelled && setWorkers(data))
      .catch((e) => !cancelled && setError(e.message))
      .finally(() => !cancelled && setLoading(false));
    return () => {
      cancelled = true;
    };
  }, [category, city, minRating]);

  const update = (key, value) => {
    const next = new URLSearchParams(params);
    value ? next.set(key, value) : next.delete(key);
    setParams(next, { replace: true });
  };

  return (
    <div className="space-y-8">
      <div className="rounded-[1.75rem] bg-aub-950 px-6 py-8 text-white sm:px-9 sm:py-10">
        <p className="text-xs font-bold uppercase tracking-[0.16em] text-gold-300">Your neighborhood, your experts</p>
        <div className="mt-3 flex flex-wrap items-end justify-between gap-4">
          <div><h1 className="text-4xl font-extrabold text-white sm:text-5xl">Find your person.</h1><p className="mt-3 max-w-xl text-sm leading-6 text-ice-200">Browse approved professionals and choose the right experience for your job.</p></div>
          {!loading && <span className="rounded-full border border-white/15 bg-white/10 px-3 py-1.5 text-sm font-semibold text-white">{workers.length} {workers.length === 1 ? "professional" : "professionals"}</span>}
        </div>
      </div>

      <div className="card grid gap-4 sm:grid-cols-3">
        <Field label="Category">
          <select className="input" value={category} onChange={(e) => update("category", e.target.value)}>
            <option value="">All</option>
            {CATEGORIES.map((c) => (
              <option key={c}>{c}</option>
            ))}
          </select>
        </Field>
        <Field label="City">
          <input className="input" placeholder="e.g. Pune" value={city} onChange={(e) => update("city", e.target.value)} />
        </Field>
        <Field label="Minimum rating">
          <select className="input" value={minRating} onChange={(e) => update("minRating", e.target.value)}>
            <option value={0}>Any</option>
            <option value={4}>4.0+</option>
            <option value={4.5}>4.5+</option>
          </select>
        </Field>
      </div>

      <ErrorBox message={error} />
      {loading ? (
        <Spinner />
      ) : workers.length === 0 ? (
        <EmptyState title="No workers match these filters" hint="Try a different category or city." />
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {workers.map((w) => (
            <div key={w.workerId} className="card group flex flex-col p-6 transition duration-200 hover:-translate-y-1 hover:border-aub-300 hover:shadow-[0_14px_30px_rgba(32,17,38,0.08)]">
              <div className="flex items-start justify-between gap-3">
                <div className="flex items-center gap-3">
                  <span className="grid h-12 w-12 place-items-center rounded-2xl bg-aub-50 font-display text-xl font-bold text-aub-800">{w.name?.trim()?.[0]?.toUpperCase() || "W"}</span>
                  <div><h3 className="font-bold">{w.name}</h3><p className="mt-0.5 text-sm text-slate-500">{w.category} · {w.city}</p></div>
                </div>
                <span className={`rounded-full px-2.5 py-1 text-[11px] font-bold ${w.isAvailable ? "bg-emerald-50 text-emerald-700" : "bg-ice-200 text-slate-600"}`}>{w.isAvailable ? "Available" : "Busy"}</span>
              </div>
              <div className="mt-5 flex items-center gap-2 border-y border-ice-200 py-3 text-sm">
                <span className="font-bold text-aub-900">{Number(w.rating || 0).toFixed(1)} <span className="text-gold-600">★</span></span>
                <span className="text-slate-400">·</span><span className="text-slate-500">{w.jobs} completed {w.jobs === 1 ? "job" : "jobs"}</span>
              </div>
              <p className="mt-4 min-h-12 flex-1 text-sm leading-6 text-slate-600">{w.bio || "Approved local professional ready to help."}</p>
              <button
                className="btn btn-primary mt-5 w-full"
                disabled={!w.isAvailable}
                onClick={() => setSelected(w)}
              >
                {w.isAvailable ? <>View & book <span className="transition group-hover:translate-x-1">→</span></> : "Currently unavailable"}
              </button>
            </div>
          ))}
        </div>
      )}

      {selected && <BookModal worker={selected} onClose={() => setSelected(null)} />}
    </div>
  );
}

function BookModal({ worker, onClose }) {
  const navigate = useNavigate();
  const [problemDescription, setProblem] = useState("");
  const [partsRequired, setParts] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (problemDescription.trim().length < 10) {
      setError("Please describe the problem in at least 10 characters.");
      return;
    }
    setLoading(true);
    setError("");
    try {
      await appointmentApi.book({
        workerId: worker.workerId,
        problemDescription: problemDescription.trim(),
        partsRequired,
      });
      navigate("/my-appointments");
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal title={`Book ${worker.name}`} onClose={onClose}>
      <form onSubmit={submit} className="space-y-4">
        <ErrorBox message={error} />
        <Field label="What needs fixing?">
          <textarea
            className="input"
            rows={4}
            value={problemDescription}
            onChange={(e) => setProblem(e.target.value)}
            placeholder="Kitchen tap is leaking from the base..."
          />
        </Field>
        <label className="flex items-center gap-2 text-sm">
          <input type="checkbox" checked={partsRequired} onChange={(e) => setParts(e.target.checked)} />
          Spare parts may be required
        </label>
        <div className="flex justify-end gap-2">
          <button type="button" className="btn btn-ghost" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" disabled={loading}>
            {loading ? "Booking..." : "Confirm booking"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
