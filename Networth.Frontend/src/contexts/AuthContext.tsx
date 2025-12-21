import {
    createContext,
    useContext,
    useCallback,
    useMemo,
    useEffect,
    useState,
    ReactNode,
} from "react";
import {
    User,
    onAuthStateChanged,
    signInWithPopup,
    signOut,
    GoogleAuthProvider,
} from "firebase/auth";
import { auth } from "../config/firebaseConfig";
import { setTokenGetter } from "../services/api";
import { config } from "../config/config";

// User info type that matches what we need for display
export interface FirebaseUserInfo {
    uid: string;
    email: string | null;
    displayName: string | null;
    photoURL: string | null;
}

interface AuthContextType {
    isAuthenticated: boolean;
    isLoading: boolean;
    isReady: boolean; // True when auth is complete AND token getter is set
    user: FirebaseUserInfo | null;
    login: () => Promise<void>;
    logout: () => Promise<void>;
    getAccessToken: () => Promise<string>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Google Auth Provider - configured once
const googleProvider = new GoogleAuthProvider();
googleProvider.addScope("email");
googleProvider.addScope("profile");

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isTokenGetterConfigured, setIsTokenGetterConfigured] =
        useState(false);

    // Listen to auth state changes
    useEffect(() => {
        if (!auth) {
            setIsLoading(false);
            return;
        }

        const unsubscribe = onAuthStateChanged(
            auth,
            (firebaseUser: User | null) => {
                setUser(firebaseUser);
                setIsLoading(false);

                if (firebaseUser) {
                    // Set up token getter for API calls
                    const tokenGetter = async () => {
                        const token = await firebaseUser.getIdToken();
                        return token;
                    };
                    setTokenGetter(tokenGetter);
                    setIsTokenGetterConfigured(true);
                } else {
                    setIsTokenGetterConfigured(false);
                }
            }
        );

        return () => unsubscribe();
    }, []);

    const login = useCallback(async () => {
        if (!auth) {
            throw new Error("Firebase auth not initialized");
        }
        try {
            await signInWithPopup(auth, googleProvider);
        } catch (error) {
            console.error("Login failed:", error);
            throw error;
        }
    }, []);

    const logout = useCallback(async () => {
        if (!auth) {
            throw new Error("Firebase auth not initialized");
        }
        try {
            // Clear session storage flags
            sessionStorage.removeItem("welcome_shown");
            await signOut(auth);
        } catch (error) {
            console.error("Logout failed:", error);
            throw error;
        }
    }, []);

    const getAccessToken = useCallback(async (): Promise<string> => {
        if (!user) {
            throw new Error("No authenticated user");
        }
        try {
            return await user.getIdToken();
        } catch (error) {
            console.error("Failed to get token:", error);
            throw error;
        }
    }, [user]);

    const isAuthenticated = user !== null;

    // Derive isReady from state
    const isReady = isAuthenticated && isTokenGetterConfigured;

    // Convert Firebase User to our UserInfo type
    const userInfo: FirebaseUserInfo | null = user
        ? {
              uid: user.uid,
              email: user.email,
              displayName: user.displayName,
              photoURL: user.photoURL,
          }
        : null;

    const value = useMemo(
        () => ({
            isAuthenticated,
            isLoading,
            isReady,
            user: userInfo,
            login,
            logout,
            getAccessToken,
        }),
        [
            isAuthenticated,
            isLoading,
            isReady,
            userInfo,
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
    // In mock mode, use the MockAuthContext
    const mockContext = useContext(MockAuthContext);
    const realContext = useContext(AuthContext);

    // If we're in mock mode and have a mock context, use it
    if (config.useMockData && mockContext !== undefined) {
        return mockContext;
    }

    // Otherwise use the real auth context
    if (realContext === undefined) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return realContext;
}

// Export the context for MockAuthContext to use
export { AuthContext };

// Import MockAuthContext at the end to avoid circular dependency
import { MockAuthContext } from "./MockAuthContext";
