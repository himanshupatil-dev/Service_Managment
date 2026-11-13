import { useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { authApi } from "../api/services";
import { ErrorBox, Field } from "../components/ui";

const CATEGORIES = ["Plumber", "Electrician", "Welder", "Painter", "Mechanic"];
const EMPTY = {
  userName: "", email: "", password: "", phoneNumber: "", address: "",
  category: CATEGORIES[0], experienceYears: 0, qualification: "", license: "",
  pricingModel: "Fixed", price: "",
};

function validate(form, isWorker) {
  const errors = {};
  if (form.userName.trim().length < 2) errors.userName = "Enter your name (at least 2 characters).";
  if (!/^\S+@\S+\.\S+$/.test(form.email)) errors.email = "Enter a valid email.";
  if (form.password.length < 8) errors.password = "Use at least 8 characters.";
  if (!/^[6-9]\d{9}$/.test(form.phoneNumber)) errors.phoneNumber = "Enter a valid 10 digit Indian mobile number.";
  if (form.address.trim().length < 5) errors.address = "Enter an address (at least 5 characters).";
  if (isWorker) {
    if (form.qualification.trim().length < 2) errors.qualification = "Tell customers about your trade or qualification.";
    if (Number(form.experienceYears) < 0) errors.experienceYears = "Experience cannot be negative.";
    if (!(Number(form.price) > 0)) errors.price = "Enter a service price greater than zero.";
  }
  return errors;
}

export default function Register() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const [isWorker, setIsWorker] = useState(params.get("as") === "worker");
  const [form, setForm] = useState(EMPTY);
  const [errors, setErrors] = useState({});
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const set = (key) => (event) => setForm({ ...form, [key]: event.target.value });

  const submit = async (event) => {
    event.preventDefault();
    setError("");
    const validation = validate(form, isWorker);
    setErrors(validation);
    if (Object.keys(validation).length) return;

    setLoading(true);
    try {
      if (isWorker) await authApi.registerWorker(form);
      else await authApi.registerUser(form);
      navigate("/login", { replace: true });
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-lg">
      <form onSubmit={submit} className="card space-y-4" noValidate>
        <div><p className="eyebrow">Join your local community</p><h1 className="mt-2 text-3xl font-bold">Create your account</h1><p className="mt-2 text-sm text-slate-500">Choose how you’d like to use Servicer.</p></div>
        <div className="grid grid-cols-2 gap-1.5 rounded-xl border border-ice-200 bg-ice-100 p-1.5">
          {[[false, "I need a service"], [true, "I am a worker"]].map(([value, label]) => (
            <button key={label} type="button" onClick={() => setIsWorker(value)} className={`rounded-lg py-2.5 text-sm font-semibold transition ${isWorker === value ? "bg-white text-aub-900 shadow-sm" : "text-slate-500 hover:text-aub-900"}`}>
              {label}
            </button>
          ))}
        </div>
        <ErrorBox message={error} />
        <Field label="Full name" error={errors.userName}><input className="input" value={form.userName} onChange={set("userName")} /></Field>
        <Field label="Email" error={errors.email}><input className="input" type="email" value={form.email} onChange={set("email")} /></Field>
        <Field label="Password" error={errors.password}><input className="input" type="password" value={form.password} onChange={set("password")} /></Field>
        <Field label="Phone number" error={errors.phoneNumber}><input className="input" inputMode="numeric" maxLength={10} value={form.phoneNumber} onChange={set("phoneNumber")} /></Field>
        <Field label="Address" error={errors.address}><textarea className="input" rows={2} value={form.address} onChange={set("address")} /></Field>
        {isWorker && (
          <>
            <div className="grid grid-cols-2 gap-4">
              <Field label="Service category"><select className="input" value={form.category} onChange={set("category")}>{CATEGORIES.map((category) => <option key={category}>{category}</option>)}</select></Field>
              <Field label="Years of experience" error={errors.experienceYears}><input className="input" type="number" min="0" max="60" value={form.experienceYears} onChange={set("experienceYears")} /></Field>
            </div>
            <Field label="Trade or qualification" error={errors.qualification}><input className="input" value={form.qualification} onChange={set("qualification")} placeholder="e.g. Licensed electrician" /></Field>
            <Field label="License (optional)"><input className="input" value={form.license} onChange={set("license")} /></Field>
            <div className="grid grid-cols-2 gap-4">
              <Field label="Pricing model"><select className="input" value={form.pricingModel} onChange={set("pricingModel")}><option value="Fixed">Fixed price</option><option value="Hourly">Hourly</option></select></Field>
              <Field label="Price (₹)" error={errors.price}><input className="input" type="number" min="1" step="1" value={form.price} onChange={set("price")} /></Field>
            </div>
            <p className="text-xs text-slate-500">New worker profiles need admin approval before customers can find them.</p>
          </>
        )}
        <button className="btn btn-primary w-full" disabled={loading}>{loading ? "Creating account..." : "Sign up"}</button>
        <p className="text-center text-sm text-slate-600">Already registered? <Link to="/login" className="text-brand-600">Log in</Link></p>
      </form>
    </div>
  );
}
