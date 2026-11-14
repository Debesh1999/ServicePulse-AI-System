import React, { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { authService, AUTH_CHANGED_EVENT } from '../services/authService';

export function ProtectedRoute({ children }) {
  const location = useLocation();
  const [user, setUser] = useState(authService.getUser());

  useEffect(() => {
    const onAuthChanged = () => setUser(authService.getUser());
    window.addEventListener(AUTH_CHANGED_EVENT, onAuthChanged);
    return () => window.removeEventListener(AUTH_CHANGED_EVENT, onAuthChanged);
  }, []);

  if (!user) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
}