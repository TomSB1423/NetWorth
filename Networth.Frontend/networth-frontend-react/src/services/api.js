/**
 * API service for interacting with the NetWorth backend
 */

const API_BASE_URL = process.env.REACT_APP_BACKEND_URL;

/**
 * Generic API request function with error handling
 */
async function apiRequest(endpoint, options = {}) {
    const url = `${API_BASE_URL}${endpoint}`;

    const config = {
        headers: {
            'Content-Type': 'application/json',
            ...options.headers,
        },
        ...options,
    };

    try {
        const response = await fetch(url, config);

        if (!response.ok) {
            throw new Error(`API request failed: ${response.status} ${response.statusText}`);
        }

        return await response.json();
    } catch (error) {
        console.error(`API request to ${url} failed:`, error);
        throw error;
    }
}

/**
 * Institution-related API calls
 */
export const institutionService = {
    /**
     * Fetch all available institutions
     */
    async getInstitutions() {
        return apiRequest('/api/institutions');
    },

    /**
     * Fetch a specific institution by ID
     */
    async getInstitution(id) {
        return apiRequest(`/api/institutions/${id}`);
    },
};

export default institutionService;
