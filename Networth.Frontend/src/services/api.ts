import {
    Account,
    AccountCategory,
    Balance,
    Institution,
    Transaction,
    NetWorthDataPoint,
    User,
    LinkInstitutionResponse,
    PagedResponse,
} from "../types";
import { config } from "../config/config";

const API_BASE_URL = `${config.api.baseUrl}/api`;

// Token getter function - will be set by AuthContext
let getAccessTokenFn: (() => Promise<string>) | null = null;

export const setTokenGetter = (fn: () => Promise<string>) => {
    console.log("Token getter set");
    getAccessTokenFn = fn;
};

const getAuthHeaders = async (): Promise<Record<string, string>> => {
    if (!getAccessTokenFn) {
        console.warn("Token getter not set, skipping auth header");
        return {};
    }
    try {
        const token = await getAccessTokenFn();
        console.log("Got access token, length:", token.length);
        return {
            Authorization: `Bearer ${token}`,
        };
    } catch (error) {
        console.error("Failed to get access token:", error);
        throw error; // Propagate the error instead of silently returning empty headers
    }
};

export const api = {
    getAccounts: async (): Promise<Account[]> => {
        const headers = await getAuthHeaders();
        console.log(
            "Making API call to /accounts with headers:",
            JSON.stringify(headers)
        );
        const response = await fetch(`${API_BASE_URL}/accounts`, { headers });
        console.log("API response status:", response.status);
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

    getNetWorthHistory: async (): Promise<NetWorthDataPoint[]> => {
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
