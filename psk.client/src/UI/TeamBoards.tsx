import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';

const TeamBoards = () => {
    const { teamId } = useParams(); 

    useEffect(() => {

    }, [teamId])

    return (
        <div>Team id {teamId}</div>
    );
};

export default TeamBoards;
