// Built with help from Claude
import { Route, Routes } from "react-router-dom";
import Layout from "./components/Layout";
import ProtectedRoute from "./components/ProtectedRoute";
import Farms from "./pages/Farms";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import FarmDetail from "./pages/FarmDetail";
import FieldDetail from "./pages/FieldDetail";
import PlantingDetail from "./pages/PlantingDetail";

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<Dashboard />} />
          <Route path="/farms" element={<Farms />} />
          <Route path="/farms/:farmId" element={<FarmDetail />} />
          <Route path="/fields/:fieldId" element={<FieldDetail />} />
          <Route path="/plantings/:plantingId" element={<PlantingDetail />} />
        </Route>
      </Route>
    </Routes>
  );
}
