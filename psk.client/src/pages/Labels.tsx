import React, { useState, useEffect } from 'react';
import { Button, Card, Table, Space, Typography, Input, Popconfirm, Modal, Tag, Spin, Alert } from 'antd';
import { DeleteOutlined, PlusOutlined, EditOutlined, SearchOutlined, ArrowLeftOutlined, ReloadOutlined } from '@ant-design/icons';
import { api } from '../components/API';
import { toast } from 'react-toastify';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import DynamicForm from '../components/DynamicForm';
import { Link, useNavigate } from 'react-router-dom';

const { Title } = Typography;

interface Label {
    id: string;
    text: string;
    textColor: string;
    backgroundColor: string;
    teamId: string;
    team?: { name: string };
    jobs: any[];
    version: number;
}

const Labels: React.FC = () => {
    const [labels, setLabels] = useState<Label[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchText, setSearchText] = useState('');
    const [jobsModalVisible, setJobsModalVisible] = useState(false);
    const [selectedLabel, setSelectedLabel] = useState<Label | null>(null);
    const [currentTeam, setCurrentTeam] = useState<any>(null);
    const [teamLoading, setTeamLoading] = useState(false);
    const [refreshLoading, setRefreshLoading] = useState(false);
    const { lastTeamId } = useAppContext();
    const navigate = useNavigate();

    const fetchCurrentTeam = async () => {
        if (!lastTeamId) return;

        try {
            setTeamLoading(true);
            const response = await api.get(`team/${lastTeamId}`);
            setCurrentTeam(response.data);
        } catch (error) {
            console.error('Failed to fetch current team:', error);
            toast.error('Failed to load team information');
        } finally {
            setTeamLoading(false);
        }
    };

    const fetchLabels = async () => {
        if (!lastTeamId) {
            setLoading(false);
            return;
        }

        try {
            setLoading(true);
            const response = await api.get(`label/team/${lastTeamId}`);
            setLabels(response.data);
        } catch (error) {
            console.error('Failed to fetch labels:', error);
            toast.error('Failed to load labels');
        } finally {
            setLoading(false);
        }
    };

    const handleRefresh = async () => {
        setRefreshLoading(true);
        try {
            await Promise.all([fetchLabels(), fetchCurrentTeam()]);
            toast.success('Data refreshed successfully');
        } catch (error) {
            toast.error('Failed to refresh data');
        } finally {
            setRefreshLoading(false);
        }
    };

    useEffect(() => {
        if (lastTeamId) {
            fetchCurrentTeam();
            fetchLabels();
        } else {
            setLoading(false);
        }
    }, [lastTeamId]);

    const handleLabelCreated = (newLabel: Label) => {
        if (!newLabel) return;

        if (newLabel.teamId === lastTeamId) {
            setLabels(currentLabels => {
                if (currentLabels.some(label => label.id === newLabel.id)) {
                    return currentLabels.map(label =>
                        label.id === newLabel.id ? newLabel : label
                    );
                } else {
                    return [...currentLabels, newLabel];
                }
            });
        }
    };

    const handleLabelUpdated = (updatedLabel: Label) => {
        if (!updatedLabel) return;

        setLabels(currentLabels =>
            currentLabels.map(label =>
                label.id === updatedLabel.id ? updatedLabel : label
            )
        );

        if (selectedLabel && selectedLabel.id === updatedLabel.id) {
            setSelectedLabel(updatedLabel);
        }
    };

    const handleLabelConflictCancelled = (latestData?: any) => {
        if (!latestData) return;

        setLabels(prev =>
            prev.map(label =>
                label.id === latestData.id ? latestData : label
            )
        );

        if (selectedLabel && selectedLabel.id === latestData.id) {
            setSelectedLabel(latestData);
        }
    };

    const handleDeleteLabel = async (labelId: string) => {
        try {
            await api.delete(`label/${labelId}`);

            setLabels(currentLabels =>
                currentLabels.filter(label => label.id !== labelId)
            );

            if (selectedLabel && selectedLabel.id === labelId) {
                setSelectedLabel(null);
                setJobsModalVisible(false);
            }

        } catch (error) {
            console.error('Failed to delete label:', error);
            toast.error('Failed to delete label');
        }
    };

    const showJobsModal = async (label: Label) => {
        setSelectedLabel(label);

        try {
            const response = await api.get(`label/${label.id}`);
            setSelectedLabel(response.data);
            setJobsModalVisible(true);
        } catch (error) {
            console.error('Failed to fetch label details:', error);
            toast.error('Failed to load label details');
        }
    };

    const filteredLabels = labels.filter(label =>
        label.text.toLowerCase().includes(searchText.toLowerCase())
    );

    if (!lastTeamId) {
        return (
            <AppLayout>
                <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
                    <Alert
                        message="No Team Selected"
                        description={
                            <div>
                                <p>Please select a team first to manage labels.</p>
                                <Button type="primary" onClick={() => navigate('/teams')}>
                                    Go to Teams
                                </Button>
                            </div>
                        }
                        type="info"
                        showIcon
                        style={{ maxWidth: 400 }}
                    />
                </div>
            </AppLayout>
        );
    }

    const columns = [
        {
            title: 'Label',
            key: 'label',
            render: (record: Label) => (
                <Tag
                    color={record.backgroundColor}
                    style={{
                        color: record.textColor,
                        borderColor: 'transparent',
                        fontSize: '14px',
                        padding: '4px 8px'
                    }}
                >
                    {record.text}
                </Tag>
            ),
        },
        {
            title: 'Text Color',
            dataIndex: 'textColor',
            key: 'textColor',
            render: (text: string) => (
                <div style={{
                    width: '24px',
                    height: '24px',
                    backgroundColor: text,
                    border: '1px solid #d9d9d9',
                    borderRadius: '4px'
                }} />
            ),
        },
        {
            title: 'Background Color',
            dataIndex: 'backgroundColor',
            key: 'backgroundColor',
            render: (text: string) => (
                <div style={{
                    width: '24px',
                    height: '24px',
                    backgroundColor: text,
                    border: '1px solid #d9d9d9',
                    borderRadius: '4px'
                }} />
            ),
        },
        {
            title: 'Jobs Count',
            key: 'jobsCount',
            render: (record: Label) => (
                <span>{record.jobs?.length || 0}</span>
            ),
        },
        {
            title: 'Actions',
            key: 'actions',
            render: (_, record: Label) => (
                <Space>
                    <Button
                        type="link"
                        onClick={() => showJobsModal(record)}
                        disabled={!record.jobs || record.jobs.length === 0}
                    >
                        View Jobs ({record.jobs?.length || 0})
                    </Button>
                    <DynamicForm
                        formTitle="Edit Label"
                        schemaName="LabelUpdate"
                        apiUrl={`label/${record.id}`}
                        type="patch"
                        onSuccess={handleLabelUpdated}
                        trigger={
                            <Button
                                type="text"
                                icon={<EditOutlined />}
                            />
                        }
                        currentData={record}
                        fetchCurrentData={() => api.get(`label/${record.id}`).then(res => res.data)}
                        onCancelConflict={handleLabelConflictCancelled}
                    />
                    <Popconfirm
                        title="Delete Label"
                        description="Are you sure you want to delete this label? It will be removed from all jobs."
                        onConfirm={() => handleDeleteLabel(record.id)}
                        okText="Yes"
                        cancelText="No"
                        okButtonProps={{ danger: true }}
                    >
                        <Button
                            type="text"
                            danger
                            icon={<DeleteOutlined />}
                        />
                    </Popconfirm>
                </Space>
            ),
        },
    ];

    const jobColumns = [
        {
            title: 'Name',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Status',
            dataIndex: 'status',
            key: 'status',
            render: (text: string) => {
                let color = 'default';
                if (text === 'In Progress') color = 'processing';
                else if (text === 'Done') color = 'success';
                return <Tag color={color}>{text}</Tag>;
            },
        },
        {
            title: 'Board',
            dataIndex: ['board', 'name'],
            key: 'boardName',
        },
        {
            title: 'Actions',
            key: 'jobActions',
            render: (_, record: any) => (
                <Button
                    type="link"
                    onClick={() => window.open(`/boards/${record.boardId}`, '_blank')}
                >
                    Go to Board
                </Button>
            ),
        }
    ];

    return (
        <AppLayout>
            <div style={{ marginBottom: 24 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <Link to={`/teams/${lastTeamId}`}>
                            <Button type="text" icon={<ArrowLeftOutlined />} style={{ marginRight: 10 }}>
                                Back to Team
                            </Button>
                        </Link>
                        <Title level={2} style={{ margin: 0 }}>
                            Label Management
                            {currentTeam && (
                                <span style={{ fontSize: '16px', fontWeight: 'normal', color: '#666', marginLeft: 8 }}>
                                    - {currentTeam.name}
                                </span>
                            )}
                        </Title>
                    </div>
                    <div style={{ display: 'flex', gap: 16 }}>
                        <Input
                            placeholder="Search labels"
                            prefix={<SearchOutlined />}
                            value={searchText}
                            onChange={e => setSearchText(e.target.value)}
                            style={{ width: 250 }}
                            allowClear
                        />
                        <Button
                            icon={<ReloadOutlined />}
                            onClick={handleRefresh}
                            loading={refreshLoading}
                            disabled={loading || teamLoading || refreshLoading}
                        >
                            Refresh
                        </Button>
                        <DynamicForm
                            formTitle="Create Label"
                            schemaName="LabelCreate"
                            apiUrl={`label/${lastTeamId}`}
                            type="post"
                            neededData={{ teamId: lastTeamId }}
                            onSuccess={handleLabelCreated}
                            trigger={
                                <Button type="primary" icon={<PlusOutlined />}>
                                    New Label
                                </Button>
                            }
                        />
                    </div>
                </div>

                {(loading || teamLoading) ? (
                    <div style={{ display: 'flex', justifyContent: 'center', padding: 100 }}>
                        <Spin size="large" />
                    </div>
                ) : (
                    <Table
                        dataSource={filteredLabels}
                        columns={columns}
                        rowKey="id"
                        pagination={{ pageSize: 10 }}
                        locale={{
                            emptyText: currentTeam
                                ? `No labels found for ${currentTeam.name}. Create a new label to get started.`
                                : "No labels found. Create a new label to get started."
                        }}
                    />
                )}
            </div>

            <Modal
                title={selectedLabel ? `Jobs using label: ${selectedLabel.text}` : 'Jobs'}
                open={jobsModalVisible}
                onCancel={() => setJobsModalVisible(false)}
                footer={null}
                width={800}
            >
                {selectedLabel && (
                    <>
                        <div style={{ marginBottom: 16 }}>
                            <Tag
                                color={selectedLabel.backgroundColor}
                                style={{
                                    color: selectedLabel.textColor,
                                    borderColor: 'transparent',
                                    fontSize: '14px',
                                    padding: '4px 8px',
                                    marginBottom: 16
                                }}
                            >
                                {selectedLabel.text}
                            </Tag>
                        </div>

                        <Table
                            dataSource={selectedLabel.jobs || []}
                            columns={jobColumns}
                            rowKey="id"
                            pagination={{ pageSize: 5 }}
                            locale={{ emptyText: "No jobs are using this label." }}
                        />
                    </>
                )}
            </Modal>
        </AppLayout>
    );
};

export default Labels;