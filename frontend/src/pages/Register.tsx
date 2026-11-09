// Built with help from Claude
import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../AuthContext'

export default function Register() {
  const { register } = useAuth()
  const navigate = useNavigate()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await register(fullName, email, password)
      navigate('/')
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="mx-auto max-w-sm rounded-lg bg-white p-6 shadow">
      <h1 className="mb-4 text-2xl font-bold text-green-800">Create account</h1>
      {error && <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>}
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="text" required placeholder="Full name" value={fullName}
          onChange={(e) => setFullName(e.target.value)}
          className="w-full rounded border p-2"
        />
        <input
          type="email" required placeholder="Email" value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded border p-2"
        />
        <input
          type="password" required minLength={8} placeholder="Password (min 8 characters)"
          value={password} onChange={(e) => setPassword(e.target.value)}
          className="w-full rounded border p-2"
        />
        <button
          type="submit" disabled={loading}
          className="w-full rounded bg-green-700 p-2 text-white hover:bg-green-600 disabled:opacity-50"
        >
          {loading ? 'Creating...' : 'Register'}
        </button>
      </form>
      <p className="mt-4 text-sm">
        Already registered? <Link to="/login" className="text-green-700 underline">Log in</Link>
      </p>
    </div>
  )
}