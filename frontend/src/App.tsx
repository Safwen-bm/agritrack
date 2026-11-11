// Built with help from Claude
import { Navigate, Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import Dashboard from "./pages/Dashboard";
import FarmDetail from "./pages/FarmDetail";
import Farms from "./pages/Farms";
import FieldDetail from "./pages/FieldDetail";
import Landing from "./pages/Landing";
import Login from "./pages/Login";
import MapPage from "./pages/MapPage";
import PlantingDetail from "./pages/PlantingDetail";
import Register from "./pages/Register";

export default function App() {
  return (
    <Routes>
      {/* Public landing page, with its own header and footer */}
      <Route path="/" element={<Landing />} />

      <Route element={<Layout />}>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route element={<ProtectedRoute />}>
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/farms" element={<Farms />} />
          <Route path="/map" element={<MapPage />} />
          <Route path="/farms/:farmId" element={<FarmDetail />} />
          <Route path="/fields/:fieldId" element={<FieldDetail />} />
          <Route path="/plantings/:plantingId" element={<PlantingDetail />} />
        </Route>
      </Route>

      {/* Unknown URLs go back to the landing page */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
