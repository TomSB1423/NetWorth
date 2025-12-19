import { createContext, useContext, ReactNode } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "../services/api";
import { Account, AccountBalances } from "../types";
import { useAuth } from "./AuthContext";
import { useUser } from "./UserContext";

interface AccountContextType {
    accounts: Account[];
    balances: AccountBalances[];
    isLoading: boolean;
    hasAccounts: boolean;
    error: Error | null;
}

const AccountContext = createContext<AccountContextType | undefined>(undefined);

export function AccountProvider({ children }: { children: ReactNode }) {
    const { isReady } = useAuth();
    const { isProvisioned } = useUser();

    // Only fetch accounts after auth is ready AND user is provisioned
    const canFetch = isReady && isProvisioned;

    const {
        data: accounts = [],
        isLoading: isLoadingAccounts,
        error: accountsError,
    } = useQuery({
        queryKey: ["accounts"],
        queryFn: api.getAccounts,
        enabled: canFetch,
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
        enabled: canFetch && accounts.length > 0,
    });

    const value = {
        accounts,
        balances,
        isLoading: isLoadingAccounts || isLoadingBalances,
        hasAccounts: accounts.length > 0,
        error: accountsError as Error | null,
    };

    return (
        <AccountContext.Provider value={value}>
            {children}
        </AccountContext.Provider>
    );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAccounts() {
    const context = useContext(AccountContext);
    if (context === undefined) {
        throw new Error("useAccounts must be used within an AccountProvider");
    }
    return context;
}
