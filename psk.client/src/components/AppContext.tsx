import React, { createContext, useContext, useState, ReactNode } from 'react';

type AppContextType = {
    lastTeamId: string | null;
    lastBoardId: string | null;
    creatorId: string | null;
    setLastTeamId: (id: string) => void;
    setLastBoardId: (id: string) => void;
    setCreatorId: (id: string) => void;
};

const AppContext = createContext<AppContextType | undefined>(undefined);

export const AppProvider = ({ children }: { children: ReactNode }) => {
    const [lastTeamId, setLastTeamId] = useState<string | null>(null);
    const [lastBoardId, setLastBoardId] = useState<string | null>(null);
    const [creatorId, setCreatorId] = useState<string | null>(null);


    return (
        <AppContext.Provider value={{ lastTeamId, lastBoardId, creatorId, setLastTeamId, setLastBoardId, setCreatorId }}>
            {children}
        </AppContext.Provider>
    );
};

export const useAppContext = (): AppContextType => {
    const context = useContext(AppContext);
    if (!context) {
        throw new Error('useAppContext must be used within an AppProvider');
    }
    return context;
};