import React, { useState, useEffect } from 'react';
import { Modal, Tabs, Button, Input } from 'antd';
import DynamicForm from '../components/DynamicForm';
import { api } from "../components/API";
interface JobDetailProps {
    open: boolean;
    onCancel: () => void;
    job: any;
    labels: any;
    teamId: string;
    teamMembers: { value: string; label: string }[];
    onSuccess: () => void;
}

const JobDetail: React.FC<JobDetailProps> = ({ open, onCancel, job, labels, teamId, teamMembers, onSuccess }) => {
    const [activeTab, setActiveTab] = useState('1');
    const [boardId, setBoardId] = useState<string>("");
    const [selectedLabels, setSelectedLabels] = useState<any[]>(job.labels || []);
    const [searchTerm, setSearchTerm] = useState('');

    const handleTabChange = (key: string) => {
        setActiveTab(key);
    };

    const toggleLabel = (label: any) => {
        const exists = selectedLabels.find((l: any) => l.id === label.id);
        if (exists) {
            setSelectedLabels(prev => prev.filter(l => l.id !== label.id));
        } else {
            setSelectedLabels(prev => [...prev, label]);
        }
    };

    const filteredLabels = labels.filter(
        (label: any) =>
            !selectedLabels.some((l: any) => l.id === label.id) &&
            label.text.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const saveSelectedLabels = async () => {
        try {
            await api.put(`job/${job.id}/labels`, selectedLabels.map((l: any) => l.id));
            onSuccess();
        } catch (err) {
            console.error('Failed to update labels', err);
        }
    };

    useEffect(() => {
        setBoardId(job.boardId);
        console.log(job);
    }, []);

    const tabItems = [
        {
            key: '1',
            label: 'Info',
            children: (
                <div style={{ padding: 20 }} >
                    <DynamicForm
                        formTitle={`Job: ${job.name}`}
                        schemaName="JobUpdate"
                        apiUrl={`job/${job.id}`}
                        type="patch"
                        onSuccess={onSuccess}
                        neededData={{ boardId }}
                        currentData={job}
                        dropdownOptions={teamMembers}
                        noModal={true}
                    />
                </div>
            ),
        },
        {
            key: '2',
            label: 'Labels',
            children: (
                <div style={{ padding: 20 }}>
        
                    <div>
                        <h4>Selected Labels</h4>
                        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginBottom: 16 }}>
                            {selectedLabels.map(label => (
                                <div
                                    key={label.id}
                                    onClick={() => toggleLabel(label)}
                                    style={{
                                        backgroundColor: label.backgroundColor,
                                        color: label.textColor,
                                        padding: '6px 12px',
                                        borderRadius: 8,
                                        cursor: 'pointer',
                                        boxShadow: '0 0 4px rgba(0,0,0,0.1)',
                                    }}
                                >
                                    {label.text}
                                </div>
                            ))}
                        </div>
                        {selectedLabels.length === 0 && (
                            <div style={{ color: '#888' }}>No selected labels</div>
                        )}
                    </div>


                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
                        <h4 style={{ margin: 0 }}>Unselected Labels</h4>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                            <Input.Search
                                placeholder="Search unselected labels"
                                value={searchTerm}
                                onChange={e => setSearchTerm(e.target.value)}
                                style={{ maxWidth: 300 }}
                            />
                        </div>
                    </div>

                    <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                        {filteredLabels.map(label => (
                            <div
                                key={label.id}
                                onClick={() => toggleLabel(label)}
                                style={{
                                    backgroundColor: label.backgroundColor,
                                    color: label.textColor,
                                    padding: '6px 12px',
                                    borderRadius: 8,
                                    cursor: 'pointer',
                                }}
                            >
                                {label.text}
                            </div>
                        ))}
                        {filteredLabels.length === 0 && (
                            <div style={{ color: '#888' }}>No matching labels</div>
                        )}
                    </div>

                    <div style={{ marginTop: 20 }}>
                        <Button
                            type="primary"
                            onClick={saveSelectedLabels}
                            style={{ marginRight: 20 }}
                        >
                            Save Labels
                        </Button>
                    </div>
                </div>
            )
        }
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
