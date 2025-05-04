import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Layout, Menu, Typography } from 'antd';
import {
    TeamOutlined,
    AppstoreOutlined,
    ProfileOutlined,
    LogoutOutlined,
} from '@ant-design/icons';
import { useAuth } from './AuthContext';
import { useAppContext } from './AppContext';
import UserInbox from './UserInbox';

const { Header, Sider } = Layout;

const Navbar: React.FC = () => {
    const { token, logout } = useAuth();
    const navigate = useNavigate();
    const { lastTeamId, lastBoardId } = useAppContext();

    if (!token) return null;

    const handleLogout = () => {
        logout();
        navigate('/auth');
    };

    return (
        <>
            <Sider
                width={200}
                style={{
                    background: '#fff',
                    position: 'fixed',
                    height: '100vh',
                    left: 0,
                    top: 0,
                    paddingTop: 64,
                    borderRight: '1px solid #f0f0f0',
                }}
            >
                <Menu mode="inline" style={{ height: '100%' }} selectable={false}>
                    <Menu.Item icon={<TeamOutlined />} onClick={() => navigate('/teams')}>
                        Teams
                    </Menu.Item>
                    <Menu.Item
                        icon={<AppstoreOutlined />}
                        onClick={() => lastTeamId && navigate(`/teams/${lastTeamId}`)}
                        disabled={!lastTeamId}
                    >
                        Boards
                    </Menu.Item>
                    <Menu.Item
                        icon={<ProfileOutlined />}
                        onClick={() => lastBoardId && navigate(`/boards/${lastBoardId}`)}
                        disabled={!lastBoardId}
                    >
                        Tasks
                    </Menu.Item>
                    <Menu.Item icon={<LogoutOutlined />} onClick={handleLogout}>
                        Logout
                    </Menu.Item>
                </Menu>
            </Sider>

            <Header
                style={{
                    position: 'fixed',
                    top: 0,
                    left: 200,
                    right: 0,
                    height: 64,
                    backgroundColor: '#ffffff',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    paddingInline: '2rem',
                    boxShadow: '0 2px 8px rgba(0, 0, 0, 0.1)',
                    zIndex: 100,
                }}
            >
                <p></p>
                <UserInbox />
            </Header>

            <div
                style={{
                    position: 'fixed',
                    bottom: 10,
                    right: 10,
                    fontSize: '20px',
                    color: '#999',
                    zIndex: 99,
                }}
            >
                kREDDIToriai
            </div>
        </>
    );
};

export default Navbar;
