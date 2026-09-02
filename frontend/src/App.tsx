import {
    BrowserRouter,
    Route,
    Routes,
} from 'react-router-dom'

import LoginPage from './pages/LoginPage'
import DashboardPage from './pages/DashboardPage'
import WorkspacePage from './pages/WorkspacePage'
import ProjectPage from './pages/ProjectPage'

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route
                    path="/"
                    element={<LoginPage />}
                />

                <Route
                    path="/dashboard"
                    element={<DashboardPage />}
                />

                <Route
                    path="/workspaces/:workspaceId"
                    element={<WorkspacePage />}
                />

                <Route
                    path="/projects/:projectId"
                    element={<ProjectPage />}
                />
            </Routes>
        </BrowserRouter>
    )
}

export default App