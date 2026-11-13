import { Navigate, useLocation } from "react-router-dom";
import { useAuth, homeFor } from "../context/AuthContext";

export default function ProtectedRoute({ roles, children }) {
  const { user } = useAuth();
  const location = useLocation();

  if (!user) return <Navigate to="/login" state={{ from: location }} replace />;
  if (roles && !roles.includes(user.role))
    return <Navigate to={homeFor(user)} replace />;
  return children;
}
