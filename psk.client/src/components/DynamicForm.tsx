import React, { useEffect, useState, ReactElement, cloneElement } from 'react';
import Form from '@rjsf/antd';
import { RJSFSchema, UiSchema } from '@rjsf/utils';
import validator from '@rjsf/validator-ajv8';
import { Modal, Spin, Button, Row, Col } from 'antd';
import { ButtonProps } from 'antd';
import { api } from './API';
import UuidDropdownWidget from './UuidDropdownWidget';
import RichTextWidget from './RichTextWidget';
import ColorPickWidget from './ColorPickWidget';
import LabelSelectionWidget from './LabelSelectionWidget';
import { getSchema, getUiSchema, loadAllSchemas, isSchemaCacheLoaded, getErrorMessageMap } from './SchemaCache';
import { toast } from 'react-toastify';

const styles = `
  .hide-label .ant-form-item-label {
    display: none !important;
  }
`;

const styleSheet = document.createElement("style");
styleSheet.innerText = styles;
document.head.appendChild(styleSheet);

interface DynamicFormProps {
    formTitle: string;
    schemaName?: string;
    schema?: RJSFSchema;
    uiSchema?: UiSchema;
    apiUrl: string;
    type: 'post' | 'patch' | 'put';
    neededData?: Record<string, any>;
    onSuccess?: (responseData?: any) => void;
    trigger?: ReactElement<Partial<ButtonProps> & { onClick?: () => void }>;
    noModal?: boolean;
    currentData?: Record<string, any>;
    dropdownOptions?: Record<string, Array<{ value: string; label: string }>>;
    onCancelConflict?: (updatedData?: Record<string, any>) => void;
    fetchCurrentData?: () => Promise<Record<string, any>>;
}

