import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import DynamicForm from '../components/DynamicForm'
import {Button, Spin } from 'antd';
import { api } from "../components/API"


const TeamBoards = () => {
    const { teamId } = useParams(); 
    const [team, setTeam] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    const fetchTeam = async () => {
        try {
            const response = await api.get(`team/${teamId}`);
            setTeam(response.data);
            console.log(response.data);
        } catch (error) {
            console.error('Failed to fetch team:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchTeam();
    }, [teamId])

    if (loading) {
        return <Spin size="large" />;
    };

    return (
        <div style={{ padding: 24 }}>
            Team name {team.name}

            <DynamicForm
                formTitle="Edit team"
                schemaName="TeamUpdate"
                apiUrl={"team/" + teamId}
                type='patch'
                onSuccess={fetchTeam}
                trigger={
                    <Button type="primary">
                        Edit Team
                    </Button>
                }
                currentData={team }
            />
        </div>
    );
};

export default TeamBoards;
