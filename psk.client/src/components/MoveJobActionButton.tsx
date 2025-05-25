import React, { useState } from 'react';
import { Button, Tooltip } from 'antd';
import { SwapOutlined } from '@ant-design/icons';
import MoveJobBoardModal from './MoveJobBoardModal';

interface MoveJobActionButtonProps {
    job: any;
    teamId: string;
    onSuccess: () => void;
    buttonType?: 'text' | 'primary' | 'default';
    size?: 'small' | 'middle' | 'large';
    iconOnly?: boolean;
}

const MoveJobActionButton: React.FC<MoveJobActionButtonProps> = ({
    job,
    teamId,
    onSuccess,
    buttonType = 'text',
    size = 'small',
    iconOnly = false
}) => {
    const [modalVisible, setModalVisible] = useState(false);

    const handleSuccess = () => {
        setModalVisible(false);
        onSuccess();
    };

    return (
        <>
            <Tooltip title="Move to different board">
                <Button
                    type={buttonType}
                    size={size}
                    icon={<SwapOutlined />}
                    onClick={(e) => {
                        e.stopPropagation();
                        setModalVisible(true);
                    }}
                >
                    {!iconOnly && 'Move'}
                </Button>
            </Tooltip>

            <MoveJobBoardModal
                open={modalVisible}
                onCancel={() => setModalVisible(false)}
                job={job}
                currentTeamId={teamId}
                onSuccess={handleSuccess}
            />
        </>
    );
};

export default MoveJobActionButton;