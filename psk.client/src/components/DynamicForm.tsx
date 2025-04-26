import React, { useEffect, useState, ReactElement, cloneElement } from 'react';
import Form from '@rjsf/antd';
import { RJSFSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import axios from 'axios';
import { Modal } from 'antd';
import { ButtonProps } from 'antd';
import { api } from './API';
import UuidDropdownWidget from './UuidDropdownWidget';


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
    const [formSchema, setFormSchema] = useState<RJSFSchema | null>(null);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [uiSchema, setUiSchema] = useState<any>({});
    const [formData, setFormData] = useState<Record<string, any>>({});
    const widgets = {
        uuidDropdown: UuidDropdownWidget
    };

    useEffect(() => {
        const fetchSwaggerSchema = async () => {
            try {
                const response = await axios.get('https://localhost:7285/swagger/v1/swagger.json');
                const swaggerDoc = response.data;

                const schema = swaggerDoc.components?.schemas?.[schemaName];

                if (!schema) {
                    throw new Error(`Schema "${schemaName}" not found in Swagger`);
                }

                const newUiSchema: any = {
                    "ui:submitButtonOptions": {
                        norender: false,
                        submitText: "Submit",
                        props: {
                            style: {
                                backgroundColor: "blue",
                                color: "white",
                                borderRadius: "8px",
                                padding: "10px 20px",
                            },
                        },
                    },
                };

                const filteredSchema: any = {
                    ...schema,
                    properties: {},
                };

                if (schema.properties) {
                    Object.entries(schema.properties).forEach(([key, value]: [string, any]) => {
                        if (value["x-ignore"] === "true") return;

                        filteredSchema.properties[key] = value;

                        const uiField: any = {};

                        if (value["x-prompt"]) {
                            uiField["ui:title"] = value["x-prompt"];
                        }

                        if (value["x-hidden"] === "true") {
                            uiField["ui:widget"] = "password";
                        }
                        if (value["x-enum"]) {
                            const options = value["x-enum"].split(',');
                            filteredSchema.properties[key].enum = options;
                            uiField["ui:widget"] = "select";
                        }

                        if (value["x-dropdown"]) {
                            filteredSchema.properties[key].enum = dropdownOptions?.map(option => option.value) || [];
                            filteredSchema.properties[key].enumNames = dropdownOptions?.map(option => option.label) || [];
                            uiField["ui:widget"] = "uuidDropdown";
                        }

                        newUiSchema[key] = uiField;
                    });
                }

                setFormSchema(filteredSchema);
                setUiSchema(newUiSchema);

            } catch (error) {
                console.error('Error fetching Swagger schema:', error);
            }
        };

        handleCloseModal();
        fetchSwaggerSchema();
    }, [schemaName, dropdownOptions]);

    useEffect(() => {
        if (type === 'patch' && currentData && formSchema) {
            const validKeys = Object.keys(formSchema.properties || {});
            const filteredData = Object.fromEntries(
                Object.entries(currentData).filter(([key]) => validKeys.includes(key))
            );
            setFormData(filteredData);
        }
    }, [currentData, formSchema, type]);

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

    return (
        <>
            {trigger && React.isValidElement(trigger)
                ? cloneElement(trigger, { onClick: handleOpenModal })
                : null}

            {noModal ? (
                formSchema ? (
                    <Form
                        schema={formSchema}
                        validator={validator}
                        uiSchema={uiSchema}
                        formData={formData}
                        widgets={widgets}
                        onSubmit={handleSubmit}
                    />
                ) : (
                    <div>Loading...</div>
                )
            ) : (
                <Modal
                    title={formTitle}
                    open={isModalVisible}
                    onCancel={handleCloseModal}
                    footer={null}
                    width={600}
                    destroyOnClose
                >
                    {formSchema ? (
                        <Form
                            schema={formSchema}
                            validator={validator}
                            uiSchema={uiSchema}
                            formData={formData}
                            widgets={widgets}
                            onSubmit={handleSubmit}
                        />
                    ) : (
                        <div>Loading...</div>
                    )}
                </Modal>
            )}
        </>
    );
};

export default DynamicForm;
