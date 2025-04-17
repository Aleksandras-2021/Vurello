import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

const PrivateRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { token, tokenExpiration, loading, logout } = useAuth();

    if (loading) {
        return null;
    }

    const isExpired = (tokenExpiration ?? 0) * 1000 < Date.now();

    if (!token || isExpired) {
        logout();
        return <Navigate to="/auth" />;
    }

    return <>{children}</>;
};

export default PrivateRoute;
