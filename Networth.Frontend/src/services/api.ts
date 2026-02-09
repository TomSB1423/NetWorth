import type {
    Account,
    AccountCategory,
    Balance,
    Institution,
    Transaction,
    NetWorthHistoryResponse,
    User,
    LinkInstitutionResponse,
    PagedResponse,
} from "../types";
import { config } from "../config/config";
import { mockApi } from "./mockApi";

const API_BASE_URL = `${config.api.baseUrl}/api`;

// Token getter function - will be set by AuthContext
let getAccessTokenFn: (() => Promise<string>) | null = null;

export const setTokenGetter = (fn: () => Promise<string>) => {
    getAccessTokenFn = fn;
};

const getAuthHeaders = async (): Promise<Record<string, string>> => {
    if (!getAccessTokenFn) {
        throw new Error("Token getter not set - auth not ready");
    }
    try {
        const token = await getAccessTokenFn();
        return {
            Authorization: `Bearer ${token}`,
        };
    } catch (error) {
        console.error("Failed to get access token");
        throw error;
    }
};

/**
 * Real API implementation that calls the backend.
 */
const realApi = {
    getAccounts: async (): Promise<Account[]> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/accounts`, { headers });
        if (!response.ok) throw new Error("Failed to fetch accounts");
        return response.json();
    },

    getAccountBalances: async (accountId: string): Promise<Balance[]> => {
        const headers = await getAuthHeaders();
        const response = await fetch(
            `${API_BASE_URL}/accounts/${accountId}/balances`,
            { headers }
        );
        if (!response.ok) throw new Error("Failed to fetch balances");
        return response.json();
    },

    getInstitutions: async (): Promise<Institution[]> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/institutions`, {
            headers,
        });
        if (!response.ok) throw new Error("Failed to fetch institutions");
        return response.json();
    },

    linkInstitution: async (
        institutionId: string
    ): Promise<LinkInstitutionResponse> => {
        const headers = await getAuthHeaders();
        const response = await fetch(
            `${API_BASE_URL}/institutions/${institutionId}/link`,
            {
                method: "POST",
                headers,
            }
        );
        if (!response.ok)
            throw new Error("Failed to initiate institution linking");
        return response.json();
    },

    syncInstitution: async (institutionId: string): Promise<void> => {
        const headers = await getAuthHeaders();
        const response = await fetch(
            `${API_BASE_URL}/institutions/${institutionId}/sync`,
            {
                method: "POST",
                headers,
            }
        );
        if (!response.ok) throw new Error("Failed to sync institution");
        // 202 Accepted returns no body, so don't try to parse JSON
    },

    getTransactions: async (
        accountId: string,
        page: number = 1,
        pageSize: number = 50
    ): Promise<PagedResponse<Transaction>> => {
        const params = new URLSearchParams({
            page: page.toString(),
            pageSize: pageSize.toString(),
        });
        const headers = await getAuthHeaders();
        const response = await fetch(
            `${API_BASE_URL}/accounts/${accountId}/transactions?${params}`,
            { headers }
        );
        if (!response.ok) throw new Error("Failed to fetch transactions");
        return response.json();
    },

    getNetWorthHistory: async (): Promise<NetWorthHistoryResponse> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/statistics/net-worth`, {
            headers,
        });
        if (!response.ok) throw new Error("Failed to fetch net worth history");
        return response.json();
    },

    createUser: async (): Promise<User> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/users`, {
            method: "POST",
            headers,
        });
        if (!response.ok) throw new Error("Failed to create user");
        return response.json();
    },

    getUser: async (): Promise<User> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/users/me`, {
            headers,
        });
        if (!response.ok) throw new Error("Failed to get user");
        return response.json();
    },

    updateUser: async (updates: {
        name?: string;
        hasCompletedOnboarding?: boolean;
    }): Promise<User> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/users/me`, {
            method: "PUT",
            headers: {
                ...headers,
                "Content-Type": "application/json",
            },
            body: JSON.stringify(updates),
        });
        if (!response.ok) throw new Error("Failed to update user");
        return response.json();
    },

    updateAccount: async (
        accountId: string,
        updates: {
            displayName?: string;
            category?: AccountCategory;
        }
    ): Promise<Account> => {
        const headers = await getAuthHeaders();
        const response = await fetch(`${API_BASE_URL}/accounts/${accountId}`, {
            method: "PUT",
            headers: {
                ...headers,
                "Content-Type": "application/json",
            },
            body: JSON.stringify(updates),
        });
        if (!response.ok) throw new Error("Failed to update account");
        return response.json();
    },
};

/**
 * Export the appropriate API based on configuration.
 * When VITE_USE_MOCK_DATA=true, uses mock data instead of real API calls.
 */
export const api = config.useMockData ? mockApi : realApi;

// Log which API mode is active
if (config.useMockData) {
    console.log(
        "%c[API] Mock mode enabled - using local mock data",
        "color: #f59e0b; font-weight: bold"
    );
}
