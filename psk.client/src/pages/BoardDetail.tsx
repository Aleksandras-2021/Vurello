import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
    Card,
    Typography,
    Spin,
    Button,
    Row,
    Col,
    Tag,
    message,
    Popconfirm,
} from 'antd';
import {
    PlusOutlined,
    ArrowLeftOutlined,
    DeleteOutlined,
    ClockCircleOutlined,
    EditOutlined
} from '@ant-design/icons';
import { useDrag, useDrop, DndProvider } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import DynamicForm from '../components/DynamicForm';
import { api } from '../components/API';
import AppLayout from '../components/AppLayout';

const { Title, Text } = Typography;


const ItemTypes = {
    CARD: 'card',
};

const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
        case 'to do':
            return 'blue';
        case 'in progress':
            return 'orange';
        case 'done':
            return 'green';
        default:
            return 'default';
    }
};

const isDeadlineOverdue = (deadline: string | null): boolean => {
    if (!deadline) return false;
    const deadlineDate = new Date(deadline);
    return deadlineDate < new Date();
};

const formatDeadlineDate = (deadline: string | null): string => {
    if (!deadline) return '';
    const date = new Date(deadline);
    return `Due: ${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
};

const DraggableJobCard = ({ job, teamMembers, onDrop, boardId }) => {
    const [{ isDragging }, drag] = useDrag(() => ({
        type: ItemTypes.CARD,
        item: { id: job.id, currentStatus: job.status },
        collect: (monitor) => ({ isDragging: monitor.isDragging() }),
    }));

    const overdue = isDeadlineOverdue(job.deadline);
    const deadlineText = formatDeadlineDate(job.deadline);

    return (
        <div ref={drag} style={{ opacity: isDragging ? 0.5 : 1 }}>
            <Card
                style={{ marginBottom: 16 }}
                title={job.name}
                extra={
                    <Row gutter={8}>
                        <Col>
                            <Tag color={getStatusColor(job.status)}>{job.status}</Tag>
                        </Col>
                        <Col>
                            <DynamicForm
                                formTitle={`Edit Job: ${job.name}`}
                                schemaName="JobUpdate"
                                apiUrl={`job/${job.id}`}
                                type="patch"
                                onSuccess={() => onDrop()}
                                trigger={<Button icon={<EditOutlined />} size="small" type="text" />}
                                neededData={{ boardId }}
                                currentData={job}
                                dropdownOptions={teamMembers}
                            />
                        </Col>
                        <Col>
                            <Popconfirm
                                title="Delete Job"
                                description="Are you sure you want to delete this job? This action cannot be undone."
                                onConfirm={(e) => {
                                    e.stopPropagation();
                                    onDrop(job.id, true);
                                }}
                                okText="Yes, Delete"
                                cancelText="Cancel"
                                okButtonProps={{ danger: true }}
                            >
                                <Button danger icon={<DeleteOutlined />} size="small" type="text" />
                            </Popconfirm>
                        </Col>
                    </Row>
                }
            >
                <p>{job.description}</p>
                {job.assignedMemberId && (
                    <p>
                        <strong>Assigned to:</strong> {teamMembers.find((m) => m.value === job.assignedMemberId)?.label || 'Unknown'}
                    </p>
                )}
                {job.deadline && (
                    <p>
                        <ClockCircleOutlined style={{ marginRight: 8, color: overdue ? '#ff4d4f' : 'inherit' }} />
                        <Text style={{ color: overdue ? '#ff4d4f' : 'inherit', fontWeight: overdue ? 'bold' : 'normal' }}>
                            {deadlineText}
                        </Text>
                    </p>
                )}
            </Card>
        </div>
    );
};

const DroppableColumn = ({ status, jobs, onDrop, teamMembers, boardId }) => {
    const [{ isOver, canDrop }, drop] = useDrop(() => ({
        accept: ItemTypes.CARD,
        drop: (item) => {
            if (item.currentStatus !== status) {
                onDrop(item.id, false, status);
            }
        },
        collect: (monitor) => ({
            isOver: monitor.isOver(),
            canDrop: monitor.canDrop(),
        }),
    }), [status, onDrop]);

    const isActive = isOver && canDrop;

    return (
        <Col span={8} key={status}>
            <div
                ref={drop}
                style={{
                    minHeight: 'calc(100vh - 200px)',
                    padding: 8,
                    backgroundColor: isActive ? '#f0f0f0' : undefined,
                    transition: 'background-color 0.2s',
                }}
            >
                <Card title={status.toUpperCase()} bordered>
                    {jobs.length === 0 ? (
                        <Text type="secondary">No Jobs</Text>
                    ) : (
                        jobs.map((job) => (
                            <DraggableJobCard
                                key={job.id}
                                job={job}
                                teamMembers={teamMembers}
                                onDrop={onDrop}
                                boardId={boardId}
                            />
                        ))
                    )}
                </Card>
            </div>
        </Col>
    );
};


const BoardDetail = () => {
    const { boardId } = useParams();
    const [board, setBoard] = useState<any>(null);
    const [jobs, setJobs] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [teamMembers, setTeamMembers] = useState<any[]>([]);

    const fetchBoardData = async () => {
        setLoading(true);
        try {
            const boardResponse = await api.get(`board/${boardId}`);
            setBoard(boardResponse.data);

            const jobsResponse = await api.get(`job`, { params: { boardId } });
            setJobs(jobsResponse.data);
        } catch (error) {
            console.error('Failed to fetch board data:', error);
        } finally {
            setLoading(false);
        }
    };

    const fetchTeamMembers = async () => {
        if (!board?.teamId) return;
        try {
            const response = await api.get(`team/${board.teamId}/members`);
            const members = [
                { value: '00000000-0000-0000-0000-000000000000', label: 'Unassigned' },
                ...response.data.map((m) => ({ value: m.id, label: m.userName }))
            ];
            setTeamMembers(members);
        } catch (error) {
            console.error('Failed to fetch team members:', error);
        }
    };

    const updateJobStatus = async (jobId: string, deleteJob = false, newStatus: string | null = null) => {
        try {
            if (deleteJob) {
                await api.delete(`job/${jobId}`);
            } else if (newStatus) {
                await api.patch(`job/${jobId}`, { status: newStatus });
            }
            fetchBoardData();
        } catch (err) {
            message.error('Failed to update job');
        }
    };

    useEffect(() => { fetchBoardData(); }, [boardId]);
    useEffect(() => { if (board?.teamId) fetchTeamMembers(); }, [board?.teamId]);

    if (loading) return <Spin size="large" />;

    return (
        <AppLayout>
            <DndProvider backend={HTML5Backend}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <Link to={`/teams/${board?.teamId}`}>
                            <Button type="text" icon={<ArrowLeftOutlined />} style={{ marginRight: 10 }}>Back</Button>
                        </Link>
                        <Title level={2} style={{ margin: 0 }}>{board?.name}</Title>
                    </div>
                    <DynamicForm
                        formTitle="Create Job"
                        schemaName="JobCreate"
                        apiUrl="job"
                        type="post"
                        onSuccess={fetchBoardData}
                        trigger={<Button type="primary" icon={<PlusOutlined />}>Create Job</Button>}
                        neededData={{ boardId }}
                        dropdownOptions={teamMembers}
                    />
                </div>
                <Row gutter={16}>
                    {['to do', 'in progress', 'done'].map((status) => (
                        <DroppableColumn
                            key={status}
                            status={status}
                            jobs={jobs.filter((job) => job.status.toLowerCase() === status)}
                            onDrop={updateJobStatus}
                            teamMembers={teamMembers}
                            boardId={boardId}
                        />
                    ))}
                </Row>
            </DndProvider>
        </AppLayout>
    );
};

export default BoardDetail;
