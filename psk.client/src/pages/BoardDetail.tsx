import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Card, Typography, Spin, Button, List, Row, Col, Tag, message, Popconfirm } from 'antd';
import { PlusOutlined, ArrowLeftOutlined, DeleteOutlined } from '@ant-design/icons';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";
import { EditOutlined } from '@ant-design/icons';
import AppLayout from '../components/AppLayout';

const { Title, Text } = Typography;

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

            const jobsResponse = await api.get(`job`, {
                params: { boardId }
            });
            setJobs(jobsResponse.data);
        } catch (error) {
            console.error('Failed to fetch board data:', error);
        } finally {
            setLoading(false);
        }
    };

    const fetchTeamMembers = async () => {
        try {
            if (!board?.teamId) return;

            const response = await api.get(`team/${board.teamId}/members`);

            const membersWithUnassigned = [
                { value: "00000000-0000-0000-0000-000000000000", label: "Unassigned" },
                ...response.data.map((member: any) => ({
                    value: member.id,
                    label: member.userName
                }))
            ];

            setTeamMembers(membersWithUnassigned);
        } catch (error) {
            console.error('Failed to fetch team members:', error);
        }
    };

    const handleDeleteJob = async (jobId: string) => {
        try {
            await api.delete(`job/${jobId}`);
            fetchBoardData();
        } catch (error) {
            console.error('Failed to delete job:', error);
        }
    };

    useEffect(() => {
        fetchBoardData();
    }, [boardId]);

    useEffect(() => {
        if (board?.teamId) {
            fetchTeamMembers();
        }
    }, [board?.teamId]);

    if (loading) {
        return <Spin size="large" />;
    }

    return (
        <AppLayout>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <Link to={`/teams/${board?.teamId}`}>
                        <Button type="text" icon={<ArrowLeftOutlined />} style={{ marginRight: 10 }}>
                            Back
                        </Button>
                    </Link>
                    <Title level={2} style={{ margin: 0 }}>{board?.name}</Title>
                </div>
                <div>
                    <DynamicForm
                        formTitle="Create Task"
                        schemaName="JobCreate"
                        apiUrl="job"
                        type="post"
                        onSuccess={fetchBoardData}
                        trigger={
                            <Button type="primary" icon={<PlusOutlined />}>
                                Create Task
                            </Button>
                        }
                        neededData={{ boardId }}
                        dropdownOptions={teamMembers}
                    />
                </div>
            </div>
            <Row gutter={16}>
                {['to do', 'in progress', 'done'].map((status) => (
                    <Col span={8} key={status}>
                        <Card title={status.toUpperCase()} bordered>
                            {jobs.filter(job => job.status.toLowerCase() === status).length === 0 ? (
                                <Text type="secondary">No tasks</Text>
                            ) : (
                                jobs
                                    .filter(job => job.status.toLowerCase() === status)
                                    .map((job) => (
                                        <Card
                                            key={job.id}
                                            style={{ marginBottom: 16 }}
                                            title={job.name}
                                            extra={
                                                <Row gutter={8}>
                                                    <Col>
                                                        <Tag color={getStatusColor(job.status)}>{job.status}</Tag>
                                                    </Col>
                                                    <Col>
                                                        <DynamicForm
                                                            formTitle={`Edit Task: ${job.name}`}
                                                            schemaName="JobUpdate"
                                                            apiUrl={`job/${job.id}`}
                                                            type="patch"
                                                            onSuccess={fetchBoardData}
                                                            trigger={
                                                                <Button
                                                                    icon={<EditOutlined />}
                                                                    size="small"
                                                                    type="text"
                                                                />
                                                            }
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
                                                                e.stopPropagation()
                                                                handleDeleteJob(job.id)
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
                                                    </Col>
                                                </Row>
                                            }
                                        >
                                            <p>{job.description}</p>
                                            {job.assignedMemberId && (
                                                <p>
                                                    <strong>Assigned to:</strong>{' '}
                                                    {teamMembers.find((member) => member.value === job.assignedMemberId)?.label || 'Unknown'}
                                                </p>
                                            )}
                                        </Card>
                                    ))
                            )}
                        </Card>
                    </Col>
                ))}
            </Row>
        </AppLayout>
    );
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

export default BoardDetail;