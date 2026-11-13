import { Routes, Route, Link } from "react-router-dom";
import Navbar from "./components/Navbar";
import ProtectedRoute from "./components/ProtectedRoute";
import Home from "./pages/Home";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Workers from "./pages/Workers";
import MyAppointments from "./pages/MyAppointments";
import WorkerDashboard from "./pages/WorkerDashboard";
import AdminDashboard from "./pages/AdminDashboard";

export default function App() {
  return (
    <>
      <Navbar />
      <main className="app-content mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8 lg:py-10">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          <Route
            path="/workers"
            element={
              <ProtectedRoute roles={["User"]}>
                <Workers />
              </ProtectedRoute>
            }
          />
          <Route
            path="/my-appointments"
            element={
              <ProtectedRoute roles={["User"]}>
                <MyAppointments />
              </ProtectedRoute>
            }
          />
          <Route
            path="/worker"
            element={
              <ProtectedRoute roles={["Worker"]}>
                <WorkerDashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <AdminDashboard />
              </ProtectedRoute>
            }
          />

          <Route
            path="*"
            element={
              <div className="py-20 text-center">
                <h1 className="text-2xl font-semibold">Page not found</h1>
                <Link to="/" className="mt-3 inline-block text-brand-600">Go home</Link>
              </div>
            }
          />
        </Routes>
      </main>
    </>
  );
}
