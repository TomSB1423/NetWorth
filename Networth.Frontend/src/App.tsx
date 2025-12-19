import { useEffect, useState } from "react";
import {
    BrowserRouter,
    Routes,
    Route,
    Navigate,
    useSearchParams,
    useNavigate,
} from "react-router-dom";
import {
    QueryClient,
    QueryClientProvider,
    useQueryClient,
} from "@tanstack/react-query";
import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { msalConfig } from "./config/authConfig";
import { AuthProvider, useAuth } from "./contexts/AuthContext";
import { UserProvider, useUser } from "./contexts/UserContext";
import { AccountProvider, useAccounts } from "./contexts/AccountContext";
import { api } from "./services/api";

import Landing from "./pages/Landing";
import OnboardingTutorial from "./pages/onboarding/OnboardingTutorial";
import SelectBank from "./pages/onboarding/SelectBank";
import NameAccount from "./pages/onboarding/NameAccount";
import Transactions from "./pages/Transactions";
import NotFound from "./pages/NotFound";
import { LoadingScreen } from "./components/ui/LoadingScreen";

// New Dashboard Pages
import { DashboardLayout } from "./components/layout/DashboardLayout";
import Overview from "./pages/dashboard/Overview";
import AccountsPage from "./pages/dashboard/Accounts";

// Create MSAL instance
const msalInstance = new PublicClientApplication(msalConfig);

const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            retry: 1,
            refetchOnWindowFocus: false,
        },
    },
});

// Landing page wrapper with navigation
function LandingWrapper() {
    const navigate = useNavigate();
    const { login } = useAuth();

    const handleGetStarted = async () => {
        try {
            await login();
            navigate("/dashboard");
        } catch (error) {
            console.error("Login error:", error);
        }
    };

    const handleSignIn = async () => {
        try {
            await login();
            navigate("/dashboard");
        } catch (error) {
            console.error("Login error:", error);
        }
    };

    return <Landing onGetStarted={handleGetStarted} onSignIn={handleSignIn} />;
}

// Protected route wrapper
function ProtectedRoute({ children }: { children: React.ReactNode }) {
    const { isAuthenticated, isLoading } = useAuth();

    if (isLoading) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center text-white">
                Checking authentication...
            </div>
        );
    }

    if (!isAuthenticated) {
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
}

