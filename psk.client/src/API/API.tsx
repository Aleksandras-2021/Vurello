import axios from 'axios';

export const api = {
    get: async <T = any>(url: string, config = {}): Promise<T> => {
        try {
            const response = await axios.get<T>('https://localhost:7285/api/' + url, config);
            return response.data;
        } catch (error: any) {
            console.error('GET request failed:', error);
            throw error;
        }
    },
};