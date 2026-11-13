import { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth, homeFor } from "../context/AuthContext";
import { ErrorBox, Field } from "../components/ui";

export default function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const submit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const user = await login(form.email.trim(), form.password);
      navigate(location.state?.from?.pathname || homeFor(user), { replace: true });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto grid max-w-4xl overflow-hidden rounded-[1.75rem] border border-ice-200 bg-white shadow-[0_18px_50px_rgba(32,17,38,0.08)] md:grid-cols-[0.85fr_1.15fr]">
      <aside className="relative hidden overflow-hidden bg-aub-950 p-9 text-white md:flex md:flex-col md:justify-between lg:p-12">
        <div className="absolute -right-24 top-1/4 h-72 w-72 rounded-full border-[45px] border-white/[0.06]" />
        <div className="relative"><span className="grid h-11 w-11 place-items-center rounded-xl bg-gold-400 font-display text-xl font-extrabold text-aub-950">S</span><p className="mt-12 text-xs font-bold uppercase tracking-[0.16em] text-gold-300">Good to see you again</p><p className="mt-3 font-display text-4xl font-bold leading-tight text-white">Your next job<br />is right here.</p></div>
        <p className="relative text-sm leading-6 text-ice-200">Pick up where you left off. Your appointments and updates are waiting.</p>
      </aside>
      <form onSubmit={submit} className="space-y-5 p-6 sm:p-9 lg:p-12">
        <div><p className="eyebrow">Welcome back</p><h1 className="mt-2 text-3xl font-bold">Sign in to Servicer</h1><p className="mt-2 text-sm text-slate-500">Use your account details to continue.</p></div>
        <ErrorBox message={error} />
        <Field label="Email">
          <input className="input" type="email" required value={form.email} onChange={set("email")} />
        </Field>
        <Field label="Password">
          <input className="input" type="password" required value={form.password} onChange={set("password")} />
        </Field>
        <button className="btn btn-primary w-full" disabled={loading}>
          {loading ? "Logging in..." : "Log in"}
        </button>
        <p className="text-center text-sm text-slate-600">New here? <Link to="/register" className="font-bold text-brand-600 hover:text-aub-900">Create an account</Link></p>
      </form>
    </div>
  );
}
