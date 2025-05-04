import React, { useEffect, useState } from 'react';
import { List, Typography, Spin, Button } from 'antd';
import { PlusOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm.tsx'
import { useAuth } from '../components/AuthContext';
import { api } from "../components/API"
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';

const { Title } = Typography;

const Teams = () => {
    const { userId } = useAuth();

    const [teams, setTeams] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const { setLastTeamId } = useAppContext();

    const fetchTeams = async () => {
        try {
            const response = await api.get('team');;
            setTeams(response.data);
            console.log(response.data);
        } catch (error) {
            console.error('Failed to fetch teams:', error);
        } finally {
            setLoading(false);
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
        }
    }, []);

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <AppLayout>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <Title level={2} style={{ margin: 0 }}>All Teams</Title>
                <div>
                    <DynamicForm
                        formTitle="Create new team"
                        schemaName="TeamCreate"
                        apiUrl="team"
                        type='post'
                        onSuccess={fetchTeams}
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
                renderItem={(team: any) => {
                    return (
                        <List.Item>
                            <div style={{ width: '100%' }}>
                                <Link to={`/teams/${team.id}`} onClick={() => setLastTeamId(team.id)}>{team.name}</Link>
                            </div>
                        </List.Item>
                    );
                }}
            />
        </AppLayout>
    );
};

export default Teams;