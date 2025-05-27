import React, { useEffect, useState } from 'react';
import { List, Typography, Button, Modal, message, Badge, Dropdown } from 'antd';
import { MailOutlined, BellOutlined, CheckOutlined, CloseOutlined } from '@ant-design/icons';
import { api } from './API';
import { toast } from 'react-toastify';

const { Title } = Typography;

const UserInbox: React.FC = () => {
    const [invitations, setInvitations] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalVisible, setIsModalVisible] = useState(false);

    const fetchInvitations = async () => {
        try {
            setLoading(true);
            const response = await api.get('invitation/inbox');
            setInvitations(Array.isArray(response.data) ? response.data : []);
        } catch (error) {
            console.error('Failed to fetch invitations:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchInvitations();
        const interval = setInterval(fetchInvitations, 30000);
        return () => clearInterval(interval);
    }, []);

    const handleRespondToInvitation = async (invitationId: string, accept: boolean) => {
        try {
            const responseBody = accept
                ? { isAccepted: true }
                : { isRejected: true };

            await api.patch(`/invitation/${invitationId}`, responseBody, {
                headers: { 'Content-Type': 'application/json' }
            });

            fetchInvitations();

            if (accept) {
                window.location.href = '/teams';
            }
        } catch (error) {
            console.error('Failed to respond to invitation:', error);
            message.error('Failed to respond to invitation');
        }
    };

    const showModal = () => {
        setIsModalVisible(true);
        fetchInvitations();
    };

    const items = [
        {
            key: '1',
            label: (
                <Button type="text" onClick={showModal} icon={<MailOutlined />}>
                    View Invitations
                </Button>
            ),
        },
    ];

    return (
        <>
            <Dropdown menu={{ items }} placement="bottomRight">
                <Badge count={invitations.length} size="small" style={{ display: invitations.length > 0 ? 'block' : 'none' }}>
                    <Button
                        type="text"
                        icon={<BellOutlined />}
                        style={{ marginRight: 16 }}
                    />
                </Badge>
            </Dropdown>

            <Modal
                title="Team Invitations"
                open={isModalVisible}
                onCancel={() => setIsModalVisible(false)}
                footer={null}
                width={600}
            >
                <List
                    loading={loading}
                    dataSource={invitations}
                    renderItem={(invitation) => (
                        <List.Item
                            actions={[
                                <Button
                                    type="primary"
                                    icon={<CheckOutlined />}
                                    onClick={() => handleRespondToInvitation(invitation.id, true)}
                                    ghost
                                >
                                    Accept
                                </Button>,
                                <Button
                                    danger
                                    icon={<CloseOutlined />}
                                    onClick={() => handleRespondToInvitation(invitation.id, false)}
                                >
                                    Decline
                                </Button>
                            ]}
                        >
                            <List.Item.Meta
                                title={`Invitation to join ${invitation.team.name}`}
                                description={`Invited by ${invitation.sender.userName} on ${new Date(invitation.createdAt).toLocaleString()}`}
                            />
                        </List.Item>
                    )}
                    locale={{ emptyText: 'No invitations found' }}
                />
            </Modal>
        </>
    );
};

export default UserInbox;
