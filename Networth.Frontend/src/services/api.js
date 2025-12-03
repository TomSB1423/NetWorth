/**
 * API service for interacting with the NetWorth backend
 */

// Backend URL is injected by Aspire via REACT_APP_BACKEND_URL environment variable
const API_BASE_URL = process.env.REACT_APP_BACKEND_URL || 'http://localhost:7071';

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
            const errorText = await response.text();
            throw new Error(`API request failed: ${response.status} ${response.statusText}. ${errorText}`);
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
     * Sync institution accounts
     */
    async syncInstitution(institutionId) {
        return apiRequest(`/api/institutions/${institutionId}/sync`, {
            method: 'POST',
        });
    },
};

/**
 * Account-related API calls
 */
export const accountService = {
    /**
     * Link a new bank account
     */
    async linkAccount(institutionId) {
        return apiRequest('/api/account/link', {
            method: 'POST',
            body: JSON.stringify({ institutionId }),
        });
    },

    /**
     * Get all user accounts
     */
    async getAccounts() {
        return apiRequest('/api/accounts');
    },

    /**
     * Get a specific account
     */
    async getAccount(accountId) {
        return apiRequest(`/api/accounts/${accountId}`);
    },

    /**
     * Get account balances
     */
    async getAccountBalances(accountId) {
        return apiRequest(`/api/accounts/${accountId}/balances`);
    },

    /**
     * Get account details
     */
    async getAccountDetails(accountId) {
        return apiRequest(`/api/accounts/${accountId}/details`);
    },

    /**
     * Get account transactions
     */
    async getAccountTransactions(accountId, dateFrom, dateTo) {
        const params = new URLSearchParams();
        if (dateFrom) params.append('dateFrom', dateFrom);
        if (dateTo) params.append('dateTo', dateTo);

        const queryString = params.toString();
        const endpoint = `/api/accounts/${accountId}/transactions${queryString ? `?${queryString}` : ''}`;

        return apiRequest(endpoint);
    },
};

/**
 * Requisition-related API calls
 */
export const requisitionService = {
    /**
     * Get requisition status
     */
    async getRequisition(requisitionId) {
        return apiRequest(`/api/requisitions/${requisitionId}`);
    },
};

/**
 * Development-only API calls
 */
export const devService = {
    /**
     * Reset the database (development only)
     */
    async resetDatabase() {
        return apiRequest('/api/dev/reset-database', {
            method: 'POST',
        });
    },
};

export default {
    institutionService,
    accountService,
    requisitionService,
    devService,
};
