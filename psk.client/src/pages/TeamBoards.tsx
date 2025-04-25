import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Button, Spin, Typography, List, Card } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";

const { Title } = Typography;

const TeamBoards = () => {
    const { teamId } = useParams();
    const [team, setTeam] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    const fetchTeam = async () => {
        try {
            const response = await api.get(`team/${teamId}`);
            setTeam(response.data);
        } catch (error) {
            console.error('Failed to fetch team:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchTeam();
    }, [teamId]);

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <Title level={2}>{team.name}</Title>
                <div>
                    <DynamicForm
                        formTitle="Edit team"
                        schemaName="TeamUpdate"
                        apiUrl={`team/${teamId}`}
                        type='patch'
                        onSuccess={fetchTeam}
                        trigger={
                            <Button style={{ marginRight: 10 }}>
                                Edit Team
                            </Button>
                        }
                        currentData={team}
                    />
                    <DynamicForm
                        formTitle="Create board"
                        schemaName="BoardCreate"
                        apiUrl="board"
                        type='post'
                        onSuccess={fetchTeam}
                        trigger={
                            <Button type="primary" icon={<PlusOutlined />}>
                                New Board
                            </Button>
                        }
                        neededData={{ teamId }}
                    />
                </div>
            </div>

            <List
                grid={{ gutter: 16, column: 3 }}
                dataSource={team.boards || []}
                renderItem={(board: any) => (
                    <List.Item>
                        <Link to={`/boards/${board.id}`} style={{ display: 'block' }}>
                            <Card
                                hoverable
                                title={board.name}
                            >
                                <p>Click to view tasks</p>
                            </Card>
                        </Link>
                    </List.Item>
                )}
                locale={{ emptyText: "No boards found. Create a new board to get started." }}
            />
        </div>
    );
};

export default TeamBoards;
