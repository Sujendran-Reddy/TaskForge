import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import api from '../api'

type Project = {
    id: string
    name: string
    description: string | null
    createdAt: string
}

function WorkspacePage() {
    const { workspaceId } = useParams()
    const navigate = useNavigate()

    const [projects, setProjects] = useState<Project[]>([])
    const [name, setName] = useState('')
    const [description, setDescription] = useState('')

    async function loadProjects() {
        const response = await api.get(
            `/projects/workspace/${workspaceId}`
        )

        setProjects(response.data)
    }

    useEffect(() => {
        loadProjects()
    }, [workspaceId])

    async function createProject(event: FormEvent) {
        event.preventDefault()

        if (!name.trim()) return

        await api.post('/projects', {
            workspaceId,
            name,
            description,
        })

        setName('')
        setDescription('')

        await loadProjects()
    }

    return (
        <div className="min-h-screen bg-slate-950 text-white">
            <header className="border-b border-slate-800 px-8 py-5 flex items-center justify-between">
                <h1 className="text-2xl font-bold">
                    TaskForge
                </h1>

                <button
                    onClick={() => navigate('/dashboard')}
                    className="text-slate-400 hover:text-white"
                >
                    ← Back to Workspaces
                </button>
            </header>

            <main className="max-w-6xl mx-auto p-8">
                <h2 className="text-3xl font-bold mb-8">
                    Projects
                </h2>

                <form
                    onSubmit={createProject}
                    className="bg-slate-900 border border-slate-800 rounded-xl p-6 mb-10"
                >
                    <h3 className="text-xl font-semibold mb-5">
                        Create Project
                    </h3>

                    <input
                        value={name}
                        onChange={(event) => setName(event.target.value)}
                        placeholder="Project name"
                        className="w-full mb-4 rounded-lg bg-slate-800 border border-slate-700 p-3"
                    />

                    <textarea
                        value={description}
                        onChange={(event) =>
                            setDescription(event.target.value)
                        }
                        placeholder="Description"
                        className="w-full mb-4 rounded-lg bg-slate-800 border border-slate-700 p-3"
                    />

                    <button className="bg-blue-600 px-6 py-3 rounded-lg font-semibold hover:bg-blue-500">
                        Create Project
                    </button>
                </form>

                <div className="grid md:grid-cols-3 gap-5">
                    {projects.map((project) => (
                        <button
                            key={project.id}
                            onClick={() =>
                                navigate(`/projects/${project.id}`)
                            }
                            className="text-left bg-slate-900 border border-slate-800 rounded-xl p-6 hover:border-blue-500"
                        >
                            <h3 className="text-xl font-semibold">
                                {project.name}
                            </h3>

                            <p className="text-slate-400 mt-2">
                                {project.description || 'No description'}
                            </p>
                        </button>
                    ))}
                </div>
            </main>
        </div>
    )
}

export default WorkspacePage