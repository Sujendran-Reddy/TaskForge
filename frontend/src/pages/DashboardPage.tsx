import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../api'

type Workspace = {
    id: string
    name: string
    role: string
    createdAt: string
}

function DashboardPage() {
    const navigate = useNavigate()

    const [workspaces, setWorkspaces] = useState<Workspace[]>([])
    const [workspaceName, setWorkspaceName] = useState('')

    async function loadWorkspaces() {
        try {
            const response = await api.get('/workspaces')
            setWorkspaces(response.data)
        } catch {
            localStorage.removeItem('token')
            navigate('/')
        }
    }

    useEffect(() => {
        loadWorkspaces()
    }, [])

    async function createWorkspace(event: FormEvent) {
        event.preventDefault()

        if (!workspaceName.trim()) return

        await api.post('/workspaces', {
            name: workspaceName,
        })

        setWorkspaceName('')

        await loadWorkspaces()
    }

    function logout() {
        localStorage.removeItem('token')
        navigate('/')
    }

    return (
        <div className="min-h-screen bg-slate-950 text-white">
            <header className="border-b border-slate-800 px-8 py-5 flex justify-between">
                <h1 className="text-2xl font-bold">
                    TaskForge
                </h1>

                <button
                    onClick={logout}
                    className="text-slate-400 hover:text-white"
                >
                    Logout
                </button>
            </header>

            <main className="max-w-6xl mx-auto p-8">
                <h2 className="text-3xl font-bold mb-8">
                    Your Workspaces
                </h2>

                <form
                    onSubmit={createWorkspace}
                    className="flex gap-3 mb-10"
                >
                    <input
                        value={workspaceName}
                        onChange={(event) =>
                            setWorkspaceName(event.target.value)
                        }
                        placeholder="Workspace name"
                        className="flex-1 rounded-lg bg-slate-900 border border-slate-700 p-3"
                    />

                    <button
                        className="bg-blue-600 px-6 rounded-lg font-semibold hover:bg-blue-500"
                    >
                        Create Workspace
                    </button>
                </form>

                <div className="grid md:grid-cols-3 gap-5">
                    {workspaces.map((workspace) => (
                        <button
                            key={workspace.id}
                            onClick={() =>
                                navigate(`/workspaces/${workspace.id}`)
                            }
                            className="text-left bg-slate-900 border border-slate-800 p-6 rounded-xl hover:border-blue-500"
                        >
                            <h3 className="text-xl font-semibold">
                                {workspace.name}
                            </h3>

                            <p className="text-slate-400 mt-2">
                                {workspace.role}
                            </p>
                        </button>
                    ))}
                </div>
            </main>
        </div>
    )
}

export default DashboardPage