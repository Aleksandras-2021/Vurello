import React, { useEffect, useState } from 'react';
import { List, Card, Typography, Spin } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { getTeamMembers } from './API';

const { Title } = Typography;

interface TeamMembersProps {
    teamId: string;
    refreshTrigger?: number;
}

const TeamMembers: React.FC<TeamMembersProps> = ({ teamId, refreshTrigger = 0 }) => {
    const [members, setMembers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchMembers = async () => {
            if (!teamId) return;

            try {
                setLoading(true);
                const data = await getTeamMembers(teamId);
                setMembers(Array.isArray(data) ? data : []);
            } catch (error) {
                console.error('Failed to fetch team members:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchMembers();
    }, [teamId, refreshTrigger]);

    if (loading) {
        return <Spin size="small" />;
    }

    return (
        <Card
            title={<Title level={5}>Team Members</Title>}
            size="small"
            style={{
                position: 'fixed',
                bottom: '20px',
                right: '20px',
                width: '250px',
                maxHeight: '400px',
                overflow: 'auto'
            }}
        >
            <List
                dataSource={members}
                renderItem={(member) => (
                    <List.Item>
                        <List.Item.Meta
                            avatar={<UserOutlined />}
                            title={member.userName}
                        />
                    </List.Item>
                )}
                locale={{ emptyText: 'No members found' }}
            />
        </Card>
    );
};

export default TeamMembers;