import React, { useEffect, useState } from 'react';
import {
    Button,
    Card,
    Table,
    Space,
    Typography,
    Input,
    Popconfirm,
    Spin
} from 'antd';
import {
    DeleteOutlined,
    PlusOutlined,
    EditOutlined,
    SearchOutlined,
    ReloadOutlined
} from '@ant-design/icons';
import { toast } from 'react-toastify';
import { api } from '../components/API';
import { useAppContext } from '../components/AppContext';
import AppLayout from '../components/AppLayout';
import DynamicForm from '../components/DynamicForm';
import { RJSFSchema, UiSchema } from '@rjsf/utils';


const { Title } = Typography;

const Roles: React.FC = () => {
    const [roles, setRoles] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchText, setSearchText] = useState('');
    const [allPermissions, setAllPermissions] = useState<any[]>([]);
    const { lastTeamId } = useAppContext();
    const [refreshLoading, setRefreshLoading] = useState(false);

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

    const fetchPermissions = async () => {
         try {
             const resp = await api.get(`/permission`);
             setAllPermissions(resp.data || []);
         } catch (err) {
             console.error('Failed to fetch permissions:', err);
         }
    };

    const handleRoleCreated = (newRole) => {
        if (!newRole) return;
        setRoles(prev => {
            if (prev.some(role => role.id === newRole.id)) {
                return prev.map(role => role.id === newRole.id ? newRole : role);
            }
            return [...prev, newRole];
        });
    };

    const handleRoleUpdated = (updatedRole) => {
        setRoles(prev =>
            prev.map(role => (role.id === updatedRole.id ? { ...role, ...updatedRole } : role))
        );
    };

    const handleRefresh = async () => {
        setRefreshLoading(true);
        try {
            await Promise.all([fetchRoles(), fetchPermissions()]);
            toast.success('Data refreshed successfully');
        } catch (error) {
            console.error('Failed to refresh data:', error);
            toast.error('Failed to refresh data');
        } finally {
            setRefreshLoading(false);
        }
    };

    useEffect(() => {
        if (lastTeamId) {
            fetchRoles();
            fetchPermissions();
        }
    }, [lastTeamId]);

    const handleDeleteRole = async (roleId: string) => {
        try {
            await api.delete(`role/${roleId}`);
            toast.success('Role deleted successfully');
            fetchRoles();
        } catch (error) {
            console.error('Failed to delete role:', error);
            toast.error('Failed to delete role');
        }
    };

    const filteredRoles = roles.filter(role =>
        role.name.toLowerCase().includes(searchText.toLowerCase())
    );

    const roleSchema: RJSFSchema = {
        type: 'object',
        required: ['permissions'],
        properties: {
            permissions: {
                type: 'array',
                items: {
                    type: 'string',
                },
                uniqueItems: true,
                default: [],
            },
        },
    };

    const roleUiSchema: UiSchema = {
        permissions: {
            'ui:widget': 'permissionSelection',
            'ui:options': { permissions: allPermissions },
            'ui:classNames': 'hide-label',

        }
    };


    const columns = [
        { title: 'Role Name', dataIndex: 'name', key: 'name' },
        {
            title: 'Actions',
            key: 'actions',
            render: (_: any, record: any) => (
                <Space>
                    <DynamicForm
                        formTitle="Assign permission"
                        schema={roleSchema}
                        uiSchema={roleUiSchema}
                        apiUrl={`role/${record.id}/permissions`}
                        type="put"
                        currentData={{
                            permissions: record.permissions?.map((p: any) => p.id) || [],
                            version: record.version
                        }}
                        onSuccess={handleRoleUpdated}
                        trigger={
                            <Button type="text" >
                                Assign permissions
                            </Button>
                        }
                    />
                    <DynamicForm
                        formTitle="Edit Role"
                        schemaName="RoleUpdate"
                        apiUrl={`role/${record.id}`}
                        type="patch"
                        currentData={record}
                        onSuccess={handleRoleUpdated}
                        trigger={<Button type="text" icon={<EditOutlined />} />}
                    />
                    <Popconfirm
                        title="Delete Role"
                        description="Are you sure you want to delete this role?"
                        onConfirm={() => handleDeleteRole(record.id)}
                        okText="Yes"
                        cancelText="No"
                        okButtonProps={{ danger: true }}
                    >
                        <Button type="text" danger icon={<DeleteOutlined />} />
                    </Popconfirm>
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
                    <Title level={2}>Role Management</Title>
                    <div style={{ display: 'flex', gap: 16 }}>
                        <Button
                            icon={<ReloadOutlined />}
                            onClick={handleRefresh}
                            loading={refreshLoading}
                            disabled={loading || refreshLoading}
                        >
                            Refresh
                        </Button>
                        <Input
                            placeholder="Search roles"
                            prefix={<SearchOutlined />}
                            value={searchText}
                            onChange={e => setSearchText(e.target.value)}
                            style={{ width: 250 }}
                            allowClear
                        />
                        <DynamicForm
                            formTitle="Create Role"
                            schemaName="RoleCreate"
                            apiUrl={`role/${lastTeamId}`}
                            type="post"
                            neededData={{ teamId: lastTeamId }}
                            onSuccess={handleRoleCreated}
                            trigger={
                                <Button type="primary" icon={<PlusOutlined />}>
                                    New Role
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
                        dataSource={filteredRoles}
                        columns={columns}
                        rowKey="id"
                        pagination={{ pageSize: 10 }}
                        locale={{ emptyText: "No roles found. Create a new role to get started." }}
                    />
                )}
            </div>
        </AppLayout>
    );
};

export default Roles;
