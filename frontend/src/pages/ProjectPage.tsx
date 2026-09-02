import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import api from '../api'

type TaskStatus = 'Todo' | 'InProgress' | 'Done'

type Task = {
    id: string
    title: string
    description: string | null
    priority: string
    status: TaskStatus
    dueDate: string | null
}

function ProjectPage() {
    const { projectId } = useParams()
    const navigate = useNavigate()

    const [tasks, setTasks] = useState<Task[]>([])
    const [title, setTitle] = useState('')
    const [description, setDescription] = useState('')
    const [priority, setPriority] = useState('Medium')

    async function loadTasks() {
        const response = await api.get(
            `/tasks/project/${projectId}`
        )

        setTasks(response.data)
    }

    useEffect(() => {
        loadTasks()
    }, [projectId])

    async function createTask(event: FormEvent) {
        event.preventDefault()

        if (!title.trim()) return

        await api.post('/tasks', {
            projectId,
            title,
            description,
            priority,
        })

        setTitle('')
        setDescription('')
        setPriority('Medium')

        await loadTasks()
    }

    async function updateStatus(
        taskId: string,
        status: TaskStatus
    ) {
        await api.patch(`/tasks/${taskId}/status`, {
            status,
        })

        await loadTasks()
    }

    function tasksForStatus(status: TaskStatus) {
        return tasks.filter((task) => task.status === status)
    }

    function handleDragStart(
        event: React.DragEvent,
        taskId: string
    ) {
        event.dataTransfer.setData('taskId', taskId)
    }

    async function handleDrop(
        event: React.DragEvent,
        status: TaskStatus
    ) {
        event.preventDefault()

        const taskId = event.dataTransfer.getData('taskId')

        if (!taskId) return

        await updateStatus(taskId, status)
    }

    const columns: {
        title: string
        status: TaskStatus
    }[] = [
            {
                title: 'TODO',
                status: 'Todo',
            },
            {
                title: 'IN PROGRESS',
                status: 'InProgress',
            },
            {
                title: 'DONE',
                status: 'Done',
            },
        ]

    return (
        <div className="min-h-screen bg-slate-950 text-white">
            <header className="border-b border-slate-800 px-8 py-5 flex justify-between">
                <h1 className="text-2xl font-bold">
                    TaskForge
                </h1>

                <button
                    onClick={() => navigate('/dashboard')}
                    className="text-slate-400 hover:text-white"
                >
                    Workspaces
                </button>
            </header>

            <main className="p-8">
                <div className="max-w-7xl mx-auto">
                    <h2 className="text-3xl font-bold mb-8">
                        Project Board
                    </h2>

                    <form
                        onSubmit={createTask}
                        className="bg-slate-900 border border-slate-800 rounded-xl p-6 mb-10"
                    >
                        <h3 className="text-xl font-semibold mb-5">
                            Create Task
                        </h3>

                        <div className="grid md:grid-cols-3 gap-4">
                            <input
                                value={title}
                                onChange={(event) =>
                                    setTitle(event.target.value)
                                }
                                placeholder="Task title"
                                className="rounded-lg bg-slate-800 border border-slate-700 p-3"
                            />

                            <input
                                value={description}
                                onChange={(event) =>
                                    setDescription(event.target.value)
                                }
                                placeholder="Description"
                                className="rounded-lg bg-slate-800 border border-slate-700 p-3"
                            />

                            <select
                                value={priority}
                                onChange={(event) =>
                                    setPriority(event.target.value)
                                }
                                className="rounded-lg bg-slate-800 border border-slate-700 p-3"
                            >
                                <option value="Low">
                                    Low
                                </option>

                                <option value="Medium">
                                    Medium
                                </option>

                                <option value="High">
                                    High
                                </option>
                            </select>
                        </div>

                        <button className="mt-4 bg-blue-600 px-6 py-3 rounded-lg font-semibold hover:bg-blue-500">
                            Create Task
                        </button>
                    </form>

                    <div className="grid lg:grid-cols-3 gap-6">
                        {columns.map((column) => (
                            <div
                                key={column.status}
                                onDragOver={(event) =>
                                    event.preventDefault()
                                }
                                onDrop={(event) =>
                                    handleDrop(event, column.status)
                                }
                                className="bg-slate-900 border border-slate-800 rounded-xl p-5 min-h-[500px]"
                            >
                                <div className="flex justify-between mb-5">
                                    <h3 className="font-bold text-slate-300">
                                        {column.title}
                                    </h3>

                                    <span className="text-slate-500">
                                        {tasksForStatus(column.status).length}
                                    </span>
                                </div>

                                <div className="space-y-4">
                                    {tasksForStatus(column.status).map(
                                        (task) => (
                                            <div
                                                key={task.id}
                                                draggable
                                                onDragStart={(event) =>
                                                    handleDragStart(
                                                        event,
                                                        task.id
                                                    )
                                                }
                                                className="bg-slate-800 border border-slate-700 rounded-lg p-4 cursor-grab"
                                            >
                                                <div className="flex justify-between gap-3">
                                                    <h4 className="font-semibold">
                                                        {task.title}
                                                    </h4>

                                                    <span
                                                        className={
                                                            task.priority === 'High'
                                                                ? 'text-red-400 text-sm'
                                                                : task.priority === 'Medium'
                                                                    ? 'text-yellow-400 text-sm'
                                                                    : 'text-green-400 text-sm'
                                                        }
                                                    >
                                                        {task.priority}
                                                    </span>
                                                </div>

                                                {task.description && (
                                                    <p className="text-slate-400 text-sm mt-3">
                                                        {task.description}
                                                    </p>
                                                )}
                                            </div>
                                        )
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </main>
        </div>
    )
}

export default ProjectPage