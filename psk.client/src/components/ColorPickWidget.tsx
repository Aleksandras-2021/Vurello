import React from 'react';
import { ColorPicker } from 'antd';
import { WidgetProps } from '@rjsf/utils';

const ColorPickWidget = (props: WidgetProps) => {
    const { value, onChange, disabled } = props;

    return (
        <ColorPicker
            value={value}
            onChange={(color) => onChange(color.toHexString())}
            showText
            disabled={disabled}
        />
    );
};

export default ColorPickWidget;