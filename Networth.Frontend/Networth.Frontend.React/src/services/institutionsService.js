/**
 * Service for handling institutions API calls
 */

const API_BASE_URL = 'http://localhost:7071/api';

/**
 * Fetches institutions from the backend API
 * @returns {Promise<Object>} Promise that resolves to the institutions data
 */
export const getInstitutions = async () => {
    try {
        const response = await fetch(`${API_BASE_URL}/institutions`, {
            method: 'GET',
            headers: {
                'accept': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching institutions:', error);
        throw error;
    }
};
