/**
 * Mock Authentication Context for development/demo mode.
 * Provides a fake authenticated state without requiring actual Azure AD login.
 */

import { createContext, useContext, useMemo, ReactNode } from "react";
import type { AccountInfo } from "@azure/msal-browser";

interface MockAuthContextType {
    isAuthenticated: boolean;
    isLoading: boolean;
    isReady: boolean;
    user: AccountInfo | null;
    login: () => Promise<void>;
    logout: () => Promise<void>;
    getAccessToken: () => Promise<string>;
}

const MockAuthContext = createContext<MockAuthContextType | undefined>(
    undefined
);

// Mock user account info matching MSAL AccountInfo structure
const mockAccount: AccountInfo = {
    homeAccountId: "mock-home-account-id",
    localAccountId: "mock-local-account-id",
    environment: "mock.environment.com",
    tenantId: "mock-tenant-id",
    username: "demo@example.com",
    name: "Demo User",
};

export function MockAuthProvider({ children }: { children: ReactNode }) {
    const value = useMemo(
        () => ({
            isAuthenticated: true,
            isLoading: false,
            isReady: true,
            user: mockAccount,
            login: async () => {
                console.log("[MockAuth] Login called - already authenticated");
            },
            logout: async () => {
                console.log("[MockAuth] Logout called - reloading page");
                window.location.reload();
            },
            getAccessToken: async () => {
                console.log("[MockAuth] Returning mock token");
                return "mock-access-token";
            },
        }),
        []
    );

    return (
        <MockAuthContext.Provider value={value}>
            {children}
        </MockAuthContext.Provider>
    );
}

export function useMockAuth() {
    const context = useContext(MockAuthContext);
    if (context === undefined) {
        throw new Error("useMockAuth must be used within a MockAuthProvider");
    }
    return context;
}

/**
 * Hook that works with both real and mock auth contexts.
 * In mock mode, it uses MockAuthContext.
 * In real mode, it should use the real AuthContext.
 */
export { MockAuthContext };