const DynamicForm: React.FC<DynamicFormProps> = ({
    formTitle,
    schemaName,
    schema,
    uiSchema,
    apiUrl,
    neededData,
    onSuccess,
    trigger,
    noModal,
    type,
    currentData = {},
    dropdownOptions,
    onCancelConflict,
    fetchCurrentData
}) => {
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [formData, setFormData] = useState<Record<string, any>>({});
    const [loading, setLoading] = useState(false);
    const [formSchema, setFormSchema] = useState<RJSFSchema | null>(null);
    const [formUiSchema, setFormUiSchema] = useState<UiSchema>({});
    const [errorMessageMap, setErrorMessageMap] = useState<Record<string, Record<string, string>>>({});

    const [conflictData, setConflictData] = useState<Record<string, any> | null>(null);
    const [userAttemptData, setUserAttemptData] = useState<Record<string, any> | null>(null);
    const [retrying, setRetrying] = useState(false);

    const widgets = {
        uuidDropdown: UuidDropdownWidget,
        richText: RichTextWidget,
        colorPick: ColorPickWidget,
        labelSelection: LabelSelectionWidget,
    };

    useEffect(() => {
        const loadSchema = async () => {
            setLoading(true);

            if (schema) {
                setFormSchema(schema);
                setFormUiSchema(uiSchema ?? getUiSchema(schemaName ?? "") ?? {});
                setErrorMessageMap(getErrorMessageMap(schemaName ?? "") ?? {});
                setLoading(false);
                return;
            }

            if (!schemaName) {
                setLoading(false);
                return;
            }

            if (!isSchemaCacheLoaded()) {
                await loadAllSchemas();
            }

            const cachedSchema = getSchema(schemaName);
            const cachedUiSchema = getUiSchema(schemaName);
            const cachedErrorMessageMap = getErrorMessageMap(schemaName);

            if (cachedSchema) {
                const clonedSchema = JSON.parse(JSON.stringify(cachedSchema));
                const clonedUiSchema = JSON.parse(JSON.stringify(cachedUiSchema ?? {}));

                if (dropdownOptions && clonedSchema.properties) {
                    Object.entries(clonedSchema.properties).forEach(([key, value]: [string, any]) => {
                        if (value["x-dropdown"]) {
                            const dropdownType = value["x-dropdown"];
                            const optionsForType = dropdownOptions[dropdownType];

                            if (optionsForType) {
                                clonedSchema.properties[key].enum = optionsForType.map(option => option.value);
                                clonedSchema.properties[key].enumNames = optionsForType.map(option => option.label);
                            }
                        }
                    });
                }

                setFormSchema(clonedSchema);
                setFormUiSchema(clonedUiSchema);
                setErrorMessageMap(cachedErrorMessageMap ?? {});
            } else {
                console.error(`Schema "${schemaName}" not found in cache`);
            }

            setLoading(false);
        };

        loadSchema();
    }, [schemaName, schema, uiSchema, dropdownOptions]);

    useEffect(() => {
        if ((type === 'patch' || type === 'put') && currentData && formSchema) {
            const validKeys = Object.keys(formSchema.properties || {});
            const filteredData = Object.fromEntries(
                Object.entries(currentData).filter(([key]) => validKeys.includes(key))
            );
            if (filteredData.labels && !Array.isArray(filteredData.labels)) {
                filteredData.labels = [];
            }
            setFormData(filteredData);
        }
    }, [currentData, formSchema, type]);

    useEffect(() => {
        if (!isModalVisible) {
            setConflictData(null);
            setUserAttemptData(null);
            setRetrying(false);
        }
    }, [isModalVisible]);

    const handleCloseModal = () => {
        setIsModalVisible(false);
        setConflictData(null);
        setUserAttemptData(null);
        setRetrying(false);
    };

    const handleSubmit = async ({ formData: submittedData, isRetrying = false }: { formData?: any, isRetrying?: boolean }) => {
        try {
            setLoading(true);
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

            if ((type === 'patch' || type === 'put') && currentData && currentData.version !== undefined && !isRetrying) {
                payload.version = currentData.version;
            }

            const fullData = {
                ...payload,
                ...neededData,
            };

            const method = type === 'put' ? 'put' : type === 'patch' ? 'patch' : 'post';
            const response = await api[method](apiUrl, fullData);
            const updatedEntity = response.data;

            toast.success(`${formTitle || 'Form'} ${type === 'patch' || type === 'put' ? 'updated' : 'created'} successfully`);
            onSuccess?.(updatedEntity);
            handleCloseModal();
        } catch (error: any) {
            if (error?.response?.status === 409) {
                let latestEntity = null;
                if (fetchCurrentData) {
                    try {
                        latestEntity = await fetchCurrentData();
                        console.log(latestEntity)
                    } catch (fetchError) {
                        console.error("Failed to fetch latest data after conflict", fetchError);
                    }
                }
                if (latestEntity) {
                    setConflictData(latestEntity);
                    setUserAttemptData(submittedData);
                } else {
                    toast.error("Conflict detected, but unable to fetch latest data.");
                }
            } else {
                console.error("Form submission error:", error);
                toast.error(error?.response?.data?.message || "Submission failed: Invalid data.");
            }
        } finally {
            setLoading(false);
        }
    };

    const handleRetry = async () => {
        if (!conflictData || !userAttemptData) return;

        const updatedAttempt = { ...userAttemptData, version: conflictData.version };
        setUserAttemptData(updatedAttempt);
        setFormData(updatedAttempt);
        setConflictData(null);
        setRetrying(true);

        try {
            await handleSubmit({ formData: updatedAttempt, isRetrying: true });
        } finally {
            setRetrying(false);
        }
    };

    const handleCancelConflict = () => {
        setConflictData(null);
        setUserAttemptData(null);
        setRetrying(false);
        onCancelConflict?.(conflictData ?? undefined);
        toast.success("Data refreshed");
    };

    if (loading) {
        return trigger
            ? cloneElement(trigger, { disabled: true })
            : <Spin size="small" />;
    }

    if (!formSchema) {
        return trigger
            ? cloneElement(trigger, { disabled: true })
            : <div>Schema not found</div>;
    }

    const transformErrors = (errors: any[]) => {
        return errors.map(error => {
            if (error.property === '.labels' && error.name === 'type') {
                return { ...error, message: 'Labels must be a list of valid label IDs.' };
            }
            const pathSegments = error.property.split('.');
            const fieldName = pathSegments[pathSegments.length - 1];
            const property = error.property.startsWith('.') ? error.property.substring(1) : error.property;
            if (errorMessageMap[property] && errorMessageMap[property][error.name]) {
                return { ...error, message: errorMessageMap[property][error.name] };
            } else if (errorMessageMap[fieldName] && errorMessageMap[fieldName][error.name]) {
                return { ...error, message: errorMessageMap[fieldName][error.name] };
            }
            return error;
        });
    };

    const baseForm = (
        <Form
            schema={formSchema}
            uiSchema={formUiSchema}
            validator={validator}
            formData={formData}
            widgets={widgets}
            onSubmit={handleSubmit}
            transformErrors={transformErrors}
            showErrorList={false}
        />
    );

    if (conflictData) {
        return (
            <Modal
                title={`${formTitle || 'Concurrency Conflict'}`}
                open={true}
                onCancel={handleCancelConflict}
                footer={null}
                width={900}
                destroyOnClose
            >
                <Row gutter={24}>
                    <Col span={12}>
                        <h3>Your Changes</h3>
                        <Form
                            schema={formSchema}
                            uiSchema={formUiSchema}
                            validator={validator}
                            formData={userAttemptData}
                            widgets={widgets}
                            onChange={({ formData }) => setUserAttemptData(formData)}
                            transformErrors={transformErrors}
                            showErrorList={false}
                            onSubmit={() => handleRetry()}
                        >
                            <div>
                                <Button type="primary" htmlType="submit" loading={retrying}>
                                    Submit your data
                                </Button>
                            </div>
                        </Form>
                    </Col>
                    <Col span={12}>
                        <h3>Current Data</h3>
                        <Form
                            schema={formSchema}
                            uiSchema={formUiSchema}
                            validator={validator}
                            formData={conflictData}
                            widgets={widgets}
                            disabled={true}
                            transformErrors={transformErrors}
                            showErrorList={false}
                        >
                            <div>
                                <Button type="primary" onClick={handleCancelConflict} loading={retrying}>
                                    Refresh your data
                                </Button>
                            </div>
                        </Form>
                    </Col>
                </Row>
            </Modal>
        );
    }

    return (
        <>
            {trigger && React.isValidElement(trigger)
                ? cloneElement(trigger, { onClick: () => setIsModalVisible(true) })
                : null}
            {noModal ? (
                baseForm
            ) : (
                <Modal
                    title={formTitle || null}
                    open={isModalVisible}
                    onCancel={() => setIsModalVisible(false)}
                    footer={null}
                    width={600}
                    destroyOnClose
                >
                    {baseForm}
                </Modal>
            )}
        </>
    );
};

export default DynamicForm;