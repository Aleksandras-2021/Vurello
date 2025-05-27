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
    Modal,
    Tooltip
} from 'antd';
import {
    PlusOutlined,
    ArrowLeftOutlined,
    DeleteOutlined,
    ClockCircleOutlined,
    MenuOutlined,
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
    COLUMN: 'column'
};

const getStatusColor = (status) => {
    return status.color || "#1890ff";
};

const isDeadlineOverdue = (deadline) => {
    if (!deadline) return false;
    const deadlineDate = new Date(deadline);
    return deadlineDate < new Date();
};

const formatDeadlineDate = (deadline) => {
    if (!deadline) return '';
    const date = new Date(deadline);
    return `Due: ${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`;
};

const calculateDaysLeft = (deadline) => {
    if (!deadline) return null;

    const deadlineDate = new Date(deadline);
    const currentDate = new Date();

    deadlineDate.setHours(0, 0, 0, 0);
    currentDate.setHours(0, 0, 0, 0);

    const timeDiff = deadlineDate.getTime() - currentDate.getTime();
    const daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));

    return daysDiff;
};

const formatDaysLeft = (deadline) => {
    if (!deadline) return '';

    const daysLeft = calculateDaysLeft(deadline);

    if (daysLeft === null) return '';

    if (daysLeft > 0) {
        return `(${daysLeft} day${daysLeft === 1 ? '' : 's'} left)`;
    } else if (daysLeft === 0) {
        return '(Due today)';
    } else {
        const daysOverdue = Math.abs(daysLeft);
        return `(${daysOverdue} day${daysOverdue === 1 ? '' : 's'} overdue)`;
    }
};


