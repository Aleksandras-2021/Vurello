import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Card, Typography, Spin, Button, List, Row, Col, Tag } from 'antd'; import { PlusOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";

const { Title, Text } = Typography;

const BoardDetail = () => {
    const { boardId } = useParams();
    const [board, setBoard] = useState<any>(null);
    const [jobs, setJobs] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

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

    useEffect(() => {
        fetchBoardData();
    }, [boardId]);

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
                                extra={<Tag color={getStatusColor(job.status)}>{job.status}</Tag>}
                            >
                                <p>{job.description}</p>
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