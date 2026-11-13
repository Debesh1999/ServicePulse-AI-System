import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { CallsListPage } from "./components/CallsListPage";
import { AddCallPage } from "./components/AddCallPage";
import { CallDetailPage } from "./components/CallDetailPage";
import { DashboardPage } from "./components/DashboardPage";
import { LoginPage } from "./components/LoginPage";
import { RegisterPage } from "./components/RegisterPage";
import { ProtectedRoute } from "./components/ProtectedRoute";

const AppRoutes = [
  {
    index: true,
    element: <DashboardPage />,
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/register',
    element: <RegisterPage />,
  },
  {
    path: '/dashboard',
    element: (
      <ProtectedRoute>
        <DashboardPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/calls',
    element: (
      <ProtectedRoute>
        <CallsListPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/calls/add',
    element: (
      <ProtectedRoute>
        <AddCallPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/calls/:id',
    element: (
      <ProtectedRoute>
        <CallDetailPage />
      </ProtectedRoute>
    ),
  },
  {
    path: '/counter',
    element: <Counter />,
  },
  {
    path: '/fetch-data',
    element: <FetchData />,
  },
];

export default AppRoutes;