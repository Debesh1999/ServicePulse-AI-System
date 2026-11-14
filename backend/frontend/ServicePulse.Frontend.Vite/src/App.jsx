import { Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './components/Layout';
import { DashboardPage } from './components/DashboardPage';
import { CallsListPage } from './components/CallsListPage';
import { AddCallPage } from './components/AddCallPage';
import { CallDetailPage } from './components/CallDetailPage';
import { LoginPage } from './components/LoginPage';
import { RegisterPage } from './components/RegisterPage';
import { ProtectedRoute } from './components/ProtectedRoute';

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/calls"
        element={
          <ProtectedRoute>
            <CallsListPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/calls/add"
        element={
          <ProtectedRoute>
            <AddCallPage />
          </ProtectedRoute>
        }
      />
      <Route
        path="/calls/:id"
        element={
          <ProtectedRoute>
            <CallDetailPage />
          </ProtectedRoute>
        }
      />
      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}

function App() {
  return (
    <Layout>
      <AppRoutes />
    </Layout>
  );
}

export default App