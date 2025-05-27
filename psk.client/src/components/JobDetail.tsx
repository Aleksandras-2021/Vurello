import React, { useState, useEffect } from 'react';
import { Modal, Tabs, Button, List, Typography, Empty } from 'antd';
import { SwapOutlined, ClockCircleOutlined, UserOutlined } from '@ant-design/icons';
import DynamicForm from '../components/DynamicForm';
import MoveJobBoardModal from '../components/MoveJobBoardModal';
import { api } from "../components/API";
import { RJSFSchema, UiSchema } from '@rjsf/utils';

const { Text } = Typography;

interface JobDetailProps {
    open: boolean;
    onCancel: () => void;
    job: any;
    labels: any;
    teamId: string;
    teamMembers: { value: string; label: string }[];
    onSuccess: () => void;
    updateJob: (job: any) => void;
}

const JobDetail: React.FC<JobDetailProps> = ({
                                                 open,
                                                 onCancel,
                                                 job,
                                                 labels,
                                                 teamId,
                                                 teamMembers,
                                                 onSuccess,
                                                 updateJob
                                             }) => {
    const [activeTab, setActiveTab] = useState('1');
    const [boardId, setBoardId] = useState<string>("");
    const [columns, setColumns] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);
    const [moveModalVisible, setMoveModalVisible] = useState(false);
    const [currentJob, setCurrentJob] = useState(job);

    const handleTabChange = (key: string) => {
        setActiveTab(key);
    };

    // Fetch latest job data when modal opens or when switching to history tab
    const fetchLatestJobData = async () => {
        try {
            const response = await api.get(`job/${job.id}`);
            setCurrentJob(response.data);
            updateJob(response.data);
        } catch (error) {
            console.error('Failed to fetch latest job data:', error);
        }
    };

    const handleJobUpdated = (updatedJob: any) => {
        if (!updatedJob) return;
        setCurrentJob(updatedJob);
        updateJob(updatedJob);
        // Optionally fetch latest data to get updated history
        if (activeTab === '3') {
            fetchLatestJobData();
        }
    };

    const handleLabelsUpdated = (response: any) => {
        if (!response) return;

        const updatedJob = {
            ...currentJob,
            labels: response.labels || currentJob.labels,
            version: response.version
        };

        setCurrentJob(updatedJob);
        updateJob(updatedJob);
        // Optionally fetch latest data to get updated history
        if (activeTab === '3') {
            fetchLatestJobData();
        }
    };

    const handleConflictCancelled = (latestData?: any) => {
        if (latestData) {
            setCurrentJob(latestData);
            updateJob(latestData);
        }
    };

    const handleMoveSuccess = () => {
        setMoveModalVisible(false);
        onSuccess();
        onCancel();
    };

    // Update current job when prop changes
    useEffect(() => {
        setCurrentJob(job);
    }, [job]);

    // Fetch latest data when switching to history tab
    useEffect(() => {
        if (activeTab === '3' && open) {
            fetchLatestJobData();
        }
    }, [activeTab, open]);

    useEffect(() => {
        setBoardId(job.boardId);
        if (job.boardId) {
            fetchBoardColumns(job.boardId);
        }
    }, [job]);

    const fetchBoardColumns = async (boardId: string) => {
        setLoading(true);
        try {
            const columnsResponse = await api.get(`board-column/board/${boardId}`);
            setColumns(columnsResponse.data);
        } catch (error) {
            console.error('Failed to fetch board columns:', error);
        } finally {
            setLoading(false);
        }
    };

    const columnOptions = columns.map(column => ({
        value: column.id,
        label: column.name
    }));

    const combinedDropdownOptions = {
        "Column": columnOptions,
        "Assigned Member": teamMembers
    };

    const labelSchema: RJSFSchema = {
        $schema: 'http://json-schema.org/draft-07/schema#',
        type: 'object',
        required: ['labels'],
        properties: {
            labels: {
                type: 'array',
                items: {
                    type: 'string',
                },
                uniqueItems: true,
                default: [],
            },
        },
    };

    const labelUiSchema: UiSchema = {
        labels: {
            'ui:widget': 'labelSelection',
            'ui:options': {
                labels: labels,
                label: false,
            },
            'ui:title': '',
            'ui:classNames': 'hide-label',
        },
    };

    const formatTimestamp = (timestamp: string) => {
        return new Date(timestamp).toLocaleString();
    };

    const tabItems = [
        {
            key: '1',
            label: 'Info',
            children: (
                <div style={{ padding: 20 }}>
                    {loading ? (
                        <div>Loading...</div>
                    ) : (
                        <DynamicForm
                            formTitle={`Job: ${currentJob.name}`}
                            schemaName="JobUpdate"
                            apiUrl={`job/${currentJob.id}`}
                            type="patch"
                            onSuccess={handleJobUpdated}
                            currentData={currentJob}
                            dropdownOptions={combinedDropdownOptions}
                            noModal={true}
                            onCancelConflict={handleConflictCancelled}
                            fetchCurrentData={() => api.get(`job/${currentJob.id}`).then(res => res.data)}
                        />
                    )}
                </div>
            ),
        },
        {
            key: '2',
            label: 'Labels',
            children: (
                <div style={{ padding: 20 }}>
                    <DynamicForm
                        formTitle=""
                        schema={labelSchema}
                        uiSchema={labelUiSchema}
                        apiUrl={`job/${currentJob.id}/labels`}
                        type="put"
                        currentData={{
                            labels: Array.isArray(currentJob.labels) ? currentJob.labels.map(l => l.id).filter(id => typeof id === 'string') : [],
                            version: currentJob.version
                        }}
                        onSuccess={handleLabelsUpdated}
                        noModal={true}
                        fetchCurrentData={async () => {
                            const latestJob = await api.get(`job/${currentJob.id}`).then(res => res.data);
                            return {
                                labels: Array.isArray(latestJob.labels) ? latestJob.labels.map(l => l.id).filter(id => typeof id === 'string') : [],
                                version: latestJob.version
                            };
                        }}
                        onCancelConflict={handleConflictCancelled}
                    />
                </div>
            ),
        },
        {
            key: '3',
            label: 'History',
            children: (
                <div style={{ padding: 20 }}>
                    <div style={{ marginBottom: 16 }}>
                        <Button onClick={fetchLatestJobData} size="small">
                            Refresh History
                        </Button>
                    </div>
                    {currentJob.jobHistories && currentJob.jobHistories.length > 0 ? (
                        <List
                            dataSource={currentJob.jobHistories.sort((a: any, b: any) =>
                                new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
                            )}
                            renderItem={(historyItem: any) => (
                                <List.Item>
                                    <List.Item.Meta
                                        avatar={<UserOutlined />}
                                        title={
                                            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                                                <Text strong>{historyItem.user?.userName || 'Unknown User'}</Text>
                                                <Text type="secondary" style={{ fontSize: '12px' }}>
                                                    <ClockCircleOutlined style={{ marginRight: 4 }} />
                                                    {formatTimestamp(historyItem.timestamp)}
                                                </Text>
                                            </div>
                                        }
                                        description={historyItem.changeMessage}
                                    />
                                </List.Item>
                            )}
                        />
                    ) : (
                        <Empty
                            description="No edit history available"
                            style={{ marginTop: 40 }}
                        />
                    )}
                </div>
            ),
        },
    ];

    return (
        <>
            <Modal
                open={open}
                onCancel={onCancel}
                footer={null}
                width={800}
                style={{ top: 20 }}
            >
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
                    <h2 style={{ margin: 0 }}>Job Details</h2>
                    <Button
                        icon={<SwapOutlined />}
                        onClick={() => setMoveModalVisible(true)}
                    >
                        Move to Board
                    </Button>
                </div>

                <div style={{ display: 'flex', height: '70vh' }}>
                    <div style={{ minWidth: 120, borderRight: '1px solid #f0f0f0' }}>
                        <Tabs
                            tabPosition="left"
                            activeKey={activeTab}
                            onChange={handleTabChange}
                            items={tabItems.map(({ key, label }) => ({ key, label }))}
                            tabBarGutter={0}
                            tabBarStyle={{ fontSize: '16px', fontWeight: 500 }}
                        />
                    </div>
                    <div style={{ flex: 1, overflowY: 'auto', padding: '16px' }}>
                        {tabItems.map(tab => (
                            <div
                                key={tab.key}
                                style={{
                                    display: activeTab === tab.key ? 'block' : 'none',
                                    visibility: activeTab === tab.key ? 'visible' : 'hidden',
                                }}
                            >
                                {tab.children}
                            </div>
                        ))}
                    </div>
                </div>
            </Modal>

            <MoveJobBoardModal
                open={moveModalVisible}
                onCancel={() => setMoveModalVisible(false)}
                job={currentJob}
                currentTeamId={teamId}
                onSuccess={handleMoveSuccess}
            />
        </>
    );
};

export default JobDetail;