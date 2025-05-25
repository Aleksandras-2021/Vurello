import React, { useState, useEffect } from 'react';
import { Modal, Tabs, Button } from 'antd';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";
import { RJSFSchema, UiSchema } from '@rjsf/utils';

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

const JobDetail: React.FC<JobDetailProps> = ({ open, onCancel, job, labels, teamId, teamMembers, onSuccess, updateJob }) => {
    const [activeTab, setActiveTab] = useState('1');
    const [boardId, setBoardId] = useState<string>("");
    const [columns, setColumns] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);

    const handleTabChange = (key: string) => {
        setActiveTab(key);
    };

    const handleJobUpdated = (updatedJob: any) => {
        if (!updatedJob) return;
        updateJob(updatedJob);
    };

    const handleLabelsUpdated = (response: any) => {
        if (!response) return;

        const updatedJob = {
            ...job,
            labels: response.labels || job.labels,
            version: response.version
        };

        updateJob(updatedJob);
    };

    const handleConflictCancelled = (latestData?: any) => {
        if (latestData) {
            updateJob(latestData);
        }
    };

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
    };;

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

    const tabItems = [
        {
            key: '1',
            label: 'Info',
            children: (
                <div style={{ padding: 20 }} >
                {loading ? (
                    <div>Loading...</div>
                ) : (
                    <DynamicForm
                        formTitle={`Job: ${job.name}`}
                        schemaName="JobUpdate"
                        apiUrl={`job/${job.id}`}
                        type="patch"
                        onSuccess={handleJobUpdated} 
                        currentData={job}
                        dropdownOptions={combinedDropdownOptions}  
                        noModal={true}
                        onCancelConflict={handleConflictCancelled}
                        fetchCurrentData={() => api.get(`job/${job.id}`).then(res => res.data)}
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
                        apiUrl={`job/${job.id}/labels`}
                        type="put"
                        currentData={{
                            labels: Array.isArray(job.labels) ? job.labels.map(l => l.id).filter(id => typeof id === 'string') : [],
                            version: job.version
                        }}
                        onSuccess={handleLabelsUpdated}
                        noModal={true}
                        fetchCurrentData={async () => {
                            const latestJob = await api.get(`job/${job.id}`).then(res => res.data);
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
    ];

    return (
        <Modal
            open={open}
            onCancel={onCancel}
            footer={null}
            width={800}
            style={{ top: 20 }}
        >
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
    );
};

export default JobDetail;  