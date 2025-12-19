import {
    createContext,
    useContext,
    useEffect,
    useState,
    useMemo,
    useCallback,
    ReactNode,
} from "react";
import { useAuth } from "./AuthContext";
import { api } from "../services/api";
import { User } from "../types";

interface UserContextType {
    user: User | null;
    isLoading: boolean;
    isProvisioned: boolean;
    hasCompletedOnboarding: boolean;
    error: string | null;
    refetchUser: () => Promise<void>;
    updateUser: (updates: {
        name?: string;
        hasCompletedOnboarding?: boolean;
    }) => Promise<void>;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export function UserProvider({ children }: { children: ReactNode }) {
    const { isAuthenticated, isLoading: isAuthLoading, isReady } = useAuth();
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [hasProvisioned, setHasProvisioned] = useState(false);

    const provisionUser = useCallback(async () => {
        if (!isReady || hasProvisioned) return;

        setIsLoading(true);
        setError(null);

        try {
            // Create/ensure user exists in backend
            console.log("Making createUser API call...");
            const createdUser = await api.createUser();
            console.log("User provisioned:", createdUser);
            setUser(createdUser);
            setHasProvisioned(true);
        } catch (err) {
            console.error("Failed to provision user:", err);
            setError(
                err instanceof Error ? err.message : "Failed to provision user"
            );
        } finally {
            setIsLoading(false);
        }
    }, [isReady, hasProvisioned]);

    const refetchUser = useCallback(async () => {
        if (!isReady) return;

        setIsLoading(true);
        setError(null);

        try {
            const userData = await api.getUser();
            setUser(userData);
        } catch (err) {
            console.error("Failed to fetch user:", err);
            setError(
                err instanceof Error ? err.message : "Failed to fetch user"
            );
        } finally {
            setIsLoading(false);
        }
    }, [isReady]);

    // Provision user when auth is fully ready
    useEffect(() => {
        if (isReady && !hasProvisioned) {
            console.log("Auth is ready, provisioning user...");
            provisionUser();
        }
    }, [isReady, hasProvisioned, provisionUser]);

    // Reset state on logout
    useEffect(() => {
        if (!isAuthenticated && !isAuthLoading) {
            setUser(null);
            setHasProvisioned(false);
            setError(null);
        }
    }, [isAuthenticated, isAuthLoading]);

    const updateUser = async (updates: {
        name?: string;
        hasCompletedOnboarding?: boolean;
    }) => {
        try {
            const updatedUser = await api.updateUser(updates);
            setUser(updatedUser);
        } catch (err) {
            console.error("Failed to update user:", err);
            throw err;
        }
    };

    const value = useMemo(
        () => ({
            user,
            isLoading:
                isLoading ||
                isAuthLoading ||
                (isAuthenticated && !hasProvisioned),
            isProvisioned: hasProvisioned,
            hasCompletedOnboarding: user?.hasCompletedOnboarding ?? false,
            error,
            refetchUser,
            updateUser,
        }),
        [
            user,
            isLoading,
            isAuthLoading,
            isAuthenticated,
            hasProvisioned,
            error,
            refetchUser,
        ]
    );

    return (
        <UserContext.Provider value={value}>{children}</UserContext.Provider>
    );
}

export function useUser() {
    const context = useContext(UserContext);
    if (context === undefined) {
        throw new Error("useUser must be used within a UserProvider");
    }
    return context;
}
