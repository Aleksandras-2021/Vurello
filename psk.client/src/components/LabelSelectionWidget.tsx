import React, { useState } from 'react';
import { Input } from 'antd';

const LabelSelectionWidget = (props) => {
    const { value = [], onChange, uiSchema, disabled } = props;
    const safeValue = Array.isArray(value) ? value.filter(id => typeof id === 'string') : [];
    const allLabels = uiSchema['ui:options']?.labels || [];
    const [searchTerm, setSearchTerm] = useState('');

    const selectedLabels = allLabels.filter(label => safeValue.includes(label.id));
    const unselectedLabels = allLabels
        .filter(label => !safeValue.includes(label.id))
        .filter(label => label.text.toLowerCase().includes(searchTerm.toLowerCase()));

    const handleChange = (newValue: string[]) => {
        onChange(newValue.filter(id => typeof id === 'string'));
    };

    if (disabled) {
        return (
            <div>
                <h3>Current Labels</h3>
                <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                    {selectedLabels.map(label => (
                        <div
                            key={label.id}
                            style={{
                                backgroundColor: label.backgroundColor,
                                color: label.textColor,
                                padding: '6px 12px',
                                borderRadius: 8,
                            }}
                        >
                            {label.text}
                        </div>
                    ))}
                    {selectedLabels.length === 0 && (
                        <div style={{ color: '#888' }}>No selected labels</div>
                    )}
                </div>
            </div>
        );
    }

    return (
        <div>
            <h4>Selected Labels</h4>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginBottom: 16 }}>
                {selectedLabels.map(label => (
                    <div
                        key={label.id}
                        onClick={() => {
                            handleChange(safeValue.filter(id => id !== label.id));
                        }}
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
                {selectedLabels.length === 0 && (
                    <div style={{ color: '#888' }}>No selected labels</div>
                )}
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
                <h4 style={{ margin: 0 }}>Unselected Labels</h4>
                <Input.Search
                    placeholder="Search unselected labels"
                    value={searchTerm}
                    onChange={e => setSearchTerm(e.target.value)}
                    style={{ maxWidth: 300 }}
                />
            </div>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '8px' }}>
                {unselectedLabels.map(label => (
                    <div
                        key={label.id}
                        onClick={() => {
                            handleChange([...safeValue, label.id]);
                        }}
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
                {unselectedLabels.length === 0 && (
                    <div style={{ color: '#888' }}>No matching labels</div>
                )}
            </div>
        </div>
    );
};

export default LabelSelectionWidget;