import React, { useEffect, useState, ReactElement, cloneElement } from 'react';
import Form from '@rjsf/antd';
import { RJSFSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import axios from 'axios';
import { Modal, Button } from 'antd';
import { ButtonProps } from 'antd';

interface DynamicFormProps {
    formTitle: string;
    schemaName: string;
    apiUrl: string;
    neededData?: Record<string, any>;
    onSuccess?: () => void;
    trigger?: ReactElement<Partial<ButtonProps> & { onClick?: () => void }>;
}

const DynamicForm: React.FC<DynamicFormProps> = ({ formTitle, schemaName, apiUrl, neededData, onSuccess, trigger }) => {
    const [formSchema, setFormSchema] = useState<RJSFSchema | null>(null);
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [uiSchema, setUiSchema] = useState<any>({});

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
                        if (value["x-ignore"] === "true") {
                            return;
                        }

                        filteredSchema.properties[key] = value;

                        const uiField: any = {};

                        if (value["x-prompt"]) {
                            uiField["ui:title"] = value["x-prompt"];
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
    }, [schemaName]);

    const handleSubmit = async ({ formData }: any) => {
        try {
            const fullData = {
                ...formData,
                ...neededData,
            };

            const response = await axios.post('https://localhost:7285/api/' + apiUrl, fullData);
            onSuccess?.();
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
            {trigger &&
                React.isValidElement(trigger)
                ? cloneElement(trigger, { onClick: handleOpenModal })
                : null}

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
                        onSubmit={handleSubmit}
                        uiSchema={uiSchema}
                    />
                ) : (
                    <div>Loading</div>
                )}
            </Modal>
        </>
    );
};

export default DynamicForm;
