import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Teams from './pages/Teams';
import TeamBoards from './pages/TeamBoards';
import BoardDetail from './pages/BoardDetail';
import Auth from './pages/Auth';
import { AuthProvider } from './components/AuthContext';
import PrivateRoute from './components/PrivateRoute';
import Navbar from './components/Navbar';

const App = () => {
    return (
        <AuthProvider>
            <Router>
                <Navbar/>
                <Routes>
                    <Route path="/auth" element={<Auth />} />

                    <Route
                        path="*"
                        element={
                            <PrivateRoute>
                                <Routes>
                                    <Route path="/" element={<Navigate to="/teams" replace />} />
                                    <Route path="/teams" element={<Teams />} />
                                    <Route path="/teams/:teamId" element={<TeamBoards />} />
                                    <Route path="/boards/:boardId" element={<BoardDetail />} />

                                </Routes>
                            </PrivateRoute>
                        }
                    />
                </Routes>
            </Router>
        </AuthProvider>
    );
};

export default App;
