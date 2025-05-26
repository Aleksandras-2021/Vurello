import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider, theme as antdTheme } from 'antd';
import Teams from './pages/Teams';
import TeamBoards from './pages/TeamBoards';
import BoardDetail from './pages/BoardDetail';
import Labels from './pages/Labels';
import Auth from './pages/Auth';
import { AuthProvider } from './components/AuthContext';
import { ThemeProvider, useTheme } from './components/ThemeContext';
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

const ThemedAuthPage = () => {
    const { isDarkMode } = useTheme();

    return (
        <div
            style={{
                minHeight: '100vh',
                backgroundColor: isDarkMode ? '#141414' : '#ffffff',
                transition: 'background-color 0.3s ease'
            }}
        >
            <Auth />
        </div>
    );
};

const ThemedApp = () => {
    const { isDarkMode } = useTheme();

    return (
        <ConfigProvider
            theme={{
                algorithm: isDarkMode ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm,
                token: {
                    colorPrimary: '#1890ff',
                    borderRadius: 8,
                },
            }}
        >
            <Router>
                <Routes>
                    <Route path="/auth" element={<ThemedAuthPage />} />
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
                <ToastContainer
                    position="top-right"
                    autoClose={5000}
                    theme={isDarkMode ? 'dark' : 'light'}
                />
            </Router>
        </ConfigProvider>
    );
};

const App = () => {
    return (
        <ThemeProvider>
            <AuthProvider>
                <AppProvider>
                    <ThemedApp />
                </AppProvider>
            </AuthProvider>
        </ThemeProvider>
    );
};

export default App;
