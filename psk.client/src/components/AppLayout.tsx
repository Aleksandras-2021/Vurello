import React from 'react';
import { Layout } from 'antd';

const AppLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    return (
        <Layout style={{ marginLeft: 100}}>
            <div style={{ padding: 24 }}>
                {children}
            </div>
        </Layout>
    );
};

export default AppLayout;