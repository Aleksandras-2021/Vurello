import React, { useState, useEffect } from 'react';
import { Link, useParams, useNavigate } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm';
import { Button, Spin, Modal, Table, Space, Typography, Card, Divider, Layout, Collapse, List, Popconfirm, message } from 'antd';
import { ArrowLeftOutlined, PlusOutlined, UserOutlined, DeleteOutlined, ReloadOutlined } from '@ant-design/icons';
import { api } from '../components/API';
import { toast } from 'react-toastify';
import { useAuth } from '../components/AuthContext';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import { EditOutlined } from '@ant-design/icons';
import { mergeEntities } from "../utils/stateHelpers.ts";

const { Title } = Typography;

const TeamBoards = () => {
    const { teamId } = useParams();
    const navigate = useNavigate();
    const { userId } = useAuth();
    const [team, setTeam] = useState<any>(null);
    const [loading, setLoading] = useState(true);
    const [refreshLoading, setRefreshLoading] = useState(false);
    const [isCreator, setIsCreator] = useState(false);
    const { setLastBoardId, setCreatorId } = useAppContext();
    const [contributions, setContributions] = useState<{ totalJobs: number, contributions: any[] }>({
        totalJobs: 0,
        contributions: []
    });
    const [loadingContributions, setLoadingContributions] = useState(false);

    const fetchTeam = async () => {
        try {
            const response = await api.get(`team/${teamId}`);
            setTeam(response.data);
            setIsCreator(response.data.creatorId === userId);
            setCreatorId(response.data.creatorId);
            console.log("Team creator ID:", response.data.creatorId);
            console.log("Current user ID:", userId);
            console.log("Is creator:", response.data.creatorId === userId);
        } catch (error) {
            console.error('Failed to fetch team:', error);
            toast.error('Failed to load team data');
        } finally {
            setLoading(false);
        }
    };

    const fetchContributions = async () => {
        setLoadingContributions(true);
        try {
            const response = await api.get(`team/${teamId}/contributions`);
            setContributions(response.data);
            localStorage.setItem(`team_${teamId}_contributions`, JSON.stringify(response.data));
        } catch (error) {
            toast.error("Failed to load contributions");
            console.error('Contributions error:', error);
        } finally {
            setLoadingContributions(false);
        }
    };

    const handleRefresh = async () => {
        setRefreshLoading(true);
        try {
            await Promise.all([fetchTeam(), fetchContributions()]);
            toast.success('Team data refreshed successfully');
        } catch (error) {
            toast.error('Failed to refresh team data');
        } finally {
            setRefreshLoading(false);
        }
    };

    const handleDeleteTeam = async () => {
        try {
            await api.delete(`team/${teamId}`);
            navigate('/teams');
        } catch (error) {
            console.error('Failed to delete team:', error);
            toast.error('Failed to delete team');
        }
    };

    const handleDeleteBoard = async (boardId: string) => {
        try {
            await api.delete(`board/${boardId}`);
            fetchTeam();
            toast.success('Board deleted successfully');
        } catch (error) {
            console.error('Failed to delete board:', error);
            toast.error('Failed to delete board');
        }
    };

    const handleTeamConflictCancelled = (latestData?: any) => {
        if (!latestData) return;
        setTeam(latestData);
    };

    const handleBoardConflictCancelled = (latestBoard?: any) => {
        if (!latestBoard) return;
        setTeam((prev) => ({
            ...prev,
            boards: prev.boards?.map((b: any) => (b.id === latestBoard.id ? latestBoard : b))
        }));
    };

    useEffect(() => {
        if (!teamId) return;

        fetchTeam();
        fetchContributions();

        const cached = localStorage.getItem(`team_${teamId}_contributions`);
        if (cached) {
            setContributions(JSON.parse(cached));
        }
    }, [teamId]);

    const handleBoardCreated = (newBoard: any) => {
        if (!newBoard) return;

        setTeam(currentTeam => {
            if (!currentTeam) return currentTeam;

            const updatedBoards = [...(currentTeam.boards || [])];
            const boardExists = updatedBoards.some(b => b.id === newBoard.id);

            if (!boardExists) {
                updatedBoards.push(newBoard);
            }

            return {
                ...currentTeam,
                boards: updatedBoards
            };
        });
    };

    const handleTeamUpdated = (updatedTeam: any) => {
        if (!updatedTeam) return;

        setTeam(currentTeam => {
            if (!currentTeam) return updatedTeam;

            return {
                ...currentTeam,
                ...updatedTeam,
                boards: currentTeam.boards || []
            };
        });
    };

    const handleBoardUpdated = (updatedBoard: any) => {
        if (!updatedBoard) return;

        setTeam(currentTeam => {
            if (!currentTeam) return currentTeam;

            const updatedBoards = currentTeam.boards.map(board =>
                board.id === updatedBoard.id ? updatedBoard : board
            );

            return {
                ...currentTeam,
                boards: updatedBoards
            };
        });
    };

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <AppLayout>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    <Link to="/teams">
                        <Button type="text" icon={<ArrowLeftOutlined />}>
                            Back
                        </Button>
                    </Link>

                    <Title level={2} style={{ margin: 0 }}>{team.name} Boards</Title>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <Button
                        icon={<ReloadOutlined />}
                        onClick={handleRefresh}
                        loading={refreshLoading}
                        disabled={loading || refreshLoading}
                    >
                        Refresh
                    </Button>
                    <DynamicForm
                        formTitle="Edit team"
                        schemaName="TeamUpdate"
                        apiUrl={`team/${teamId}`}
                        type='patch'
                        onSuccess={handleTeamUpdated}
                        trigger={
                            <Button>
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
                            <Button danger icon={<DeleteOutlined />} type="primary">
                                Delete Team
                            </Button>
                        </Popconfirm>
                    )}
                    <DynamicForm
                        formTitle="Create board"
                        schemaName="BoardCreate"
                        apiUrl={`board/${teamId}/with-columns`}
                        type='post'
                        onSuccess={handleBoardCreated}
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
                                        marginBottom: '12px'
                                    }}>
                                        {board.name}
                                    </p>
                                    <p style={{ marginTop: '8px' }}>
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
                                    onSuccess={handleBoardUpdated}
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
                                        e.stopPropagation();
                                        handleDeleteBoard(board.id);
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
                                    />
                                </Popconfirm>
                            </div>
                        </div>
                    </List.Item>
                )}
                locale={{ emptyText: "No boards found. Create a new board to get started." }}
            />
            <Card
                title="Team Contributions"
                extra={
                    <Button onClick={fetchContributions} loading={loadingContributions}>
                        Refresh Contributions
                    </Button>
                }
                style={{ marginTop: 40 }}
            >
                <Table
                    dataSource={contributions.contributions}
                    rowKey="memberId"
                    pagination={false}
                    columns={[
                        {
                            title: 'Username',
                            dataIndex: 'username',
                            key: 'username'
                        },
                        {
                            title: 'Completed Jobs',
                            dataIndex: 'completedJobs',
                            key: 'completedJobs'
                        },
                        {
                            title: 'Contribution',
                            key: 'contribution',
                            render: (_, record) => {
                                const percent = contributions.totalJobs > 0
                                    ? Math.round((record.completedJobs / contributions.totalJobs) * 100)
                                    : 0;
                                return `${percent}%`;
                            }
                        }
                    ]}
                />
            </Card>
        </AppLayout>
    );
};

export default TeamBoards;