// Built with help from Claude
import { Link, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Layout() {
  const { user, logout } = useAuth();

  function handleLogout() {
  logout();
  window.location.assign("/");
}

  return (
    <div className="min-h-screen bg-green-50">
      <nav className="bg-green-800 text-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <Link to={user ? "/dashboard" : "/"} className="text-xl font-bold">
            AgriTrack
          </Link>
          {user && (
            <div className="flex items-center gap-4">
              <Link to="/dashboard" className="text-sm hover:underline">
                Dashboard
              </Link>
              <Link to="/farms" className="text-sm hover:underline">
                Farms
              </Link>
              <Link to="/map" className="text-sm hover:underline">
                Map
              </Link>
              <span className="hidden text-sm sm:inline">{user.fullName}</span>
              <button
                onClick={handleLogout}
                className="rounded bg-green-700 px-3 py-1 text-sm hover:bg-green-600"
              >
                Log out
              </button>
            </div>
          )}
        </div>
      </nav>
      <main className="mx-auto max-w-5xl px-4 py-8">
        <Outlet />
      </main>
    </div>
  );
}
