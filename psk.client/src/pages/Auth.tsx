import React, { useState, useEffect } from 'react';
import { useAuth } from '../components/AuthContext';
import DynamicForm from '../components/DynamicForm';
import { useNavigate } from 'react-router-dom';
import { Card, Typography, Tabs, Row, Col } from 'antd';

const Auth: React.FC = () => {
    const {token, login } = useAuth();
    const navigate = useNavigate();

    const handleLoginSuccess = (responseData?: any) => {
        const newToken = responseData?.token;
        if (newToken) {
            login(newToken);
            navigate('/teams');
        }
    };

    useEffect(() => {
        if (token) {
            navigate('/teams');
        }
    }, [])

    if (token) return null;

    return (
        <Row
            justify="center"
            align="middle"
            style={{ padding: 24 }}
        >
            <Col >
                <Card
                    style={{ borderRadius: 12, boxShadow: '0 4px 16px rgba(0, 0, 0, 0.1)', maxWidth: 480}}
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
                                        onSuccess={handleLoginSuccess}
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
