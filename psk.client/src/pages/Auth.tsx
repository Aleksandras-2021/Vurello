import React, { useState, useEffect } from 'react';
import { useAuth } from '../components/AuthContext';
import DynamicForm from '../components/DynamicForm';
import { useNavigate } from 'react-router-dom';
import { Card, Typography, Tabs, Row, Col, Alert } from 'antd';
import { loadAllSchemas, isSchemaCacheLoaded } from '../components/SchemaCache';

const Auth: React.FC = () => {
    const { token, login } = useAuth();
    const navigate = useNavigate();
    const [schemaLoading, setSchemaLoading] = useState(false);
    const [schemaLoaded, setSchemaLoaded] = useState(false);

    const handleLoginSuccess = (responseData?: any) => {
        const newToken = responseData?.token;
        if (newToken) {
            login(newToken);
        }
    };

    useEffect(() => {
        const preloadSchemas = async () => {
            if (!isSchemaCacheLoaded()) {
                setSchemaLoading(true);
                await loadAllSchemas();
                setSchemaLoading(false);
                setSchemaLoaded(true);
            } else {
                setSchemaLoaded(true);
            }
        };

        preloadSchemas();
    }, []);

    useEffect(() => {
        if (token) {
            navigate('/teams');
        }
    }, [token, navigate]);

    if (token) return null;

    const handleRegisterSuccess = (responseData?: any) => {
        const newToken = responseData?.token;
        if (newToken) {
            login(newToken);
        }
    };

    return (
        <Row
            justify="center"
            align="middle"
            style={{ padding: 24 }}
        >
            <Col>
                {schemaLoading}
                {schemaLoaded}
                <Card
                    style={{ borderRadius: 12, boxShadow: '0 4px 16px rgba(0, 0, 0, 0.1)', maxWidth: 480 }}
                    size="default"
                >
                    <div style={{ textAlign: 'center', marginBottom: 24 }}>
                        <Typography.Title level={3}>Welcome</Typography.Title>
                        <Typography.Text type="secondary">Login or register to access your account</Typography.Text>
                    </div>
                    <Tabs
                        defaultActiveKey="login"
                        centered
                        items={[
                            {
                                key: 'login',
                                label: 'Login',
                                children: (
                                    <DynamicForm
                                        formTitle="Login"
                                        schemaName="Login"
                                        apiUrl="auth/login"
                                        type='post'
                                        onSuccess={handleLoginSuccess}
                                        noModal={true}
                                    />
                                ),
                            },
                            {
                                key: 'register',
                                label: 'Register',
                                children: (
                                    <DynamicForm
                                        formTitle="Register"
                                        schemaName="Register"
                                        apiUrl="auth/register"
                                        type='post'
                                        onSuccess={handleRegisterSuccess}
                                        noModal={true}
                                    />
                                ),
                            },
                        ]}
                    />
                </Card>
            </Col>
        </Row>
    );
};

export default Auth;