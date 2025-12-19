import {
    createContext,
    useContext,
    useCallback,
    useMemo,
    useEffect,
    useSyncExternalStore,
    ReactNode,
} from "react";
import { useMsal, useIsAuthenticated, useAccount } from "@azure/msal-react";
import { InteractionStatus, AccountInfo } from "@azure/msal-browser";
import { loginRequest, tokenRequest } from "../config/authConfig";
import { setTokenGetter } from "../services/api";

interface AuthContextType {
    isAuthenticated: boolean;
    isLoading: boolean;
    isReady: boolean; // True when auth is complete AND token getter is set
    user: AccountInfo | null;
    login: () => Promise<void>;
    logout: () => Promise<void>;
    getAccessToken: () => Promise<string>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// External store for token getter configuration status
let tokenGetterConfigured = false;
const tokenGetterListeners = new Set<() => void>();

const subscribeToTokenGetter = (callback: () => void): (() => void) => {
    tokenGetterListeners.add(callback);
    return () => tokenGetterListeners.delete(callback);
};

const getTokenGetterSnapshot = (): boolean => tokenGetterConfigured;

const setTokenGetterStatus = (value: boolean): void => {
    tokenGetterConfigured = value;
    tokenGetterListeners.forEach((listener) => listener());
};

export function AuthProvider({ children }: { children: ReactNode }) {
    const { instance, accounts, inProgress } = useMsal();
    const isAuthenticated = useIsAuthenticated();
    const account = useAccount(accounts[0] || {});
    const isLoading = inProgress !== InteractionStatus.None;

    // Use external store to avoid setState in effect
    const isTokenGetterConfigured = useSyncExternalStore(
        subscribeToTokenGetter,
        getTokenGetterSnapshot
    );

    const login = useCallback(async () => {
        try {
            await instance.loginPopup(loginRequest);
        } catch (error) {
            console.error("Login failed:", error);
            throw error;
        }
    }, [instance]);

    const logout = useCallback(async () => {
        try {
            await instance.logoutPopup({
                postLogoutRedirectUri: window.location.origin,
                mainWindowRedirectUri: window.location.origin,
            });
        } catch (error) {
            console.error("Logout failed:", error);
            throw error;
        }
    }, [instance]);

    const getAccessToken = useCallback(async (): Promise<string> => {
        if (!account) {
            throw new Error("No active account");
        }

        try {
            // Try to acquire token silently first
            const response = await instance.acquireTokenSilent({
                ...tokenRequest,
                account: account,
            });
            return response.accessToken;
        } catch {
            // If silent acquisition fails, try interactive
            console.warn("Silent token acquisition failed, trying interactive");
            const response = await instance.acquireTokenPopup(tokenRequest);
            return response.accessToken;
        }
    }, [instance, account]);

    // Set up the token getter for API calls - synchronize with external MSAL system
    useEffect(() => {
        if (isAuthenticated && account) {
            console.log("Auth ready - setting token getter");
            setTokenGetter(getAccessToken);
            setTokenGetterStatus(true);
        }
        return () => {
            setTokenGetterStatus(false);
        };
    }, [isAuthenticated, account, getAccessToken]);

    // Derive isReady from state rather than setting it in an effect
    const isReady =
        isAuthenticated && account != null && isTokenGetterConfigured;

    const value = useMemo(
        () => ({
            isAuthenticated,
            isLoading,
            isReady,
            user: account,
            login,
            logout,
            getAccessToken,
        }),
        [
            isAuthenticated,
            isLoading,
            isReady,
            account,
            login,
            logout,
            getAccessToken,
        ]
    );

    return (
        <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
}
