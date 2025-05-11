import React from 'react';
import { ColorPicker } from 'antd';
import { WidgetProps } from '@rjsf/utils';

const ColorPickWidget = (props: WidgetProps) => {
    const {value, onChange } = props;

    return (
        <ColorPicker
            value={value}
            onChange={(color) => onChange(color.toHexString())}
            showText
        />
    );
};

export default ColorPickWidget;
