/**
 * Mock Authentication Context for development/demo mode.
 * Provides a fake authenticated state without requiring actual Firebase login.
 * Users start unauthenticated and can login/logout normally.
 */

import {
    createContext,
    useContext,
    useMemo,
    useState,
    useCallback,
    ReactNode,
} from "react";
import type { FirebaseUserInfo } from "./AuthContext";

interface MockAuthContextType {
    isAuthenticated: boolean;
    isLoading: boolean;
    isReady: boolean;
    user: FirebaseUserInfo | null;
    login: () => Promise<void>;
    logout: () => Promise<void>;
    getAccessToken: () => Promise<string>;
}

const MockAuthContext = createContext<MockAuthContextType | undefined>(
    undefined,
);

// Session storage key for mock auth state
const MOCK_AUTH_KEY = "mock_auth_authenticated";

// Mock user account info matching Firebase user structure
const mockUser: FirebaseUserInfo = {
    uid: "mock-user-123",
    email: "demo@example.com",
    displayName: "Demo User",
    photoURL: null,
};

export function MockAuthProvider({ children }: { children: ReactNode }) {
    // Check session storage for existing auth state
    const [isAuthenticated, setIsAuthenticated] = useState(() => {
        return sessionStorage.getItem(MOCK_AUTH_KEY) === "true";
    });

    const login = useCallback(async () => {
        sessionStorage.setItem(MOCK_AUTH_KEY, "true");
        setIsAuthenticated(true);
    }, []);

    const logout = useCallback(async () => {
        sessionStorage.removeItem(MOCK_AUTH_KEY);
        sessionStorage.removeItem("welcome_shown");
        setIsAuthenticated(false);
    }, []);

    const getAccessToken = useCallback(async () => {
        return "mock-access-token";
    }, []);

    const value = useMemo(
        () => ({
            isAuthenticated,
            isLoading: false,
            isReady: isAuthenticated, // Ready when authenticated
            user: isAuthenticated ? mockUser : null,
            login,
            logout,
            getAccessToken,
        }),
        [isAuthenticated, login, logout, getAccessToken],
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
