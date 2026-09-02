import { FormEvent, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../api'

function LoginPage() {
    const navigate = useNavigate()

    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')

    async function handleSubmit(event: FormEvent) {
        event.preventDefault()
        setError('')

        try {
            const response = await api.post('/auth/login', {
                email,
                password,
            })

            localStorage.setItem('token', response.data.token)

            navigate('/dashboard')
        } catch {
            setError('Invalid email or password')
        }
    }

    return (
        <div className="min-h-screen bg-slate-950 text-white flex items-center justify-center">
            <form
                onSubmit={handleSubmit}
                className="w-full max-w-md bg-slate-900 p-8 rounded-xl shadow-xl"
            >
                <h1 className="text-3xl font-bold mb-2">
                    TaskForge
                </h1>

                <p className="text-slate-400 mb-8">
                    Sign in to your workspace
                </p>

                {error && (
                    <p className="mb-4 text-red-400">
                        {error}
                    </p>
                )}

                <label className="block mb-2">
                    Email
                </label>

                <input
                    type="email"
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                    className="w-full mb-5 rounded-lg bg-slate-800 border border-slate-700 p-3"
                    required
                />

                <label className="block mb-2">
                    Password
                </label>

                <input
                    type="password"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                    className="w-full mb-6 rounded-lg bg-slate-800 border border-slate-700 p-3"
                    required
                />

                <button
                    type="submit"
                    className="w-full rounded-lg bg-blue-600 py-3 font-semibold hover:bg-blue-500"
                >
                    Login
                </button>
            </form>
        </div>
    )
}

export default LoginPage