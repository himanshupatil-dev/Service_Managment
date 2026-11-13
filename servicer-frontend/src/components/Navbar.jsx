import { useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAuth, homeFor } from "../context/AuthContext";

function navClass({ isActive }) {
  return `rounded-lg px-3 py-2 text-sm font-semibold transition ${isActive ? "bg-aub-50 text-aub-900" : "text-slate-600 hover:bg-ice-100 hover:text-aub-900"}`;
}

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);
  const initials = user?.name?.split(/\s+/).map((part) => part[0]).slice(0, 2).join("").toUpperCase() || "S";

  const close = () => setOpen(false);
  const signOut = () => { logout(); close(); navigate("/"); };

  return (
    <header className="sticky top-0 z-40 border-b border-ice-200/80 bg-white/90 backdrop-blur-xl">
      <div className="mx-auto flex h-[72px] max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <Link to="/" onClick={close} className="flex items-center gap-2.5" aria-label="Servicer home">
          <span className="grid h-10 w-10 place-items-center rounded-xl bg-aub-900 font-display text-xl font-extrabold text-gold-400">S</span>
          <span className="font-display text-xl font-bold tracking-tight text-aub-950">servicer<span className="text-gold-600">.</span></span>
        </Link>

        <button className="grid h-10 w-10 place-items-center rounded-xl border border-ice-200 text-aub-900 md:hidden" onClick={() => setOpen(!open)} aria-label={open ? "Close menu" : "Open menu"} aria-expanded={open}>
          <span className="text-xl leading-none">{open ? "×" : "☰"}</span>
        </button>

        <nav className="hidden items-center gap-1 md:flex">
          {!user && <>
            <NavLink to="/login" className={navClass}>Log in</NavLink>
            <Link to="/register" className="btn btn-primary ml-2 !px-4 !py-2.5">Get started <span aria-hidden="true">↗</span></Link>
          </>}
          {user?.role === "User" && <>
            <NavLink to="/workers" className={navClass}>Find a worker</NavLink>
            <NavLink to="/my-appointments" className={navClass}>My appointments</NavLink>
          </>}
          {user?.role === "Worker" && <NavLink to="/worker" className={navClass}>My jobs</NavLink>}
          {user?.role === "Admin" && <NavLink to="/admin" className={navClass}>Admin</NavLink>}
          {user && <div className="ml-3 flex items-center gap-3 border-l border-ice-200 pl-4">
            <Link to={homeFor(user)} className="flex items-center gap-2.5 rounded-xl px-2 py-1.5 hover:bg-ice-100">
              <span className="grid h-9 w-9 place-items-center rounded-full bg-gold-100 text-xs font-bold text-aub-900">{initials}</span>
              <span className="max-w-32 truncate text-sm font-semibold text-aub-900">{user.name}</span>
            </Link>
            <button className="text-sm font-semibold text-slate-500 transition hover:text-aub-900" onClick={signOut}>Log out</button>
          </div>}
        </nav>
      </div>

      {open && <nav className="border-t border-ice-200 bg-white px-4 py-3 shadow-lg md:hidden">
        <div className="mx-auto flex max-w-7xl flex-col gap-1">
          {!user && <><NavLink onClick={close} to="/login" className={navClass}>Log in</NavLink><Link onClick={close} to="/register" className="btn btn-primary mt-1">Get started</Link></>}
          {user?.role === "User" && <><NavLink onClick={close} to="/workers" className={navClass}>Find a worker</NavLink><NavLink onClick={close} to="/my-appointments" className={navClass}>My appointments</NavLink></>}
          {user?.role === "Worker" && <NavLink onClick={close} to="/worker" className={navClass}>My jobs</NavLink>}
          {user?.role === "Admin" && <NavLink onClick={close} to="/admin" className={navClass}>Admin</NavLink>}
          {user && <button className="mt-2 rounded-lg px-3 py-2 text-left text-sm font-semibold text-slate-600 hover:bg-ice-100" onClick={signOut}>Log out · {user.name}</button>}
        </div>
      </nav>}
    </header>
  );
}
