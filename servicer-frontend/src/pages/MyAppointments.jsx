import { useCallback, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { appointmentApi, paymentApi, ratingApi } from "../api/services";
import { EmptyState, ErrorBox, Field, Modal, Spinner, StatusBadge, Stars } from "../components/ui";
import PaymentQr from "../components/PaymentQr";
import { UPI_ID } from "../utils/upi";

const CANCELLABLE = ["Pending", "Accepted"];

export default function MyAppointments() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [busyId, setBusyId] = useState(null);
  const [payFor, setPayFor] = useState(null);
  const [rateFor, setRateFor] = useState(null);
  const load = useCallback(async () => {
    try { setItems((await appointmentApi.forUser()) || []); setError(""); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  }, []);
  useEffect(() => {
    load();
    const timer = setInterval(load, 15000);
    return () => clearInterval(timer);
  }, [load]);

  const cancel = async (id) => {
    if (!window.confirm("Cancel this appointment?")) return;
    setBusyId(id);
    try { await appointmentApi.cancel(id); await load(); }
    catch (e) { setError(e.message); }
    finally { setBusyId(null); }
  };
  if (loading) return <Spinner />;

  return <div className="space-y-6">
    <div className="flex items-center justify-between"><h1 className="text-3xl font-bold">My appointments</h1><Link to="/workers" className="btn btn-primary">New booking</Link></div>
    <ErrorBox message={error} />
    {items.length === 0 ? <EmptyState title="No appointments yet" hint="Find a worker and book your first service." /> : <div className="space-y-4">
      {items.map((a) => <div key={a.appointmentId} className="card space-y-3">
        <div className="flex items-start justify-between gap-4"><div>
          <h3 className="font-semibold">{a.workerName || `Worker #${a.workerId}`}</h3><p className="text-sm text-slate-600">{a.problemDescription}</p>
          {a.partsRequired && <p className="text-xs text-slate-500">Spare parts may be required</p>}
        </div><StatusBadge status={a.status} /></div>
        {a.status === "Accepted" && <div className="rounded-lg bg-gold-100 px-4 py-3 text-sm text-aub-900">
          Your worker accepted. Share this start code only when they arrive: <strong className="font-display text-lg tracking-widest">{a.startCode || "Check your messages for the code."}</strong>
        </div>}
        {a.status === "CannotComplete" && <div className="rounded-xl bg-red-50 px-4 py-3 text-sm text-red-700">The worker could not complete this job{a.failureReason ? `: ${a.failureReason}` : "."}</div>}
        <div className="flex flex-wrap items-center gap-2">
          {CANCELLABLE.includes(a.status) && <button className="btn btn-ghost" disabled={busyId === a.appointmentId} onClick={() => cancel(a.appointmentId)}>Cancel</button>}
          {a.status === "AwaitingPayment" && a.paymentStatus === "Pending" && <span className="text-sm text-gold-700">Payment submitted · waiting for worker confirmation</span>}
          {a.status === "AwaitingPayment" && a.paymentStatus !== "Pending" && <button className="btn btn-success" onClick={() => setPayFor(a)}>Pay now</button>}
          {a.paymentMethod && <span className="text-sm text-aub-700">Paid via {a.paymentMethod}</span>}
          {a.status === "Completed" && a.paymentMethod && !a.rating && <button className="btn btn-ghost" onClick={() => setRateFor(a)}>Rate worker</button>}
          {a.rating && <Stars value={a.rating} />}
        </div>
      </div>)}
    </div>}
    {payFor && <PayModal appointment={payFor} onClose={() => setPayFor(null)} onDone={async () => { setPayFor(null); await load(); }} />}
    {rateFor && <RateModal appointment={rateFor} onClose={() => setRateFor(null)} onDone={async () => { setRateFor(null); await load(); }} />}
  </div>;
}

function PayModal({ appointment, onClose, onDone }) {
  const [method, setMethod] = useState("UPI");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const pay = async () => {
    setLoading(true); setError("");
    try { await paymentApi.pay(appointment.appointmentId, method); await onDone(); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  };
  return <Modal title="Pay for this job" onClose={onClose}><div className="space-y-4">
    <ErrorBox message={error} />
    <div className="grid grid-cols-3 gap-2">{["UPI", "Wallet", "Cash"].map((option) => <button key={option} onClick={() => setMethod(option)} className={`rounded-lg border px-3 py-2 text-sm font-medium ${method === option ? "border-brand-500 bg-brand-50 text-brand-700" : "border-slate-300"}`}>{option}</button>)}</div>
    {method === "UPI" ? <>
      <PaymentQr appointment={appointment} />
      <p className="text-xs text-slate-500">After paying, submit the payment here. Your worker will confirm receipt before the appointment is marked complete. UPI transfers are not verified automatically.</p>
    </> : <p className="text-xs text-slate-500">Submit this payment method; your worker must confirm receipt before the job is marked complete.</p>}
    <div className="flex justify-end gap-2"><button className="btn btn-ghost" onClick={onClose}>Cancel</button><button className="btn btn-success" disabled={loading || (method === "UPI" && !UPI_ID)} onClick={pay}>{loading ? "Submitting..." : "I have paid"}</button></div>
  </div></Modal>;
}

function RateModal({ appointment, onClose, onDone }) {
  const [stars, setStars] = useState(5);
  const [comment, setComment] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const submit = async () => {
    setLoading(true); setError("");
    try { await ratingApi.rate(appointment.appointmentId, stars, comment); await onDone(); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  };
  return <Modal title="Rate your worker" onClose={onClose}><div className="space-y-4">
    <ErrorBox message={error} /><Stars value={stars} onChange={setStars} />
    <Field label="Comment (optional)"><textarea className="input" rows={3} value={comment} onChange={(e) => setComment(e.target.value)} /></Field>
    <div className="flex justify-end gap-2"><button className="btn btn-ghost" onClick={onClose}>Cancel</button><button className="btn btn-primary" disabled={loading} onClick={submit}>{loading ? "Submitting..." : "Submit"}</button></div>
  </div></Modal>;
}