function getContrastColor(hexColor) {
    if (!hexColor || !hexColor.match(/^#[0-9A-F]{6}$/i)) {
        return '#000000';
    }

    const r = parseInt(hexColor.slice(1, 3), 16);
    const g = parseInt(hexColor.slice(3, 5), 16);
    const b = parseInt(hexColor.slice(5, 7), 16);

    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    return luminance > 0.5 ? '#000000' : '#ffffff';
}

const DraggableJobCard = ({ job, teamMembers, onDrop, boardId, setSelectedJob }) => {
    const [{ isDragging }, drag] = useDrag(() => ({
        type: ItemTypes.CARD,
        item: { id: job.id, currentStatus: job.status, currentColumnId: job.columnId },
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
                        <Tag color={job.column?.color || "#1890ff"}>{job.status}</Tag>
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
                    <div style={{ marginTop: 12 }}>
                        <div style={{ display: 'flex', alignItems: 'center' }}>
                            <ClockCircleOutlined style={{ marginRight: 8, color: overdue ? '#ff4d4f' : 'inherit' }} />
                            <Text style={{ color: overdue ? '#ff4d4f' : 'inherit', fontWeight: overdue ? 'bold' : 'normal' }}>
                                {deadlineText}
                            </Text>
                        </div>
                        <Text
                            style={{
                                color: overdue ? '#ff4d4f' : 'inherit',
                                fontWeight: overdue ? 'bold' : 'normal',
                                fontSize: '12px',
                                marginLeft: '24px',
                                display: 'block'
                            }}
                        >
                            {formatDaysLeft(job.deadline)}
                        </Text>
                    </div>
                )}
            </Card>
        </div>
    );
};

const DraggableColumn = ({
                             column,
                             jobs,
                             onDrop,
                             teamMembers,
                             boardId,
                             setSelectedJob,
                             index,
                             moveColumnVisually,
                             onColumnDragEnd,
                             onColumnEdit,
                             onColumnDelete
                         }) => {
    const dragRef = React.useRef(null);

    const [{ isDragging }, drag] = useDrag(() => ({
        type: ItemTypes.COLUMN,
        item: { index, id: column.id },
        end: (item, monitor) => {
            const dropResult = monitor.getDropResult();
            if (dropResult && typeof dropResult.index === 'number') {
                const fromIndex = item.index;
                const toIndex = dropResult.index;
                if (fromIndex !== toIndex) {
                    moveColumnVisually(fromIndex, toIndex);
                    onColumnDragEnd();
                }
            }
        },
        collect: (monitor) => ({
            isDragging: monitor.isDragging()
        }),
    }), [index, column.id, moveColumnVisually, onColumnDragEnd]);

    const [{ isOver, isOverColumn }, dropRef] = useDrop({
        accept: [ItemTypes.CARD, ItemTypes.COLUMN],
        drop: (item, monitor) => {
            const type = monitor.getItemType();

            if (type === ItemTypes.CARD) {
                if (item.currentColumnId !== column.id) {
                    onDrop(item.id, false, column.id);
                }
            } else if (type === ItemTypes.COLUMN) {
                return { index };
            }
        },
        collect: (monitor) => ({
            isOver: monitor.isOver() && monitor.getItemType() === ItemTypes.CARD,
            isOverColumn: monitor.isOver() && monitor.getItemType() === ItemTypes.COLUMN
        }),
    });

    drag(dragRef);

    return (
        <Col span={6}>
            <div
                ref={dropRef}
                style={{
                    minHeight: '300px',
                    opacity: isDragging ? 0.5 : 1,
                    backgroundColor: isOver
                        ? '#f0f0f0'
                        : isOverColumn
                            ? '#e6f7ff'
                            : 'transparent',
                    border: isOverColumn ? '2px dashed #1890ff' : undefined,
                    borderRadius: 4,
                    transition: 'background-color 0.2s, opacity 0.2s',
                    padding: 8
                }}
            >
                <Card
                    title={
                        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <span
                                    ref={dragRef}
                                    style={{
                                        cursor: 'move',
                                        marginRight: 8,
                                        color: '#888'
                                    }}
                                >
                                    <MenuOutlined />
                                </span>
                                <span>{column.name}</span>
                            </div>
                            <div>
                                {!column.isDefault && (
                                    <>
                                        <Tooltip title="Edit column">
                                            <Button
                                                type="text"
                                                size="small"
                                                icon={<EditOutlined />}
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    onColumnEdit(column);
                                                }}
                                                style={{ marginRight: 4 }}
                                            />
                                        </Tooltip>
                                        <Tooltip title="Delete column">
                                            <Popconfirm
                                                title="Delete Column"
                                                description="Are you sure you want to delete this column?"
                                                onConfirm={(e) => {
                                                    e.stopPropagation();
                                                    onColumnDelete(column.id);
                                                }}
                                                okText="Delete"
                                                cancelText="Cancel"
                                                okButtonProps={{ danger: true }}
                                            >
                                                <Button
                                                    danger
                                                    type="text"
                                                    size="small"
                                                    icon={<DeleteOutlined />}
                                                    onClick={(e) => e.stopPropagation()}
                                                />
                                            </Popconfirm>
                                        </Tooltip>
                                    </>
                                )}
                            </div>
                        </div>
                    }
                    headStyle={{ backgroundColor: column.color, color: getContrastColor(column.color) }}
                    bordered
                >
                    {jobs.length === 0 ? (
                        <Text type="secondary">No jobs</Text>
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

const ColumnMoveModal = ({ visible, onCancel, onMove, columns, columnToDelete }) => {
    const [targetColumnId, setTargetColumnId] = useState(null);

    const filteredColumns = columns.filter(column => column.id !== columnToDelete);

    useEffect(() => {
        if (filteredColumns.length > 0) {
            setTargetColumnId(filteredColumns[0].id);
        }
    }, [columnToDelete, visible, filteredColumns]);

    const handleMove = () => {
        if (targetColumnId) {
            onMove(columnToDelete, targetColumnId);
        }
    };

    return (
        <Modal
            title="Move Jobs to Another Column"
            open={visible}
            onCancel={onCancel}
            onOk={handleMove}
            okText="Move Jobs"
            cancelText="Cancel"
        >
            <p>This column contains jobs that need to be moved before deletion. Please select a target column:</p>
            <select
                style={{ width: '100%', padding: '8px', borderRadius: '4px', marginTop: '10px' }}
                value={targetColumnId || ''}
                onChange={(e) => setTargetColumnId(e.target.value)}
            >
                {filteredColumns.map(column => (
                    <option key={column.id} value={column.id}>
                        {column.name}
                    </option>
                ))}
            </select>
        </Modal>
    );
};

const BoardDetail = () => {
    const { boardId } = useParams();
    const [board, setBoard] = useState(null);
    const [jobs, setJobs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [teamMembers, setTeamMembers] = useState([]);
    const [selectedJob, setSelectedJob] = useState(null); // Fixed: changed from selectedJobDetails to selectedJob
    const [columns, setColumns] = useState([]);
    const [originalColumns, setOriginalColumns] = useState([]);
    const [selectedColumn, setSelectedColumn] = useState(null);
    const [isDeleteModalVisible, setIsDeleteModalVisible] = useState(false);
    const [columnToDelete, setColumnToDelete] = useState(null);

    useEffect(() => {
        const loadBoardData = async () => {
            setLoading(true);
            try {
                const boardResponse = await api.get(`board/${boardId}`);
                setBoard(boardResponse.data);
                setJobs(boardResponse.data.jobs);

                const columnsResponse = await api.get(`board-column/board/${boardId}`);
                const sortedColumns = columnsResponse.data.sort((a, b) => a.order - b.order);
                setColumns(sortedColumns);
                setOriginalColumns([...sortedColumns]);

                if (selectedJob) {
                    const newSelectedJob = boardResponse.data.jobs.find(job => job.id === selectedJob.id);
                    setSelectedJob(newSelectedJob || null);
                }

                if (boardResponse.data?.teamId) {
                    await fetchTeamMembers(boardResponse.data.teamId);
                }
            } catch (error) {
                console.error('Failed to load board data:', error);
            } finally {
                setLoading(false);
            }
        };

        loadBoardData();
    }, [boardId]);

    const fetchTeamMembers = async (teamId) => {
        if (!teamId) return;
        try {
            const response = await api.get(`team/${teamId}/members`);
            const members = [
                { value: '00000000-0000-0000-0000-000000000000', label: 'Unassigned' },
                ...response.data.map((m) => ({ value: m.id, label: m.userName }))
            ];
            setTeamMembers(members);
        } catch (error) {
            console.error('Failed to fetch team members:', error);
        }
    };

    const fetchBoardColumns = async () => {
        try {
            const columnsResponse = await api.get(`board-column/board/${boardId}`);
            const sortedColumns = columnsResponse.data.sort((a, b) => a.order - b.order);
            setColumns(sortedColumns);
            setOriginalColumns([...sortedColumns]);
            return sortedColumns;
        } catch (error) {
            console.error('Failed to fetch board columns:', error);
            setColumns([]);
            setOriginalColumns([]);
            return [];
        }
    };

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

    const getJobsForColumn = (column) => {
        return jobs.filter(job => job.columnId === column.id);
    };

    const updateJobStatus = async (jobId, deleteJob = false, newColumnId = null) => {
        try {
            if (deleteJob) {
                await api.delete(`job/${jobId}`);
                fetchBoardData();
                return;
            }

            const job = jobs.find(j => j.id === jobId);
            if (!job || !newColumnId) return;

            await api.post(`job/${jobId}/move-to-column/${newColumnId}`);
            fetchBoardData();

        } catch (err) {
            console.error('Failed to update job:', err);
            message.error('Failed to update job');
        }
    };

    const updateJob = (updatedJob) => {
        setJobs(prev =>
            prev.map(job => (job.id === updatedJob.id ? updatedJob : job))
        );
        setSelectedJob(updatedJob);
    };

    const moveColumnVisually = (fromIndex, toIndex) => {
        const updatedColumns = [...columns];
        const [movedItem] = updatedColumns.splice(fromIndex, 1);
        updatedColumns.splice(toIndex, 0, movedItem);
        setColumns(updatedColumns);
    };

    const handleColumnDragEnd = async () => {
        try {
            const columnIds = columns.map(column => column.id);
            await api.post('board-column/reorder', {
                boardId: boardId,
                columnIds: columnIds
            });
            setOriginalColumns([...columns]);
        } catch (error) {
            console.error('Failed to reorder columns:', error);
            message.error('Failed to update column order');
            setColumns([...originalColumns]);
        }
    };

    const handleColumnCreated = () => {
        fetchBoardColumns();
        fetchBoardData();
    };

    const handleColumnEdit = (column) => {
        if (column.isDefault) {
            toast.warning("Cannot edit default columns");
            return;
        }
        setSelectedColumn(column);
    };

    const handleColumnUpdated = () => {
        setSelectedColumn(null);
        fetchBoardColumns();
        fetchBoardData();
    };

    const handleColumnDelete = (columnId) => {
        if (columns.find(c => c.id === columnId)?.isDefault) {
            toast.warning("Cannot delete default columns");
            return;
        }
        const columnJobs = jobs.filter(job => job.columnId === columnId);
        if (columnJobs.length > 0) {
            setColumnToDelete(columnId);
            setIsDeleteModalVisible(true);
        } else {
            deleteColumn(columnId);
        }
    };

    const deleteColumn = async (columnId, targetColumnId = null) => {
        try {
            if (columns.find(c => c.id === columnId)?.isDefault) {
                toast.error("Cannot delete default column.");
                return;
            }
            let endpoint = `board-column/column/${columnId}`;
            if (targetColumnId) {
                endpoint += `?targetColumnId=${targetColumnId}`;
            }
            await api.delete(endpoint);
            fetchBoardColumns();
            fetchBoardData();
        } catch (error) {
            const message = error?.response?.data?.message || 'Failed to delete column';
            toast.error(message);
        }
    };

    const handleMoveJobsAndDelete = (columnId, targetColumnId) => {
        deleteColumn(columnId, targetColumnId);
        setIsDeleteModalVisible(false);
        setColumnToDelete(null);
    };

    const handleJobCreated = (newJob) => {
        if (!newJob) return;
        setJobs(currentJobs => {
            // Check if job already exists
            if (currentJobs.some(job => job.id === newJob.id)) {
                return currentJobs.map(job =>
                    job.id === newJob.id ? newJob : job
                );
            } else {
                return [...currentJobs, newJob];
            }
        });
        // Update board data if necessary
        if (board) {
            setBoard({
                ...board,
                jobs: [...board.jobs, newJob]
            });
        }
    };

    const handleJobUpdated = (updatedJob) => {
        if (!updatedJob) return;
        setJobs(currentJobs =>
            currentJobs.map(job => job.id === updatedJob.id ? updatedJob : job)
        );
        // If this job is selected, update it there too
        if (selectedJob && selectedJob.id === updatedJob.id) {
            setSelectedJob(updatedJob);
        }
    };

    useEffect(() => { fetchBoardData(); }, [boardId]);
    useEffect(() => { if (board?.teamId) fetchTeamMembers(); }, [board?.teamId]);

    if (loading) return <Spin size="large" />;

    const columnOptions = columns.map(column => ({
        value: column.id,
        label: column.name
    })) || [];

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
                    <div>
                        <DynamicForm
                            formTitle="Create Column"
                            schemaName="BoardColumnCreate"
                            apiUrl="board-column"
                            type="post"
                            onSuccess={handleColumnCreated}
                            trigger={<Button icon={<PlusOutlined />} style={{ marginRight: 8 }}>Add Column</Button>}
                            neededData={{ boardId }}
                        />
                        <DynamicForm
                            formTitle="Create Job"
                            schemaName="JobCreate"
                            apiUrl="job"
                            type="post"
                            onSuccess={handleJobCreated}
                            trigger={<Button type="primary" icon={<PlusOutlined />}>Create Job</Button>}
                            neededData={{ boardId }}
                            dropdownOptions={{
                                "Column": columnOptions,
                                "Assigned Member": teamMembers
                            }}
                        />
                    </div>
                </div>

                <Row gutter={16}>
                    {columns.map((column, index) => (
                        <DraggableColumn
                            key={column.id}
                            column={column}
                            jobs={getJobsForColumn(column)}
                            onDrop={updateJobStatus}
                            teamMembers={teamMembers}
                            boardId={boardId}
                            setSelectedJob={setSelectedJob}
                            index={index}
                            moveColumnVisually={moveColumnVisually}
                            onColumnDragEnd={handleColumnDragEnd}
                            onColumnEdit={handleColumnEdit}
                            onColumnDelete={handleColumnDelete}
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
                        updateJob={updateJob}
                    />
                )}

                <Modal
                    title={selectedColumn ? `Edit Column: ${selectedColumn.name}` : ''}
                    open={!!selectedColumn}
                    onCancel={() => setSelectedColumn(null)}
                    footer={null}
                    width={600}
                    destroyOnClose
                >
                    {selectedColumn && (
                        <DynamicForm
                            formTitle={`Edit Column: ${selectedColumn.name}`}
                            schemaName="BoardColumnUpdate"
                            apiUrl={`board-column/${selectedColumn.id}`}
                            type="patch"
                            onSuccess={handleColumnUpdated}
                            currentData={selectedColumn}
                            noModal={true}
                            onCancelConflict={() => setSelectedColumn(null)}
                            fetchCurrentData={() => api.get(`board-column/${selectedColumn.id}`).then(res => res.data)}
                        />
                    )}
                </Modal>

                <ColumnMoveModal
                    visible={isDeleteModalVisible}
                    onCancel={() => {
                        setIsDeleteModalVisible(false);
                        setColumnToDelete(null);
                    }}
                    onMove={handleMoveJobsAndDelete}
                    columns={columns}
                    columnToDelete={columnToDelete}
                />
            </DndProvider>
        </AppLayout>
    );
};

export default BoardDetail;
