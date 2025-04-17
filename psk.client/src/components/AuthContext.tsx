import React, { createContext, useContext, useState, ReactNode, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import {setRefreshTokenFn } from './API';
import axios from 'axios';

interface AuthContextType {
    token: string | null;
    userId: string | null;
    tokenExpiration: number | null;
    loading: boolean;
    login: (token: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [userId, setUserId] = useState<string | null>(null);
    const [tokenExpiration, setTokenExpiration] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (token) {
            const decodedToken: any = jwtDecode(token);
            setUserId(decodedToken.userId);
            setTokenExpiration(decodedToken.exp);
            localStorage.setItem('tokenExpiration', decodedToken.exp);
            localStorage.setItem('token', token);
        } else {
            setUserId(null);
            setTokenExpiration(null);
        }
        setRefreshTokenFn(async () => {
            try {
                const response = await axios.post(
                    `https://localhost:7285/api/auth/refresh/${token}`,
                    null,
                    {
                        headers: {
                            'Content-Type': 'application/json',
                        },
                    }
                );

                const newToken = response.data.token as string;
                setToken(newToken);
                return newToken;
            } catch (error) {
                console.error('Error refreshing token:', error);
                logout(); 
                return null;
            }
        });
        setLoading(false);
    }, [token]);

    const login = (newToken: string) => {
        setToken(newToken);
    };

    const logout = () => {
        setToken(null);
        setUserId(null);
        setTokenExpiration(null);
        localStorage.removeItem('token');
        localStorage.removeItem('tokenExpiration');
    };

    return (
        <AuthContext.Provider value={{ token, userId, tokenExpiration, loading, login, logout, }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error();
    return context;
};
