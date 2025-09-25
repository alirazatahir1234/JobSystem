import React from 'react';
import { Navigate } from 'react-router-dom';
// import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  // For now, we'll assume all routes are accessible
  // const { isAuthenticated } = useAuth();
  const isAuthenticated = true; // Temporary - will be replaced with actual auth logic
  
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" />;
};

export default ProtectedRoute;