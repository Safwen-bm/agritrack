// Built with help from Claude
import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../AuthContext'

export default function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  function handleLogout() {
    logout()
    navigate('/login')
  }

  return (
    <div className="min-h-screen bg-green-50">
      <nav className="bg-green-800 text-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <Link to="/" className="text-xl font-bold">AgriTrack</Link>
          {user && (
            <div className="flex items-center gap-4">
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
  )
}