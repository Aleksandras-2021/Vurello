import React, { useEffect, useState, useMemo } from 'react';
import {
    Button,
    Table,
    Space,
    Typography,
    Input,
    Popconfirm,
    Spin
} from 'antd';
import {
    DeleteOutlined,
    SearchOutlined,
    ReloadOutlined
} from '@ant-design/icons';
import { toast } from 'react-toastify';
import { api } from '../components/API';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import DynamicForm from '../components/DynamicForm';

const { Title } = Typography;

const Users: React.FC = () => {
    const [users, setUsers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchText, setSearchText] = useState('');
    const [refreshLoading, setRefreshLoading] = useState(false);
    const { lastTeamId, creatorId } = useAppContext();
    const [roles, setRoles] = useState<any[]>([]);

    const fetchUsers = async () => {
        setLoading(true);
        try {
            const response = await api.get(`/team/${lastTeamId}/members`);
            setUsers(response.data || []);
            console.log(response.data);
        } catch (error) {
            console.error('Failed to fetch users:', error);
            toast.error('Failed to load users');
        } finally {
            setLoading(false);
        }
    };

    const fetchRoles = async () => {
        setLoading(true);
        try {
            const response = await api.get(`team/${lastTeamId}/roles`);
            setRoles(response.data[0].roles);
        } catch (error) {
            console.error('Failed to fetch roles:', error);
            toast.error('Failed to load roles');
        } finally {
            setLoading(false);
        }
    };

    const handleInviteUser = async () => {
        toast.success('Invitation sent');
    };

    const handleKickUser = async (userId: string) => {
        try {
            await api.delete(`/team/${lastTeamId}/members/${userId}`);
            toast.success('User removed from team');
            fetchUsers();
        } catch (error) {
            console.error('Failed to remove user:', error);
            toast.error('Failed to remove user');
        }
    };

    const handleRefresh = async () => {
        setRefreshLoading(true);
        try {
            await Promise.all([fetchUsers(), fetchRoles()]); 
            toast.success('Data refreshed successfully');
        } catch (error) {
            toast.error('Failed to refresh data:', error);
            toast.error('Failed to refresh data');
        } finally {
            setRefreshLoading(false);
        }
    };

    useEffect(() => {
        if (lastTeamId) {
            fetchUsers();
            fetchRoles();
        }
    }, [lastTeamId]);

    const filteredUsers = users.filter(user =>
        user.userName.toLowerCase().includes(searchText.toLowerCase())
    );

    const roleOptions = useMemo(() =>
        roles.map(role => ({
            value: role.id,
            label: role.name
        })),
        [roles]
    );

    const combinedDropdownOptions = useMemo(() => ({
        "Role": roleOptions,
    }), [roleOptions]);

    const columns = [
        { title: 'Username', dataIndex: 'userName', key: 'userName' },
        {
            title: 'Role',
            key: 'role',
            render: (_: any, record: any) => {
                if (record.id === creatorId) {
                    return 'Team Creator';
                }
                const roleId = record.userTeamRoles?.[0]?.roleId;
                const role = roles.find(r => r.id === roleId);
                return role ? role.name : 'No role assigned';
            }
        },
        {
            title: 'Actions',
            key: 'actions',
            render: (_: any, record: any) => (
                <Space>
                    {record.id !== creatorId && (
                        <>
                            <DynamicForm
                                formTitle="Assign Role"
                                schemaName="AssignRole"
                                apiUrl={`team/${lastTeamId}/assign`}
                                type="put"
                                neededData={{
                                    userId: record.id,
                                    teamId: lastTeamId
                                }}
                                onSuccess={fetchUsers}
                                dropdownOptions={combinedDropdownOptions}
                                currentData={record.userTeamRoles?.[0] || {}}
                                trigger={
                                    <Button type="text">
                                        Assign Role
                                    </Button>
                                }
                            />
                            <Popconfirm
                                title="Remove User"
                                description="Are you sure you want to remove this user from the team?"
                                onConfirm={() => handleKickUser(record.id)}
                                okText="Remove"
                                cancelText="Cancel"
                                okButtonProps={{ danger: true }}
                            >
                                <Button type="text" danger icon={<DeleteOutlined />} />
                            </Popconfirm>
                        </>
                    )}
                </Space>
            )
        }
    ];

    return (
        <AppLayout>
            <div style={{ marginBottom: 24 }}>
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    marginBottom: 16
                }}>
                    <Title level={2}>Team Members</Title>
                    <div style={{ display: 'flex', gap: 16 }}>
                        <Input
                            placeholder="Search users"
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
                            disabled={loading || refreshLoading}
                        >
                            Refresh
                        </Button>
                        <DynamicForm
                            formTitle="Invite user to team"
                            schemaName="InvitationCreate"
                            apiUrl={`invitation/${lastTeamId}`}
                            type='post'
                            neededData={{ teamId: lastTeamId }}
                            onSuccess={handleInviteUser}
                            trigger={
                                <Button style={{ marginRight: 10 }}>
                                    Invite Member
                                </Button>
                            }
                        />
                    </div>
                </div>

                {loading ? (
                    <div style={{ display: 'flex', justifyContent: 'center', padding: 100 }}>
                        <Spin size="large" />
                    </div>
                ) : (
                    <Table
                        dataSource={filteredUsers}
                        columns={columns}
                        rowKey="id"
                        pagination={{ pageSize: 10 }}
                        locale={{ emptyText: "No users in this team." }}
                    />
                )}
            </div>
        </AppLayout>
    );
};

export default Users;