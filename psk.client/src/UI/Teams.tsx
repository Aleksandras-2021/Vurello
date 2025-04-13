import React, { useEffect, useState } from 'react';
import { List, Typography, Spin, Button } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { Link } from 'react-router-dom';
import { api } from '../API/API.tsx';
import DynamicForm from '../API/DynamicForm.tsx'

const Teams = () => {
    const [teams, setTeams] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchTeams = async () => {
        try {
            const data = await api.get('team');
            setTeams(Array.isArray(data) ? data : [data]);
            console.log(data);
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
                onSuccess={fetchTeams}
                trigger={
                    <Button type="primary" icon={<PlusOutlined />}>
                        New Team
                    </Button>
                }
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
