import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Teams from './UI/Teams';
import TeamBoards from './UI/TeamBoards';


const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Navigate to="/teams" />} />
                <Route path="/teams" element={<Teams />} />
                <Route path="/teams/:teamId" element={<TeamBoards />} />
            </Routes>
        </Router>
    );
};

export default App;
