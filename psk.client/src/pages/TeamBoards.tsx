import React, { useState, useEffect } from 'react';
import { Link, useParams, useNavigate } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm';
import { Button, Spin, Modal, Table, Space, Typography, Card, Divider, Layout, Collapse, List, Popconfirm, message } from 'antd';
import { ArrowLeftOutlined, PlusOutlined, UserOutlined, DeleteOutlined } from '@ant-design/icons';
import { api } from '../components/API';
import { toast } from 'react-toastify';
import { useAuth } from '../components/AuthContext';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import { EditOutlined } from '@ant-design/icons';

const { Title } = Typography;

const TeamBoards = () => {
    const { teamId } = useParams();
    const navigate = useNavigate();
    const { userId } = useAuth();
    const [team, setTeam] = useState<any>(null);
    const [loading, setLoading] = useState(true);
    const [membersVisible, setMembersVisible] = useState(false);
    const [members, setMembers] = useState<any[]>([]); 
    const [refreshMembersTrigger, setRefreshMembersTrigger] = useState(0);
    const [isCreator, setIsCreator] = useState(false);
    const { setLastBoardId } = useAppContext();

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

            // Check if the current user is the team creator
            setIsCreator(response.data.creatorId === userId);
            console.log("Team creator ID:", response.data.creatorId);
            console.log("Current user ID:", userId);
            console.log("Is creator:", response.data.creatorId === userId);
        } catch (error) {
            console.error('Failed to fetch team:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleShowMembers = async () => {
        await fetchMembers();
        setMembersVisible(true);
    };

    const handleDeleteTeam = async () => {
        try {
            await api.delete(`team/${teamId}`);
            toast.success('Team deleted successfully');
            navigate('/teams');
        } catch (error) {
            console.error('Failed to delete team:', error);
        }
    };

    const handleDeleteBoard = async (boardId: string) => {
        try {
            await api.delete(`board/${boardId}`);
            toast.success('Board deleted successfully');
            fetchTeam();
        } catch (error) {
            console.error('Failed to delete board:', error);
        }
    };

    const handleRemoveMember = async (memberId: string) => {
        try {
            await api.delete(`team/${teamId}/members/${memberId}`);
            toast.success('Member removed successfully');
            fetchMembers();
        } catch (error) {
            console.error('Failed to remove member:', error);
        }
    };

    const handleTeamConflictCancelled = (latestData?: any) => {
        if (!latestData) return;
        if (latestData) {
            setTeam(latestData);
        }
    };
    const handleBoardConflictCancelled = (latestBoard?: any) => {
        if (!latestBoard) return;

        const updatedBoards = team.boards.map((b: any) =>
            b.id === latestBoard.id ? latestBoard : b
        );

        setTeam({ ...team, boards: updatedBoards });
    };

    useEffect(() => {
        if (!teamId) return;

        fetchTeam();
        fetchMembers();

    }, [teamId, userId]);

    const handleInvitationSent = () => {
        setRefreshMembersTrigger(prev => prev + 1);
        fetchMembers();
    };

    if (loading) {
        return <Spin size="large" />;
    }

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
                    {isCreator && record.id !== team.creatorId && (
                        <Popconfirm
                            title="Remove Member"
                            description="Are you sure you want to remove this member from the team?"
                            onConfirm={() => handleRemoveMember(record.id)}
                            okText="Yes"
                            cancelText="No"
                        >
                            <Button type="link" danger>Remove</Button>
                        </Popconfirm>
                    )}
                </Space>
            )
        }
    ];

    return (
        <AppLayout>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <Link to="/teams">
                        <Button type="text" icon={<ArrowLeftOutlined />} style={{ marginRight: 10 }}>
                            Back
                        </Button>
                    </Link>
                    <Title level={2} style={{ margin: 0 }}>{team.name} Boards</Title>
                </div>
                <div>
                    <Button
                        style={{ marginRight: 10 }}
                        onClick={handleShowMembers}
                    >
                        My Team
                    </Button>

                    <DynamicForm
                        formTitle="Invite user to team"
                        schemaName="InvitationCreate"
                        apiUrl="invitation"
                        type='post'
                        neededData={{ teamId }}
                        onSuccess={handleInvitationSent}
                        trigger={
                            <Button style={{ marginRight: 10 }}>
                                Invite Member
                            </Button>
                        }
                    />

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
                        fetchCurrentData={() => api.get(`team/${teamId}`).then(res => res.data)}
                        onCancelConflict={handleTeamConflictCancelled}
                    />

                    {isCreator && (
                        <Popconfirm
                            title="Delete Team"
                            description="Are you sure you want to delete this team? This action cannot be undone."
                            onConfirm={handleDeleteTeam}
                            okText="Yes, Delete"
                            cancelText="Cancel"
                            okButtonProps={{ danger: true }}
                        >
                            <Button
                                danger
                                icon={<DeleteOutlined />}
                                style={{ marginRight: 10 }}
                                type={"primary"}
                            >
                                Delete Team
                            </Button>
                        </Popconfirm>
                    )}

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
                grid={{ gutter: 16, column: 5 }}
                dataSource={team.boards || []}
                renderItem={(board: any) => (
                    <List.Item>
                        <div style={{ position: 'relative' }}>
                            <Link
                                to={`/boards/${board.id}`}
                                onClick={() => setLastBoardId(board.id)}
                                style={{ display: 'block' }}
                            >
                                <Card hoverable style={{ cursor: 'pointer' }}>
                                    <p style={{
                                        fontSize: '18px',
                                        fontWeight: 'bold',
                                        marginBottom: '12px' // Increased space below title
                                    }}>
                                        {board.name}
                                    </p>
                                    <p style={{ marginTop: '8px' }}> {/* Added space above this line */}
                                        Click anywhere to view tasks
                                    </p>
                                </Card>
                            </Link>
                            <div style={{ position: 'absolute', top: 16, right: 16, zIndex: 1 }}>
                                <DynamicForm
                                    formTitle={`Edit Board: ${board.name}`}
                                    schemaName="BoardUpdate"
                                    apiUrl={`board/${board.id}`}
                                    type="patch"
                                    currentData={board}
                                    onSuccess={fetchTeam}
                                    fetchCurrentData={() => api.get(`team/${teamId}`).then(res => res.data.boards.find((b: any) => b.id === board.id))}
                                    onCancelConflict={handleBoardConflictCancelled}
                                    trigger={
                                        <Button
                                            icon={<EditOutlined />}
                                            size="small"
                                            type="text"
                                            onClick={(e) => e.preventDefault()}
                                        />
                                    }
                                />

                                    <Popconfirm
                                        title="Delete Board"
                                        description="Are you sure you want to delete this Board? This action cannot be undone, and will delete all jobs belonging to the board."
                                        onConfirm={(e) => {
                                            e.stopPropagation()
                                            handleDeleteBoard(board.id)
                                        }}
                                        okText="Yes, Delete"
                                        cancelText="Cancel"
                                        okButtonProps={{ danger: true }}
                                    >
                                        <Button
                                            danger
                                            icon={<DeleteOutlined />}
                                            size="small"
                                            type="text"
                                        >
                                        </Button>
                                    </Popconfirm>
                            </div>
                        </div>
                    </List.Item>





                )}
                locale={{ emptyText: "No boards found. Create a new board to get started." }}
            />

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
        </AppLayout>
    );
};

export default TeamBoards;