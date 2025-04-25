import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm';
import { Button, Spin, Typography,Modal, List, Card, Table, Space } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { api } from "../components/API";
const { Title } = Typography;


const TeamBoards = () => {
    const { teamId } = useParams();
    const [team, setTeam] = useState<any>(null);
    const [loading, setLoading] = useState(true);
    const [membersVisible, setMembersVisible] = useState(false);
    const [members, setMembers] = useState<any[]>([]);

    const columns = [
        {
            title: 'Name',
            dataIndex: 'userName',
            key: 'userName',
        },
        {
            title: 'Actions',
            key: 'actions',
            render: (_, record) => (
                <Space>
                    <Button type="link" danger>View</Button>
                    <Button type="link">Remove</Button>
                </Space>
            )
        }
    ];

    const fetchMembers = async () => {
        try {
            const response = await api.get(`team/${teamId}/members`);
            setMembers(response.data);
        } catch (error) {
            console.error('Failed to fetch team members:', error);
        }
    };

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

    const handleShowMembers = async () => {
        setMembersVisible(true);
    };

    useEffect(() => {
        fetchTeam();
        fetchMembers();
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

            <Button
                type="primary"
                style={{ marginTop: 16 }}
                onClick={handleShowMembers}
            >
                My Team
            </Button>

            <Modal
                title="Team Members"
                open={membersVisible}
                onCancel={() => setMembersVisible(false)}
                footer={null}
            >
                <Table
                    dataSource={members}
                    columns={columns}
                    rowKey="id"
                    pagination={false}
                />
            </Modal>
        </div>
    );
};

export default TeamBoards;
