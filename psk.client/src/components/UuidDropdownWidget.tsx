import React, { useEffect } from 'react';
import { Select } from 'antd';
import { WidgetProps } from '@rjsf/utils';

const UuidDropdownWidget = (props: WidgetProps) => {
    const { options, value, onChange, id, disabled } = props;

    const selectOptions = options.enumOptions?.map((option: any) => ({
        value: option.value,
        label: option.label || option.value
    })) || [];

    useEffect(() => {
        if ((value === null || value === undefined) && selectOptions.length > 0) {
            onChange(selectOptions[0].value);
        }
    }, [value, selectOptions, onChange]);

    return (
        <Select
            id={id}
            value={value}
            onChange={(newValue) => {
                console.log("Selected value:", newValue);
                onChange(newValue);
            }}
            style={{ width: '100%' }}
            options={selectOptions}
            showSearch
            filterOption={(input, option) =>
                (option?.label?.toString().toLowerCase() || '').includes(input.toLowerCase())
            }
            placeholder="Search and select..."
            disabled={disabled}
        />
    );
};

export default UuidDropdownWidget;