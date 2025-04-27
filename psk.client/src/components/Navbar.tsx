import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Layout, Button, Typography } from 'antd';
import { useAuth } from './AuthContext';
import UserInbox from "./UserInbox";


const Navbar: React.FC = () => {
    const { token, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/auth');
    };

    return (
        <>
            <Layout.Header
                style={{
                    position: 'fixed',
                    top: 0,
                    width: '100%',
                    zIndex: 100,
                    padding: 0,
                    height: '64px',
                    lineHeight: '64px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    backgroundColor: '#ffffff',
                    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.1)',
                    paddingInline: '2rem',
                    margin: 0
                }}
            >
                <Typography.Title level={4} style={{ margin: 0 }}>pvd</Typography.Title>
                <div>
                    {token && (
                        <>
                            <UserInbox/>
                            <Button type="text" onClick={handleLogout}>Logout</Button>
                        </>
                    )}
                </div>
            </Layout.Header>

            <div style={{ height: '64px' }} />
        </>

    );
};

export default Navbar;