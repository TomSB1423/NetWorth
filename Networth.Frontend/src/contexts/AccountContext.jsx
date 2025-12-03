import React, { createContext, useContext } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "../services/api";

const AccountContext = createContext();

export function AccountProvider({ children }) {
    const {
        data: accounts = [],
        isLoading: isLoadingAccounts,
        error: accountsError,
    } = useQuery({
        queryKey: ["accounts"],
        queryFn: api.getAccounts,
    });

    // Fetch balances for all accounts
    // In a real app, we might want to fetch this on demand or optimize
    const { data: balances = [], isLoading: isLoadingBalances } = useQuery({
        queryKey: ["balances", accounts],
        queryFn: async () => {
            if (!accounts.length) return [];
            const promises = accounts.map((account) =>
                api
                    .getAccountBalances(account.id)
                    .then((b) => ({ accountId: account.id, balances: b }))
                    .catch(() => ({ accountId: account.id, balances: [] }))
            );
            return Promise.all(promises);
        },
        enabled: accounts.length > 0,
    });

    const value = {
        accounts,
        balances,
        isLoading: isLoadingAccounts || isLoadingBalances,
        hasAccounts: accounts.length > 0,
        error: accountsError,
    };

    return (
        <AccountContext.Provider value={value}>
            {children}
        </AccountContext.Provider>
    );
}

export function useAccounts() {
    const context = useContext(AccountContext);
    if (context === undefined) {
        throw new Error("useAccounts must be used within an AccountProvider");
    }
    return context;
}
