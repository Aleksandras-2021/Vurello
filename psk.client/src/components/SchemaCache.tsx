import axios from 'axios';
import { RJSFSchema } from '@rjsf/utils';

interface SchemaCache {
    schemas: Record<string, RJSFSchema>;
    uiSchemas: Record<string, any>;
    errorMessageMaps: Record<string, Record<string, Record<string, string>>>;
    isLoaded: boolean;
}

const schemaCache: SchemaCache = {
    schemas: {},
    uiSchemas: {},
    errorMessageMaps: {},
    isLoaded: false
};

export const loadAllSchemas = async (): Promise<void> => {
    if (schemaCache.isLoaded) return;

    try {
        const response = await axios.get('https://localhost:7285/swagger/v1/swagger.json');
        const swaggerDoc = response.data;

        if (!swaggerDoc.components?.schemas) {
            console.error('No schemas found in Swagger document');
            return;
        }

        Object.entries(swaggerDoc.components.schemas).forEach(([schemaName, schema]: [string, any]) => {
            const filteredSchema: any = {
                ...schema,
                properties: {},
            };

            const uiSchema: any = {
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

            const errorMessageMap: Record<string, Record<string, string>> = {};
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
                        uiField["ui:widget"] = "uuidDropdown";
                    }

                    if (value["x-richText"] === "true") {
                        uiField["ui:widget"] = "richText";
                    }

                    if (value["x-colorPick"] === "true") {
                        uiField["ui:widget"] = "colorPick";
                        if (!value.default) {
                            filteredSchema.properties[key].default = "#ffffff";
                        }
                    }

                    uiSchema[key] = uiField;

                    const fieldTitle = value["x-prompt"] || key;
                    errorMessageMap[key] = {
                        required: `${fieldTitle} is required`,
                        minLength: `${fieldTitle} must have at least ${value.minLength || 0} characters`,
                        maxLength: `${fieldTitle} cannot exceed ${value.maxLength || 0} characters`,
                        format: `Please enter a valid ${fieldTitle.toLowerCase()}`,
                        pattern: `${fieldTitle} has an invalid format`,
                        enum: `Please select a valid option for ${fieldTitle.toLowerCase()}`
                    };
                });
            }

            schemaCache.schemas[schemaName] = filteredSchema;
            schemaCache.uiSchemas[schemaName] = uiSchema;
            schemaCache.errorMessageMaps[schemaName] = errorMessageMap;
        });

        schemaCache.isLoaded = true;
        console.log('Schemas loaded and cached');
    } catch (error) {
        console.error('Error loading schemas:', error);
    }
};

export const getSchema = (schemaName: string): RJSFSchema | null => {
    return schemaCache.schemas[schemaName] || null;
};

export const getUiSchema = (schemaName: string): any => {
    return schemaCache.uiSchemas[schemaName] || {};
};

export const getErrorMessageMap = (schemaName: string): Record<string, Record<string, string>> => {
    return schemaCache.errorMessageMaps[schemaName] || {};
};

export const isSchemaCacheLoaded = (): boolean => {
    return schemaCache.isLoaded;
};