import axios from 'axios';
import { toast } from 'react-toastify';


export const api = axios.create({
    baseURL: 'https://localhost:7285/api/',
});

let refreshTokenFn: (() => Promise<string | null>) | null = null;

export const setRefreshTokenFn = (fn: () => Promise<string | null>) => {
    refreshTokenFn = fn;
};

let refreshPromise: Promise<string | null> | null = null;

api.interceptors.request.use(async (config) => {
    if (refreshTokenFn) {
        const exp = localStorage.getItem('tokenExpiration');
        const tokenExpiration = exp ? parseInt(exp, 10) : null;
        const now = Date.now() / 1000;

        if (tokenExpiration && tokenExpiration - now < 60 * 60) {
            if (!refreshPromise) {
                refreshPromise = refreshTokenFn()
                    .catch(err => {
                        throw err;
                    })
                    .finally(() => {
                        refreshPromise = null;
                    });
            }

            try {
                await refreshPromise;
            } catch (err) {
                console.error(err);
            }
        }
    }

    const token = localStorage.getItem('token');
    if (token) {
        config.headers!['Authorization'] = `Bearer ${token}`;
    }


    return config;
});

api.interceptors.response.use(
    response => response,
    error => {
        const response = error.response;

        if (response?.data && typeof response.data === 'object') {
            const { title, detail, message } = response.data;
            const status = response.status;

            const errorMessage =
                title || detail || message || `An error occurred (status ${status || 'unknown'})`;

            toast.error(errorMessage);
        } else {
            toast.error(error.message || 'An unexpected error occurred.');
        }

        return Promise.reject(error);
    }
);

export const getUserTeams = async () => {
    const response = await api.get('team');
    return response.data;
};

export const getTeamMembers = async (teamId: string) => {
    const response = await api.get(`team/${teamId}/members`);
    return response.data;
}

export const inviteUserToTeam = async (teamId: string, username: string) => {
    const response = await api.post('invitation', {
        recipientUsername: username,
        teamId
    });
    return response.data;
};

export const getUserInvitations = async () => {
    const response = await api.get('invitation/inbox');
    return response.data;
};

export const respondToInvitation = async (invitationId: string, accept: boolean) => {
    const response = await api.post(`invitation/${invitationId}/respond`, accept, {
        headers: {
            'Content-Type': 'application/json'
        }
    });
    return response.data;
};