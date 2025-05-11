import React, { useEffect, useState, ReactElement, cloneElement } from 'react';
import Form from '@rjsf/antd';
import { RJSFSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import { Modal, Spin } from 'antd';
import { ButtonProps } from 'antd';
import { api } from './API';
import UuidDropdownWidget from './UuidDropdownWidget';
import RichTextWidget from './RichTextWidget';
import ColorPickWidget from './ColorPickWidget';
import { getSchema, getUiSchema, loadAllSchemas, isSchemaCacheLoaded } from './SchemaCache';

interface DynamicFormProps {
    formTitle: string;
    schemaName: string;
    apiUrl: string;
    type: 'post' | 'patch';
    neededData?: Record<string, any>;
    onSuccess?: (responseData?: any) => void;
    trigger?: ReactElement<Partial<ButtonProps> & { onClick?: () => void }>;
    noModal?: boolean;
    currentData?: Record<string, any>;
    dropdownOptions?: { value: string; label: string }[];
}

const DynamicForm: React.FC<DynamicFormProps> = ({
                                                     formTitle,
                                                     schemaName,
                                                     apiUrl,
                                                     neededData,
                                                     onSuccess,
                                                     trigger,
                                                     noModal,
                                                     type,
                                                     currentData = {},
                                                     dropdownOptions,
                                                 }) => {
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [formData, setFormData] = useState<Record<string, any>>({});
    const [loading, setLoading] = useState(false);
    const [schema, setSchema] = useState<RJSFSchema | null>(null);
    const [uiSchema, setUiSchema] = useState<any>({});

    const widgets = {
        uuidDropdown: UuidDropdownWidget,
        richText: RichTextWidget,
        colorPick: ColorPickWidget,
    };

    // Load schema from cache or fetch it
    useEffect(() => {
        const loadSchema = async () => {
            setLoading(true);

            // If schemas haven't been loaded yet, load them
            if (!isSchemaCacheLoaded()) {
                await loadAllSchemas();
            }

            // Get schema from cache
            const cachedSchema = getSchema(schemaName);
            const cachedUiSchema = getUiSchema(schemaName);

            if (cachedSchema) {
                // Clone the schema to avoid modifying the cached version
                const clonedSchema = JSON.parse(JSON.stringify(cachedSchema));
                const clonedUiSchema = JSON.parse(JSON.stringify(cachedUiSchema));

                // Handle dropdown options if provided
                if (dropdownOptions && clonedSchema.properties) {
                    Object.entries(clonedSchema.properties).forEach(([key, value]: [string, any]) => {
                        if (value["x-dropdown"]) {
                            clonedSchema.properties[key].enum = dropdownOptions.map(option => option.value);
                            clonedSchema.properties[key].enumNames = dropdownOptions.map(option => option.label);
                        }
                    });
                }

                setSchema(clonedSchema);
                setUiSchema(clonedUiSchema);
            } else {
                console.error(`Schema "${schemaName}" not found in cache`);
            }

            setLoading(false);
        };

        loadSchema();
    }, [schemaName, dropdownOptions]);

    // Set form data from currentData when type is patch
    useEffect(() => {
        if (type === 'patch' && currentData && schema) {
            const validKeys = Object.keys(schema.properties || {});
            const filteredData = Object.fromEntries(
                Object.entries(currentData).filter(([key]) => validKeys.includes(key))
            );
            setFormData(filteredData);
        }
    }, [currentData, schema, type]);

    const handleSubmit = async ({ formData: submittedData }: any) => {
        try {
            let payload;

            if (type === 'patch' && currentData) {
                payload = Object.entries(submittedData).reduce((acc, [key, value]) => {
                    if (currentData[key] !== value) {
                        acc[key] = value;
                    }
                    return acc;
                }, {} as Record<string, any>);
            } else {
                payload = submittedData;
            }

            const fullData = {
                ...payload,
                ...neededData,
            };

            const response = await api[type === 'patch' ? 'patch' : 'post'](apiUrl, fullData);
            onSuccess?.(response.data);
            handleCloseModal();
            console.log('Form submitted successfully:', response.data);
        } catch (error) {
            console.error('Form submission error:', error);
        }
    };

    const handleOpenModal = () => {
        setIsModalVisible(true);
    };

    const handleCloseModal = () => {
        setIsModalVisible(false);
    };

    if (loading) {
        return trigger
            ? cloneElement(trigger, { disabled: true })
            : <Spin size="small" />;
    }

    if (!schema) {
        return trigger
            ? cloneElement(trigger, { disabled: true })
            : <div>Schema not found</div>;
    }

    const formComponent = (
        <Form
            schema={schema}
            uiSchema={uiSchema}
            validator={validator}
            formData={formData}
            widgets={widgets}
            onSubmit={handleSubmit}
        />
    );

    return (
        <>
            {trigger && React.isValidElement(trigger)
                ? cloneElement(trigger, { onClick: handleOpenModal })
                : null}

            {noModal ? (
                formComponent
            ) : (
                <Modal
                    title={formTitle}
                    open={isModalVisible}
                    onCancel={handleCloseModal}
                    footer={null}
                    width={600}
                    destroyOnClose
                >
                    {formComponent}
                </Modal>
            )}
        </>
    );
};

export default DynamicForm;