import {
    Account,
    Balance,
    Institution,
    Transaction,
    NetWorthDataPoint,
} from "../types";
import { config } from "../config";

const API_BASE_URL = `${config.api.baseUrl}/api`;

export const api = {
    getAccounts: async (): Promise<Account[]> => {
        const response = await fetch(`${API_BASE_URL}/accounts`);
        if (!response.ok) throw new Error("Failed to fetch accounts");
        return response.json();
    },

    getAccountBalances: async (accountId: string): Promise<Balance[]> => {
        const response = await fetch(
            `${API_BASE_URL}/accounts/${accountId}/balances`
        );
        if (!response.ok) throw new Error("Failed to fetch balances");
        return response.json();
    },

    getInstitutions: async (): Promise<Institution[]> => {
        const response = await fetch(`${API_BASE_URL}/institutions`);
        if (!response.ok) throw new Error("Failed to fetch institutions");
        return response.json();
    },

    linkInstitution: async (
        institutionId: string
    ): Promise<{ link: string }> => {
        const response = await fetch(
            `${API_BASE_URL}/institutions/${institutionId}/link`,
            {
                method: "POST",
            }
        );
        if (!response.ok)
            throw new Error("Failed to initiate institution linking");
        return response.json();
    },

    syncInstitution: async (institutionId: string): Promise<void> => {
        const response = await fetch(
            `${API_BASE_URL}/institutions/${institutionId}/sync`,
            {
                method: "POST",
            }
        );
        if (!response.ok) throw new Error("Failed to sync institution");
        return response.json();
    },

    getTransactions: async (
        accountId: string,
        dateFrom: string,
        dateTo: string
    ): Promise<Transaction[]> => {
        const params = new URLSearchParams({
            dateFrom,
            dateTo,
        });
        const response = await fetch(
            `${API_BASE_URL}/accounts/${accountId}/transactions?${params}`
        );
        if (!response.ok) throw new Error("Failed to fetch transactions");
        return response.json();
    },

    getNetWorthHistory: async (): Promise<NetWorthDataPoint[]> => {
        const response = await fetch(`${API_BASE_URL}/statistics/net-worth`);
        if (!response.ok) throw new Error("Failed to fetch net worth history");
        return response.json();
    },
};
