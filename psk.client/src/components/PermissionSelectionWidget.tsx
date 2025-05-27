import React, { useState } from 'react';
import { Input, List, Checkbox } from 'antd';


const PermissionSelectionWidget = (props) => {
    const { value = [], onChange, uiSchema, disabled } = props;
    const safeValue: string[] = Array.isArray(value)
    ? value.filter(id => typeof id === 'string')
    : [];
    const allPermissions: any[] = uiSchema['ui:options']?.permissions || [];
    const [searchTerm, setSearchTerm] = useState('');

    const filteredPermissions = allPermissions.filter(p =>
        p.description.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleToggle = (id: string, checked: boolean) => {
    const next = checked
        ? [...safeValue, id]
        : safeValue.filter(currentId => currentId !== id);
    onChange(next);
    };

    return (
    <div>
        <h4>Permissions</h4>
        <Input.Search
        placeholder="Search permissions"
        value={searchTerm}
        onChange={e => setSearchTerm(e.target.value)}
        style={{ marginBottom: 16, maxWidth: 300 }}
        disabled={disabled}
        />
        <List
        bordered
        dataSource={filteredPermissions}
        style={{ maxHeight: '300px', overflowY: 'auto' }}
        renderItem={item => (
            <List.Item>
            <Checkbox
                checked={safeValue.includes(item.id)}
                onChange={e => handleToggle(item.id, e.target.checked)}
                disabled={disabled}
            >
                    <span style={{ fontWeight: 500 }}>{item.description}</span>
            </Checkbox>
            </List.Item>
        )}
        />
        {filteredPermissions.length === 0 && (
        <div style={{ color: '#888', marginTop: 8 }}>No matching permissions</div>
        )}
    </div>
    );
};

export default PermissionSelectionWidget;
