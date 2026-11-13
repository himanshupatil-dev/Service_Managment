import { Link } from "react-router-dom";
import { useAuth, homeFor } from "../context/AuthContext";

const SERVICES = [
  { name: "Plumbing", category: "Plumber", detail: "Leaks, taps & pipes", mark: "P", tone: "bg-sky-100 text-sky-800" },
  { name: "Electrical", category: "Electrician", detail: "Wiring, fans & fittings", mark: "E", tone: "bg-amber-100 text-amber-800" },
  { name: "Painting", category: "Painter", detail: "A fresh coat, done right", mark: "P", tone: "bg-rose-100 text-rose-800" },
  { name: "Mechanic", category: "Mechanic", detail: "Vehicle care at home", mark: "M", tone: "bg-violet-100 text-violet-800" },
  { name: "Welding", category: "Welder", detail: "Gates, grills & repairs", mark: "W", tone: "bg-emerald-100 text-emerald-800" },
];

const STEPS = [
  { title: "Find your expert", text: "Browse approved local workers by trade and rating." },
  { title: "Book with confidence", text: "Describe the job and follow each update in one place." },
  { title: "Start securely", text: "Share your private start code when your worker arrives." },
  { title: "Pay when it’s done", text: "Your worker confirms receipt before the job closes." },
];

