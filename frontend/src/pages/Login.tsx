// Built with help from Claude
import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../AuthContext'

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault() // same idea as preventDefault in the lecture
    setError('')
    setLoading(true)
    try {
      await login(email, password)
      navigate('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="mx-auto max-w-sm rounded-lg bg-white p-6 shadow">
      <h1 className="mb-4 text-2xl font-bold text-green-800">Log in</h1>
      {error && <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>}
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="email" required placeholder="Email" value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded border p-2"
        />
        <input
          type="password" required placeholder="Password" value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full rounded border p-2"
        />
        <button
          type="submit" disabled={loading}
          className="w-full rounded bg-green-700 p-2 text-white hover:bg-green-600 disabled:opacity-50"
        >
          {loading ? 'Logging in...' : 'Log in'}
        </button>
      </form>
      <p className="mt-4 text-sm">
        No account? <Link to="/register" className="text-green-700 underline">Register</Link>
      </p>
    </div>
  )
}