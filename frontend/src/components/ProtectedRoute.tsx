// Built with help from Claude
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

// Pages inside this route require a logged-in user
export default function ProtectedRoute() {
  const { user } = useAuth();
  return user ? <Outlet /> : <Navigate to="/login" replace />;
}
