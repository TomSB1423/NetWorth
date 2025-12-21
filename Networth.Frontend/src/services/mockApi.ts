/**
 * Mock API implementation for development and demo purposes.
 * Provides the same interface as the real API but returns mock data.
 */

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

import {
    mockAccounts,
    mockInstitutions,
    mockUser,
    getMockBalances,
    getMockNetWorthHistory,
    getMockTransactions,
    getMockLinkInstitutionResponse,
} from "./mockData";

// Simulate network delay for realistic behavior
const MOCK_DELAY_MS = 300;

function delay(ms: number): Promise<void> {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

// In-memory state for mock data
let mutableAccounts = [...mockAccounts];
let mutableUser = { ...mockUser };

export const mockApi = {
    getAccounts: async (): Promise<Account[]> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getAccounts");
        return mutableAccounts;
    },

    getAccountBalances: async (accountId: string): Promise<Balance[]> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getAccountBalances:", accountId);
        return getMockBalances(accountId);
    },

    getInstitutions: async (): Promise<Institution[]> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getInstitutions");
        return mockInstitutions;
    },

    linkInstitution: async (
        institutionId: string
    ): Promise<LinkInstitutionResponse> => {
        await delay(MOCK_DELAY_MS * 2);
        console.log("[MockAPI] linkInstitution:", institutionId);
        return getMockLinkInstitutionResponse(institutionId);
    },

    syncInstitution: async (institutionId: string): Promise<void> => {
        await delay(MOCK_DELAY_MS * 2);
        console.log("[MockAPI] syncInstitution:", institutionId);
        // Update lastSynced for affected accounts
        mutableAccounts = mutableAccounts.map((acc) =>
            acc.institutionId === institutionId
                ? { ...acc, lastSynced: new Date().toISOString() }
                : acc
        );
    },

    getTransactions: async (
        accountId: string,
        page: number = 1,
        pageSize: number = 50
    ): Promise<PagedResponse<Transaction>> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getTransactions:", accountId, page, pageSize);
        return getMockTransactions(accountId, page, pageSize);
    },

    getNetWorthHistory: async (): Promise<NetWorthHistoryResponse> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getNetWorthHistory");
        return getMockNetWorthHistory();
    },

    createUser: async (): Promise<User> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] createUser");
        mutableUser = {
            ...mockUser,
            isNewUser: true,
            hasCompletedOnboarding: false,
        };
        return mutableUser;
    },

    getUser: async (): Promise<User> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] getUser");
        return mutableUser;
    },

    updateUser: async (updates: {
        name?: string;
        hasCompletedOnboarding?: boolean;
    }): Promise<User> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] updateUser:", updates);
        mutableUser = { ...mutableUser, ...updates };
        return mutableUser;
    },

    updateAccount: async (
        accountId: string,
        updates: {
            displayName?: string;
            category?: AccountCategory;
        }
    ): Promise<Account> => {
        await delay(MOCK_DELAY_MS);
        console.log("[MockAPI] updateAccount:", accountId, updates);

        const accountIndex = mutableAccounts.findIndex(
            (acc) => acc.id === accountId
        );
        if (accountIndex === -1) {
            throw new Error("Account not found");
        }

        mutableAccounts[accountIndex] = {
            ...mutableAccounts[accountIndex],
            ...updates,
        };

        return mutableAccounts[accountIndex];
    },
};

/**
 * Reset mock data to initial state.
 * Useful for testing scenarios.
 */
export function resetMockData(): void {
    mutableAccounts = [...mockAccounts];
    mutableUser = { ...mockUser };
}
