import React, { useEffect, useState } from 'react';
import { List, Typography, Spin, Button } from 'antd';
import { PlusOutlined, ArrowLeftOutlined, ReloadOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm.tsx';
import { useAuth } from '../components/AuthContext';
import { api } from '../components/API';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import { mergeEntities } from '../utils/stateHelpers.ts';
import { toast } from 'react-toastify';

const { Title } = Typography;

const Teams = () => {
    const { userId } = useAuth();
    const [teams, setTeams] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [refreshLoading, setRefreshLoading] = useState(false);
    const { setLastTeamId } = useAppContext();

    const fetchTeams = async () => {
        try {
            setLoading(true);
            const response = await api.get('team');
            setTeams(response.data);
        } catch (error) {
            console.error('Failed to fetch teams:', error);
            toast.error('Failed to load teams'); 
        } finally {
            setLoading(false);
        }
    };

    const handleTeamCreated = (newTeam: any) => {
        if (!newTeam) return;
        setTeams(currentTeams => mergeEntities(currentTeams, newTeam));
    };

    const handleRefresh = async () => {
        setRefreshLoading(true);
        try {
            await fetchTeams();
            toast.success('Teams refreshed successfully');
        } catch (error) {
            toast.error('Failed to refresh teams');
        } finally {
            setRefreshLoading(false);
        }
    };

    useEffect(() => {
        fetchTeams();

        const handleTeamInvitationAccepted = () => {
            fetchTeams();
        };

        window.addEventListener('team-invitation-accepted', handleTeamInvitationAccepted);
        return () => {
            window.removeEventListener('team-invitation-accepted', handleTeamInvitationAccepted);
        };
    }, []);

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <AppLayout>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <Title level={2} style={{ margin: 0 }}>All Teams</Title>
                <div style={{ display: 'flex', gap: 16 }}>
                    <Button
                        icon={<ReloadOutlined />}
                        onClick={handleRefresh}
                        loading={refreshLoading}
                        disabled={loading || refreshLoading}
                    >
                        Refresh
                    </Button>
                    <DynamicForm
                        formTitle="Create new team"
                        schemaName="TeamCreate"
                        apiUrl="team"
                        type='post'
                        onSuccess={handleTeamCreated}
                        trigger={
                            <Button type="primary" icon={<PlusOutlined />}>
                                New Team
                            </Button>
                        }
                        neededData={{ userId }}
                    />
                </div>
            </div>

            <List
                bordered
                dataSource={teams}
                renderItem={(team: any) => (
                    <List.Item>
                        <div style={{ width: '100%' }}>
                            <Link to={`/teams/${team.id}`} onClick={() => setLastTeamId(team.id)}>{team.name}</Link>
                        </div>
                    </List.Item>
                )}
            />
        </AppLayout>
    );
};

export default Teams;