import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { CallsListPage } from "./components/CallsListPage";
import { AddCallPage } from "./components/AddCallPage";
import { CallDetailPage } from "./components/CallDetailPage";
import { DashboardPage } from "./components/DashboardPage";

const AppRoutes = [
  {
    index: true,
    element: <DashboardPage />
  },
  {
    path: '/dashboard',
    element: <DashboardPage />
  },
  {
    path: '/calls',
    element: <CallsListPage />
  },
  {
    path: '/calls/add',
    element: <AddCallPage />
  },
  {
    path: '/calls/:id',
    element: <CallDetailPage />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  }
];

export default AppRoutes;
