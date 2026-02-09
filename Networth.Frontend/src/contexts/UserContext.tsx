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
    
    // Initialize from session storage to prevent loading flash on reload
    const [hasProvisioned, setHasProvisioned] = useState(() => {
        return sessionStorage.getItem("user_provisioned") === "true";
    });
    
    const [cachedOnboardingStatus, setCachedOnboardingStatus] = useState<boolean>(() => {
        return sessionStorage.getItem("user_onboarding_complete") === "true";
    });

    const provisionUser = useCallback(async () => {
        if (!isReady || (hasProvisioned && user)) return;

        setIsLoading(true);
        setError(null);

        try {
            // Create/ensure user exists in backend
            const createdUser = await api.createUser();
            setUser(createdUser);
            setHasProvisioned(true);
            sessionStorage.setItem("user_provisioned", "true");
            
            if (createdUser.hasCompletedOnboarding) {
                setCachedOnboardingStatus(true);
                sessionStorage.setItem("user_onboarding_complete", "true");
            }
        } catch (err) {
            console.error("Failed to provision user:", err);
            setError(
                err instanceof Error ? err.message : "Failed to provision user"
            );
        } finally {
            setIsLoading(false);
        }
    }, [isReady, hasProvisioned, user]);

    const refetchUser = useCallback(async () => {
        if (!isReady) return;

        setIsLoading(true);
        setError(null);

        try {
            const userData = await api.getUser();
            setUser(userData);
            if (userData.hasCompletedOnboarding) {
                setCachedOnboardingStatus(true);
                sessionStorage.setItem("user_onboarding_complete", "true");
            }
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
        if (isReady) {
            if (!hasProvisioned) {
                provisionUser();
            } else if (!user) {
                // If we have provisioned flag but no user data (e.g. reload), fetch user
                refetchUser();
            }
        }
    }, [isReady, hasProvisioned, user, provisionUser, refetchUser]);

    // Reset state on logout
    useEffect(() => {
        if (!isAuthenticated && !isAuthLoading) {
            setUser(null);
            setHasProvisioned(false);
            setCachedOnboardingStatus(false);
            setError(null);
            sessionStorage.removeItem("user_provisioned");
            sessionStorage.removeItem("user_onboarding_complete");
        }
    }, [isAuthenticated, isAuthLoading]);

    const updateUser = async (updates: {
        name?: string;
        hasCompletedOnboarding?: boolean;
    }) => {
        try {
            const updatedUser = await api.updateUser(updates);
            setUser(updatedUser);
            if (updatedUser.hasCompletedOnboarding) {
                setCachedOnboardingStatus(true);
                sessionStorage.setItem("user_onboarding_complete", "true");
            }
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
            hasCompletedOnboarding: user?.hasCompletedOnboarding ?? cachedOnboardingStatus,
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
            cachedOnboardingStatus,
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