function AppRoutes() {
    const { hasAccounts, isLoading } = useAccounts();
    const { isAuthenticated, isLoading: authLoading, isReady } = useAuth();
    const { isProvisioned, hasCompletedOnboarding, updateUser, user } =
        useUser();
    const [searchParams, setSearchParams] = useSearchParams();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const institutionId = searchParams.get("institutionId");
    const [isSyncing, setIsSyncing] = useState(false);

    // Wait for user provisioning before showing main app
    const isAppLoading =
        !isProvisioned || isLoading || institutionId || isSyncing;
    const [showLoading, setShowLoading] = useState(true);

    // When we enter a loading state, ensure the loading screen is visible.
    // When loading completes, the LoadingScreen itself will call onComplete
    // to hide, so we don't toggle showLoading off here to avoid flicker.
    useEffect(() => {
        if (isAppLoading) {
            setShowLoading(true);
        }
    }, [isAppLoading]);

    // Wait for auth to be ready AND user to be provisioned before syncing
    const canSync = isReady && isProvisioned;

    // Handle institution callback - sync and complete onboarding
    useEffect(() => {
        const handleCallback = async () => {
            if (institutionId && canSync && !isSyncing) {
                setIsSyncing(true);
                console.log("Syncing institution:", institutionId);
                try {
                    await api.syncInstitution(institutionId);

                    // Poll for accounts to be created (sync happens asynchronously in queue)
                    const maxAttempts = 30;
                    const pollInterval = 1000; // 1 second
                    let accountsCreated = false;

                    for (let attempt = 0; attempt < maxAttempts; attempt++) {
                        await queryClient.invalidateQueries({
                            queryKey: ["accounts"],
                        });

                        // Wait for the query to refetch and check if accounts exist
                        const accounts = await api.getAccounts();
                        if (accounts.length > 0) {
                            accountsCreated = true;
                            console.log(
                                `Accounts found after ${attempt + 1} attempt(s)`
                            );
                            break;
                        }

                        console.log(
                            `Waiting for accounts... attempt ${
                                attempt + 1
                            }/${maxAttempts}`
                        );
                        await new Promise((resolve) =>
                            setTimeout(resolve, pollInterval)
                        );
                    }

                    if (!accountsCreated) {
                        console.warn(
                            "Accounts not created within timeout, proceeding anyway"
                        );
                    }

                    // Final invalidation to ensure fresh data
                    await queryClient.invalidateQueries({
                        queryKey: ["accounts"],
                    });
                    await queryClient.invalidateQueries({
                        queryKey: ["balances"],
                    });
                    console.log("Institution sync completed");

                    // Mark onboarding as complete if not already
                    if (!hasCompletedOnboarding) {
                        await updateUser({ hasCompletedOnboarding: true });
                        console.log("Onboarding marked as complete");
                    }

                    // Remove the institutionId from the URL and navigate to dashboard
                    const newParams = new URLSearchParams(searchParams);
                    newParams.delete("institutionId");
                    setSearchParams(newParams, { replace: true });
                    navigate("/dashboard", { replace: true });
                } catch (error) {
                    console.error("Failed to sync institution:", error);
                    // Still navigate to dashboard on error
                    const newParams = new URLSearchParams(searchParams);
                    newParams.delete("institutionId");
                    setSearchParams(newParams, { replace: true });
                    navigate("/dashboard", { replace: true });
                } finally {
                    setIsSyncing(false);
                }
            }
        };
        handleCallback();
    }, [
        institutionId,
        canSync,
        isSyncing,
        queryClient,
        searchParams,
        setSearchParams,
        hasCompletedOnboarding,
        updateUser,
        navigate,
    ]);

    // Show loading while checking auth status
    if (authLoading) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center text-white">
                Checking authentication...
            </div>
        );
    }

    // Landing page for unauthenticated users
    if (!isAuthenticated) {
        return (
            <Routes>
                <Route path="/" element={<LandingWrapper />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        );
    }

    if (showLoading) {
        const message = !isProvisioned
            ? "Setting up your account..."
            : institutionId || isSyncing
            ? "Connecting to your bank..."
            : undefined;

        return (
            <LoadingScreen
                message={message}
                userName={user?.name}
                isFinished={!isAppLoading}
                onComplete={() => setShowLoading(false)}
            />
        );
    }

    // Determine where to route the user
    // 1. If not completed onboarding and no accounts -> onboarding tutorial
    // 2. If has accounts -> dashboard
    // 3. Otherwise -> onboarding tutorial

    return (
        <Routes>
            <Route path="/" element={<Navigate to="/dashboard" replace />} />

            {/* Dashboard with nested routes */}
            <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        {hasAccounts || hasCompletedOnboarding ? (
                            <DashboardLayout />
                        ) : (
                            <Navigate to="/onboarding" replace />
                        )}
                    </ProtectedRoute>
                }
            >
                <Route index element={<Overview />} />
                <Route path="transactions" element={<Transactions />} />
                <Route path="accounts" element={<AccountsPage />} />
            </Route>

            <Route
                path="/onboarding"
                element={
                    <ProtectedRoute>
                        {hasCompletedOnboarding ? (
                            <Navigate to="/dashboard" replace />
                        ) : (
                            <OnboardingTutorial />
                        )}
                    </ProtectedRoute>
                }
            />
            <Route
                path="/select-bank"
                element={
                    <ProtectedRoute>
                        <SelectBank />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/name-account"
                element={
                    <ProtectedRoute>
                        <NameAccount />
                    </ProtectedRoute>
                }
            />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}

function App() {
    return (
        <MsalProvider instance={msalInstance}>
            <AuthProvider>
                <QueryClientProvider client={queryClient}>
                    <UserProvider>
                        <AccountProvider>
                            <BrowserRouter>
                                <AppRoutes />
                            </BrowserRouter>
                        </AccountProvider>
                    </UserProvider>
                </QueryClientProvider>
            </AuthProvider>
        </MsalProvider>
    );
}

export default App;
