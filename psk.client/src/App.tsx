import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Teams from './pages/Teams';
import TeamBoards from './pages/TeamBoards';
import BoardDetail from './pages/BoardDetail';
import Labels from './pages/Labels';
import Auth from './pages/Auth';
import { AuthProvider } from './components/AuthContext';
import PrivateRoute from './components/PrivateRoute';
import Navbar from './components/Navbar';
import { ToastContainer } from 'react-toastify';
import { AppProvider } from './components/AppContext';
import { Layout } from 'antd';
import { DndProvider } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';

const AppLayout = ({ children }: { children: React.ReactNode }) => (
    <Layout style={{ minHeight: '100vh' }}>
        <Navbar />
        <Layout style={{ marginLeft: 80, marginTop: 64, padding: 24 }}>
            {children}
        </Layout>
    </Layout>
);

const App = () => {
    return (
        <AuthProvider>
            <AppProvider>
                <Router>
                    <Routes>
                        <Route path="/auth" element={<Auth />} />
                        <Route
                            path="*"
                            element={
                                <PrivateRoute>
                                    <DndProvider backend={HTML5Backend}>
                                        <AppLayout>
                                            <Routes>
                                                <Route path="/" element={<Navigate to="/teams" replace />} />
                                                <Route path="/teams" element={<Teams />} />
                                                <Route path="/teams/:teamId" element={<TeamBoards />} />
                                                <Route path="/boards/:boardId" element={<BoardDetail />} />
                                                <Route path="/labels" element={<Labels />} />
                                            </Routes>
                                        </AppLayout>
                                    </DndProvider>
                                </PrivateRoute>
                            }
                        />
                    </Routes>
                    <ToastContainer position="top-right" autoClose={5000} />
                </Router>
            </AppProvider>
        </AuthProvider>
    );
};

export default App;
