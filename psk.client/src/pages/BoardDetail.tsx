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
import { toast } from 'react-toastify';
import JobDetail from '../components/JobDetail';


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

const DraggableJobCard = ({ job, teamMembers, onDrop, boardId, setSelectedJob }) => {
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
                hoverable
                onClick={() => setSelectedJob(job)}
                style={{ marginBottom: 16 }}
                title={<span style={{ fontWeight: 600 }}>{job.name}</span>}
                extra={
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
                        <Button
                            danger
                            icon={<DeleteOutlined />}
                            size="small"
                            type="text"
                            onClick={(e) => e.stopPropagation()}
                        />
                    </Popconfirm>
                }
            >
                <Row gutter={[8, 8]} align="middle" wrap>
                    <Col>
                        <Tag color={getStatusColor(job.status)}>{job.status}</Tag>
                    </Col>

                    {job.labels &&
                        job.labels.slice(0, 3).map((label, index) => (
                            <Col key={index}>
                                <Tag
                                    color={label.backgroundColor}
                                    style={{
                                        color: label.textColor,
                                        wordWrap: 'break-word',
                                        whiteSpace: 'normal',
                                        maxWidth: '200px',
                                    }}
                                >
                                    {label.text}
                                </Tag>
                            </Col>
                        ))}

                    {job.labels && job.labels.length > 3 && (
                        <Col>
                            <Tag>And {job.labels.length - 3} more</Tag>
                        </Col>
                    )}
                </Row>

                {job.assignedMemberId && (
                    <p style={{ marginTop: 12 }}>
                        <strong>Assigned to:</strong>{' '}
                        {teamMembers.find((m) => m.value === job.assignedMemberId)?.label || 'Unknown'}
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

const DroppableColumn = ({ status, jobs, onDrop, teamMembers, boardId, setSelectedJob }) => {
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
                                setSelectedJob={setSelectedJob}
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
    const [selectedJob, setSelectedJob] = useState<any | null>(null);

    const fetchBoardData = async () => {
        setLoading(true);
        try {
            const boardResponse = await api.get(`board/${boardId}`);
            setBoard(boardResponse.data);
            setJobs(boardResponse.data.jobs);
            if (selectedJob) {
                const newSelectedJob = boardResponse.data.jobs.find(job => job.id === selectedJob.id);
                setSelectedJob(newSelectedJob || null);
            }
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
                toast.success("Job deleted successfully");
            } else if (newStatus) {
                await api.patch(`job/${jobId}`, { status: newStatus });
                toast.success(`Job moved to ${newStatus} successfully`);
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
                    {['To Do', 'In Progress', 'Done'].map((status) => (
                        <DroppableColumn
                            key={status}
                            status={status}
                            jobs={jobs.filter((job) => job.status === status)}
                            onDrop={updateJobStatus}
                            teamMembers={teamMembers}
                            boardId={boardId}
                            setSelectedJob={setSelectedJob}
                        />
                    ))}
                </Row>
                {selectedJob && (
                    <JobDetail
                        open={!!selectedJob}
                        onCancel={() => setSelectedJob(null)}
                        job={selectedJob}
                        labels={board.team.labels}
                        teamId={board.teamId}
                        teamMembers={teamMembers}
                        onSuccess={fetchBoardData}
                    />
                )}
            </DndProvider>
        </AppLayout>
    );
};

export default BoardDetail;