export default function Home() {
  const { user } = useAuth();

  return (
    <div className="space-y-20 pb-12">
      <section className="fade-up relative isolate overflow-hidden rounded-[2rem] bg-aub-950 px-6 py-12 text-white shadow-[0_24px_60px_rgba(32,17,38,0.18)] sm:px-10 sm:py-16 lg:px-16 lg:py-20">
        <div className="surface-grid absolute inset-0 -z-10 opacity-[0.08]" />
        <div className="absolute -right-20 -top-36 -z-10 h-[28rem] w-[28rem] rounded-full bg-aub-700/50 blur-3xl" />
        <div className="absolute -bottom-48 left-1/3 -z-10 h-96 w-96 rounded-full bg-gold-500/10 blur-3xl" />
        <div className="grid items-center gap-12 lg:grid-cols-[1.05fr_0.95fr]">
          <div>
            <span className="inline-flex items-center gap-2 rounded-full border border-white/15 bg-white/[0.07] px-3.5 py-2 text-xs font-semibold tracking-wide text-ice-100">
              <span className="h-2 w-2 rounded-full bg-gold-400" /> LOCAL HELP. REAL PEOPLE.
            </span>
            <h1 className="mt-6 max-w-2xl font-display text-5xl font-extrabold leading-[0.98] tracking-[-0.055em] text-white sm:text-6xl lg:text-[4.5rem]">
              Your home,<br /><span className="text-gold-400">in good hands.</span>
            </h1>
            <p className="mt-6 max-w-lg text-base leading-7 text-ice-200 sm:text-lg">
              Find trusted local professionals for the jobs that keep life running. Book simply, track every step, and stay in control.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              {user ? <Link to={homeFor(user)} className="btn btn-primary !px-6">Open my dashboard <span>↗</span></Link> : <>
                <Link to="/register" className="btn btn-primary !px-6">Find a professional <span>↗</span></Link>
                <Link to="/register?as=worker" className="btn !border !border-white/20 !bg-white/5 !text-white hover:!bg-white/10">I’m a professional</Link>
              </>}
            </div>
            <div className="mt-10 flex flex-wrap items-center gap-x-6 gap-y-3 border-t border-white/10 pt-6 text-sm text-ice-200">
              <span className="inline-flex items-center gap-2"><span className="text-gold-400">✦</span> Admin-approved pros</span>
              <span className="inline-flex items-center gap-2"><span className="text-gold-400">✦</span> Secure start code</span>
              <span className="inline-flex items-center gap-2"><span className="text-gold-400">✦</span> Pay after the work</span>
            </div>
          </div>

          <div className="relative mx-auto w-full max-w-[27rem] py-5">
            <div className="absolute inset-8 rounded-[2.5rem] bg-gold-400/20 blur-3xl" />
            <div className="relative rounded-[1.75rem] border border-white/60 bg-ice-50 p-5 text-aub-950 shadow-2xl sm:p-7">
              <div className="flex items-start justify-between gap-4">
                <div><p className="text-xs font-bold uppercase tracking-[0.15em] text-aub-600">Your booking</p><h2 className="mt-1 text-2xl font-bold">Sorted, step by step.</h2></div>
                <span className="rounded-full bg-emerald-100 px-3 py-1.5 text-xs font-bold text-emerald-800">● Verified</span>
              </div>
              <div className="mt-7 flex items-center gap-4 rounded-2xl bg-white p-4 shadow-sm ring-1 ring-aub-900/5">
                <div className="grid h-14 w-14 shrink-0 place-items-center rounded-2xl bg-gold-100 font-display text-2xl font-bold text-aub-900">R</div>
                <div className="min-w-0 flex-1"><p className="font-bold">Ravi Sharma</p><p className="mt-0.5 text-sm text-slate-500">Electrician · 4.9 <span className="text-gold-600">★</span></p></div>
                <span className="text-xs font-semibold text-emerald-700">Approved</span>
              </div>
              <div className="mt-5 space-y-0">
                <div className="flex gap-4"><div className="flex flex-col items-center"><span className="grid h-7 w-7 place-items-center rounded-full bg-aub-900 text-xs font-bold text-white">✓</span><span className="h-8 w-px bg-ice-300" /></div><div className="pb-4"><p className="text-sm font-bold">Booking accepted</p><p className="text-xs text-slate-500">Your worker is on the way</p></div></div>
                <div className="flex gap-4"><div className="flex flex-col items-center"><span className="grid h-7 w-7 place-items-center rounded-full bg-gold-400 text-xs font-bold text-aub-950">02</span><span className="h-8 w-px bg-ice-300" /></div><div className="pb-4"><p className="text-sm font-bold">Share your start code</p><p className="text-xs text-slate-500">Only when they arrive</p></div></div>
                <div className="flex gap-4"><div className="grid h-7 w-7 place-items-center rounded-full border border-ice-300 bg-white text-xs font-bold text-slate-500">03</div><div><p className="text-sm font-bold text-slate-500">Work done. Pay securely.</p><p className="text-xs text-slate-500">You stay in control</p></div></div>
              </div>
              <div className="mt-6 flex items-center justify-between rounded-xl bg-aub-50 px-4 py-3"><span className="text-xs font-semibold text-aub-700">Your start code</span><span className="font-display text-lg font-extrabold tracking-[0.3em] text-aub-950">••••••</span></div>
            </div>
            <div className="absolute -left-4 top-10 hidden rounded-2xl border border-ice-200 bg-white px-4 py-3 shadow-lg sm:block"><span className="text-xs font-semibold text-slate-500">Trusted nearby</span><p className="mt-0.5 text-sm font-bold text-aub-900">Made for your neighborhood</p></div>
          </div>
        </div>
      </section>

      <section>
        <div className="flex flex-wrap items-end justify-between gap-4">
          <div><p className="eyebrow">What needs doing?</p><h2 className="section-title">The right person for the job.</h2></div>
          <Link to={user ? "/workers" : "/register"} className="hidden text-sm font-bold text-aub-700 hover:text-aub-950 sm:inline-flex">Explore all services <span className="ml-2">↗</span></Link>
        </div>
        <div className="mt-7 grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
          {SERVICES.map((service) => <Link key={service.name} to={user ? `/workers?category=${service.category}` : "/register"} className="group rounded-2xl border border-ice-200 bg-white p-5 transition duration-200 hover:-translate-y-1 hover:border-aub-300 hover:shadow-[0_14px_30px_rgba(32,17,38,0.08)]">
            <span className={`grid h-11 w-11 place-items-center rounded-xl font-display text-lg font-extrabold ${service.tone}`}>{service.mark}</span>
            <h3 className="mt-5 text-lg font-bold">{service.name}</h3><p className="mt-1 text-sm text-slate-500">{service.detail}</p>
            <span className="mt-5 inline-flex items-center text-xs font-bold text-aub-700">Find a pro <span className="ml-1 transition group-hover:translate-x-1">→</span></span>
          </Link>)}
        </div>
      </section>

      <section className="grid gap-10 rounded-[2rem] border border-ice-200 bg-white p-7 sm:p-10 lg:grid-cols-[0.8fr_1.2fr] lg:p-12">
        <div><p className="eyebrow">Easy from start to finish</p><h2 className="section-title">A better way to get it done.</h2><p className="mt-4 max-w-md leading-7 text-slate-600">No chasing, no guesswork. Your appointment stays organized from the first request through payment.</p></div>
        <ol className="grid gap-5 sm:grid-cols-2">
          {STEPS.map((step, i) => <li key={step.title} className="flex gap-4">
            <span className="grid h-10 w-10 shrink-0 place-items-center rounded-xl bg-aub-50 font-display text-sm font-extrabold text-aub-800">0{i + 1}</span>
            <div><h3 className="text-base font-bold">{step.title}</h3><p className="mt-1 text-sm leading-6 text-slate-500">{step.text}</p></div>
          </li>)}
        </ol>
      </section>

      {!user && <section className="relative overflow-hidden rounded-[2rem] bg-gold-400 px-7 py-9 sm:flex sm:items-center sm:justify-between sm:px-10">
        <div className="absolute -right-10 -top-24 h-64 w-64 rounded-full border-[40px] border-white/20" />
        <div className="relative"><p className="text-xs font-extrabold uppercase tracking-[0.15em] text-aub-800">For skilled professionals</p><h2 className="mt-2 text-3xl font-bold">Good at what you do?</h2><p className="mt-2 text-sm text-aub-800">Meet customers nearby and grow your work on your terms.</p></div>
        <Link to="/register?as=worker" className="btn btn-success relative mt-6 sm:mt-0">Join as a worker <span>↗</span></Link>
      </section>}
    </div>
  );
}
