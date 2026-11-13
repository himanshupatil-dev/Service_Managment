import { useCallback, useEffect, useState } from "react";
import { appointmentApi, workerApi } from "../api/services";
import { useAuth } from "../context/AuthContext";
import { EmptyState, ErrorBox, Field, Modal, Spinner, StatusBadge } from "../components/ui";
import PaymentQr from "../components/PaymentQr";

export default function WorkerDashboard() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [busyId, setBusyId] = useState(null);
  const [available, setAvailable] = useState(false);
  const [otpFor, setOtpFor] = useState(null);
  const [failFor, setFailFor] = useState(null);
  const [paymentQrFor, setPaymentQrFor] = useState(null);

  const load = useCallback(async () => {
    try {
      setItems((await appointmentApi.forWorker()) || []);
      setError("");
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    workerApi.me().then((profile) => setAvailable(profile.isAvailable)).catch((e) => setError(e.message));
    load();
    const t = setInterval(load, 15000);
    return () => clearInterval(t);
  }, [load]);

  const hasActive = items.some((a) => ["Accepted", "InProgress"].includes(a.status));

  const run = async (id, fn) => {
    setBusyId(id);
    setError("");
    try {
      await fn();
      await load();
    } catch (e) {
      setError(e.message);
    } finally {
      setBusyId(null);
    }
  };

  const complete = async (appointment) => {
    setBusyId(appointment.appointmentId);
    setError("");
    try {
      const completed = await appointmentApi.complete(appointment.appointmentId);
      await load();
      setPaymentQrFor(completed);
    } catch (e) {
      setError(e.message);
    } finally {
      setBusyId(null);
    }
  };

  const toggle = async () => {
    try {
      const next = !available;
      await workerApi.toggleAvailability(Number(user.id), next);
      setAvailable(next);
    } catch (e) {
      setError(e.message);
    }
  };

  if (loading) return <Spinner />;

  return (
    <div className="space-y-6">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-3xl font-bold">My jobs</h1>
        <button
          onClick={toggle}
          disabled={hasActive}
          title={hasActive ? "Finish your active job first" : ""}
          className={`btn ${available ? "btn-success" : "btn-ghost"}`}
        >
          <span className={`h-2 w-2 rounded-full ${available ? "bg-aub-900" : "bg-slate-400"}`} />
          {available ? "Available for jobs" : "Offline"}
        </button>
      </div>
      <p className="text-sm text-slate-500">
        You can hold one active job at a time. Go available to receive new requests.
      </p>

      <ErrorBox message={error} />

      {items.length === 0 ? (
        <EmptyState title="No jobs yet" hint="New requests will show up here." />
      ) : (
        <div className="space-y-4">
          {items.map((a) => (
            <div key={a.appointmentId} className="card space-y-3">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <h3 className="font-semibold">{a.userName || `Customer #${a.userId}`}</h3>
                  <p className="text-sm text-slate-600">{a.problemDescription}</p>
                  {a.partsRequired && <p className="text-xs text-slate-500">Spare parts may be required</p>}
                </div>
                <StatusBadge status={a.status} />
              </div>

              <div className="flex flex-wrap gap-2">
                {a.status === "Pending" && (
                  <>
                    <button className="btn btn-success" disabled={busyId === a.appointmentId} onClick={() => run(a.appointmentId, () => appointmentApi.accept(a.appointmentId))}>
                      Accept
                    </button>
                    <button className="btn btn-ghost" disabled={busyId === a.appointmentId} onClick={() => run(a.appointmentId, () => appointmentApi.reject(a.appointmentId))}>
                      Reject
                    </button>
                  </>
                )}
                {a.status === "Accepted" && (
                  <button className="btn btn-primary" onClick={() => setOtpFor(a)}>
                    Start job (enter OTP)
                  </button>
                )}
                {a.status === "InProgress" && (
                  <>
                    <button className="btn btn-success" disabled={busyId === a.appointmentId} onClick={() => complete(a)}>
                      Mark work finished
                    </button>
                    <button className="btn btn-danger" onClick={() => setFailFor(a)}>
                      Cannot complete
                    </button>
                  </>
                )}
                {a.status === "AwaitingPayment" && a.paymentStatus === "Pending" && (
                  <>
                    <span className="self-center rounded-lg bg-gold-100 px-3 py-2 text-sm text-aub-900"><strong>Payment confirmation request:</strong> customer says they paid {a.workerPrice?.toLocaleString("en-IN", { style: "currency", currency: "INR" })}. Confirm receipt to complete the job.</span>
                    <button className="btn btn-success" disabled={busyId === a.appointmentId} onClick={() => run(a.appointmentId, () => appointmentApi.confirmPayment(a.appointmentId))}>
                      {busyId === a.appointmentId ? "Confirming..." : "Confirm payment received"}
                    </button>
                  </>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {otpFor && (
        <OtpModal
          appointment={otpFor}
          onClose={() => setOtpFor(null)}
          onDone={() => {
            setOtpFor(null);
            load();
          }}
        />
      )}
      {paymentQrFor && (
        <Modal title="Work finished · payment request" onClose={() => setPaymentQrFor(null)}>
          <div className="space-y-4">
            <PaymentQr appointment={paymentQrFor} />
            <div className="flex justify-end">
              <button className="btn btn-ghost" onClick={() => setPaymentQrFor(null)}>Done</button>
            </div>
          </div>
        </Modal>
      )}
      {failFor && (
        <FailModal
          appointment={failFor}
          onClose={() => setFailFor(null)}
          onDone={() => {
            setFailFor(null);
            load();
          }}
        />
      )}
    </div>
  );
}

function OtpModal({ appointment, onClose, onDone }) {
  const [otp, setOtp] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (!/^\d{6}$/.test(otp)) {
      setError("Enter the 6 digit code from the customer.");
      return;
    }
    setLoading(true);
    setError("");
    try {
      await appointmentApi.start(appointment.appointmentId, otp);
      onDone();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal title="Start job" onClose={onClose}>
      <form onSubmit={submit} className="space-y-4">
        <p className="text-sm text-slate-600">Ask the customer for their start code once you have arrived.</p>
        <ErrorBox message={error} />
        <input
          className="input text-center font-display text-3xl font-bold tracking-[0.5em]"
          inputMode="numeric"
          maxLength={6}
          autoFocus
          value={otp}
          onChange={(e) => setOtp(e.target.value.replace(/\D/g, ""))}
          placeholder="••••••"
        />
        <div className="flex justify-end gap-2">
          <button type="button" className="btn btn-ghost" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" disabled={loading}>{loading ? "Verifying..." : "Verify and start"}</button>
        </div>
      </form>
    </Modal>
  );
}

function FailModal({ appointment, onClose, onDone }) {
  const [reason, setReason] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const submit = async (e) => {
    e.preventDefault();
    if (reason.trim().length < 5) {
      setError("Please give a short reason.");
      return;
    }
    setLoading(true);
    try {
      await appointmentApi.cannotComplete(appointment.appointmentId, reason.trim());
      onDone();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal title="Cannot complete this job" onClose={onClose}>
      <form onSubmit={submit} className="space-y-4">
        <ErrorBox message={error} />
        <Field label="Reason (e.g. item is unrepairable)">
          <textarea className="input" rows={3} value={reason} onChange={(e) => setReason(e.target.value)} />
        </Field>
        <div className="flex justify-end gap-2">
          <button type="button" className="btn btn-ghost" onClick={onClose}>Back</button>
          <button className="btn btn-danger" disabled={loading}>Submit</button>
        </div>
      </form>
    </Modal>
  );
}

