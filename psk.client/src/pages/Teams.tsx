import React, { useEffect, useState } from 'react';
import { List, Typography, Spin, Button } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm.tsx'
import { useAuth } from '../components/AuthContext';
import {api } from "../components/API"
const Teams = () => {
    const { userId } = useAuth();

    const [teams, setTeams] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

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
    }, []);

    if (loading) {
        return <Spin size="large" />;
    };

    return (

        <div style={{ padding: 24 }}>
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

            <Typography>All Teams</Typography>
            <List
                bordered
                dataSource={teams}
                renderItem={(team: any) => (
                    <List.Item>
                        <Link to={`/teams/${team.id}`}>{team.name}</Link>
                    </List.Item>
                )}
            />
        </div>
    );
};

export default Teams;
