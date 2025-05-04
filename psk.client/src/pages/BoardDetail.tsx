import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Card, Typography, Spin, Button, List, Row, Col, Tag } from 'antd'; import { PlusOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";
import { EditOutlined } from '@ant-design/icons';

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
        <div style={{ padding: 24 }}>
            <Row align="middle" style={{ marginBottom: 20 }}>
                <Col>
                    <Link to={`/teams/${board?.teamId}`}>
                        <Button type="text" icon={<ArrowLeftOutlined />}>
                            Back to Team
                        </Button>
                    </Link>
                </Col>
                <Col flex="auto">
                    <Title level={2}>{board?.name}</Title>
                </Col>
                <Col>
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
                </Col>
            </Row>

            {jobs.length === 0 ? (
                <Text>No tasks found. Create a new task to get started.</Text>
            ) : (
                <List
                    grid={{ gutter: 16, column: 3 }}
                    dataSource={jobs}
                    renderItem={(job: any) => (
                        <List.Item>
                            <Card
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
                                    </Row>
                                }
                            >
                                <p>{job.description}</p>
                                <p>
                                    <strong>Assigned to:</strong>{' '}
                                    {job.assignedMemberId
                                        ? teamMembers.find((member) => member.value === job.assignedMemberId)?.label || 'Unknown'
                                        : 'Unassigned'}
                                </p>
                            </Card>
                        </List.Item>
                    )}
                />
            )}
        </div>
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