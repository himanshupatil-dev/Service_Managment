import { useCallback, useEffect, useState } from "react";
import { adminApi } from "../api/services";
import { ErrorBox, Spinner } from "../components/ui";

export default function AdminDashboard() {
  const [data, setData] = useState(null);
  const [error, setError] = useState("");
  const load = useCallback(async () => {
    try { setData(await adminApi.overview()); setError(""); }
    catch (e) { setError(e.message); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const updateWorker = async (action) => {
    try { await action(); await load(); }
    catch (e) { setError(e.message); }
  };
  if (error && !data) return <ErrorBox message={error} />;
  if (!data) return <Spinner />;

  const stats = [["Users", data.totalUsers], ["Workers", data.totalWorkers], ["Active jobs", data.activeJobs], ["Open complaints", data.openComplaints]];
  return (
    <div className="space-y-8">
      <h1 className="text-3xl font-bold">Admin dashboard</h1>
      <p className="rounded-xl bg-gold-100 px-4 py-2 text-sm text-aub-900">Review new worker profiles and manage access to the marketplace.</p>
      <ErrorBox message={error} />
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        {stats.map(([label, value]) => <div key={label} className="card"><div className="text-sm text-slate-500">{label}</div><div className="mt-1 font-display text-4xl font-bold">{value}</div></div>)}
      </div>
      <section>
        <h2 className="mb-3 text-xl font-bold">Workers</h2>
        <div className="card overflow-x-auto p-0">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-ice-300 bg-ice-50 text-slate-500"><tr>
              <th className="px-4 py-3">Name</th><th className="px-4 py-3">Category</th><th className="px-4 py-3">Rating</th><th className="px-4 py-3">Access</th><th className="px-4 py-3">Approval</th><th className="px-4 py-3">Actions</th>
            </tr></thead>
            <tbody>{data.workers.map((w) => <tr key={w.workerId} className="border-b border-ice-200 last:border-0">
              <td className="px-4 py-3 font-medium">{w.name}</td><td className="px-4 py-3">{w.category}</td><td className="px-4 py-3">{w.rating}</td>
              <td className="px-4 py-3">{w.isBlocked ? "Blocked" : "Active"}</td><td className="px-4 py-3">{w.verificationStatus}</td>
              <td className="space-x-2 px-4 py-3 text-right">
                <button className={`btn ${w.isBlocked ? "btn-ghost" : "btn-danger"}`} onClick={() => updateWorker(() => adminApi.setBlocked(w.workerId, !w.isBlocked))}>{w.isBlocked ? "Unblock" : "Block"}</button>
                {w.verificationStatus !== "Verified" && <button className="btn btn-success" onClick={() => updateWorker(() => adminApi.setVerified(w.workerId, true))}>Approve</button>}
              </td>
            </tr>)}</tbody>
          </table>
        </div>
      </section>
      <section>
        <h2 className="mb-3 text-xl font-bold">Complaints</h2>
        <div className="space-y-3">{data.complaints.map((c) => <div key={c.complaintId} className="card flex items-start justify-between gap-4">
          <div><p className="text-sm">{c.text}</p><p className="text-xs text-slate-500">Appointment #{c.appointmentId}</p></div>
          <span className="rounded-full bg-gold-100 px-2.5 py-0.5 text-xs font-medium text-aub-900">{c.status}</span>
        </div>)}</div>
      </section>
    </div>
  );
}
