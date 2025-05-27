import React, { useState, useEffect } from 'react';
import { Modal, Select, Button, Typography, message, Spin } from 'antd';
import { SwapOutlined } from '@ant-design/icons';
import { api } from './API';
import { toast } from 'react-toastify';
import { useTheme } from './ThemeContext';

const { Text } = Typography;

interface MoveJobBoardModalProps {
    open: boolean;
    onCancel: () => void;
    job: any;
    currentTeamId: string;
    onSuccess: () => void;
}

const MoveJobBoardModal: React.FC<MoveJobBoardModalProps> = ({
    open,
    onCancel,
    job,
    currentTeamId,
    onSuccess
}) => {
    const { isDarkMode } = useTheme();
    const [loading, setLoading] = useState(false);
    const [targetBoardId, setTargetBoardId] = useState<string>('');
    const [availableBoards, setAvailableBoards] = useState<any[]>([]);
    const [fetchingBoards, setFetchingBoards] = useState(false);
    const [currentBoardName, setCurrentBoardName] = useState<string>('Unknown');

    useEffect(() => {
        if (open && currentTeamId) {
            fetchTeamBoards();
            fetchCurrentBoard();
        }
    }, [open, currentTeamId, job?.boardId]);

    const fetchCurrentBoard = async () => {
        if (!job?.boardId) return;

        try {
            const response = await api.get(`board/${job.boardId}`);
            setCurrentBoardName(response.data.name);
        } catch (error) {
            console.error('Failed to fetch current board:', error);
            setCurrentBoardName('Unknown');
        }
    };

    const fetchTeamBoards = async () => {
        setFetchingBoards(true);
        try {
            const response = await api.get(`team/${currentTeamId}`);
            const boards = response.data.boards || [];

            // Filter out the current board
            const otherBoards = boards.filter((board: any) => board.id !== job.boardId);
            setAvailableBoards(otherBoards);

            // Reset selection
            setTargetBoardId('');
        } catch (error) {
            console.error('Failed to fetch team boards:', error);
            toast.error('Failed to load available boards');
        } finally {
            setFetchingBoards(false);
        }
    };

    const handleMoveJob = async () => {
        if (!targetBoardId) {
            message.warning('Please select a target board');
            return;
        }

        setLoading(true);
        try {
            await api.post(`job/${job.id}/move-to-board/${targetBoardId}`);
            onSuccess();
            onCancel();
        } catch (error: any) {
            console.error('Failed to move job:', error);
            const errorMessage = error?.response?.data?.message || 'Failed to move job';
            toast.error(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    const selectedBoard = availableBoards.find(board => board.id === targetBoardId);

    return (
        <Modal
            title={
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <SwapOutlined style={{ marginRight: 8 }} />
                    Move Job to Different Board
                </div>
            }
            open={open}
            onCancel={onCancel}
            footer={[
                <Button key="cancel" onClick={onCancel}>
                    Cancel
                </Button>,
                <Button
                    key="move"
                    type="primary"
                    onClick={handleMoveJob}
                    loading={loading}
                    disabled={!targetBoardId}
                >
                    Move Job
                </Button>
            ]}
            width={500}
        >
            <div style={{ marginBottom: 16 }}>
                <Text strong>Job: </Text>
                <Text>{job?.name}</Text>
            </div>

            <div style={{ marginBottom: 16 }}>
                <Text strong>Current Board: </Text>
                <Text>{currentBoardName}</Text>
            </div>

            <div style={{ marginBottom: 20 }}>
                <Text strong>Select Target Board:</Text>
                <Select
                    style={{ width: '100%', marginTop: 8 }}
                    placeholder="Choose a board to move this job to"
                    value={targetBoardId}
                    onChange={setTargetBoardId}
                    loading={fetchingBoards}
                    disabled={fetchingBoards || availableBoards.length === 0}
                >
                    {availableBoards.map(board => (
                        <Select.Option key={board.id} value={board.id}>
                            {board.name}
                        </Select.Option>
                    ))}
                </Select>

                {!fetchingBoards && availableBoards.length === 0 && (
                    <Text type="secondary" style={{ fontSize: '12px', marginTop: 4, display: 'block' }}>
                        No other boards available in this team
                    </Text>
                )}
            </div>

            {selectedBoard && (
                <div style={{
                    padding: 12,
                    backgroundColor: isDarkMode ? '#2a2a2a' : '#f6f6f6',
                    borderRadius: 6,
                    marginTop: 12,
                    border: isDarkMode ? '1px solid #434343' : 'none'
                }}>
                    <Text
                        type="secondary"
                        style={{
                            fontSize: '12px',
                            color: isDarkMode ? '#ffffff' : undefined
                        }}
                    >
                        The job will be moved to "{selectedBoard.name}" and placed in the default "To Do" column.
                    </Text>
                </div>
            )}
        </Modal>
    );
};

export default MoveJobBoardModal;
