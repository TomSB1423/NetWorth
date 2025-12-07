import { useEffect } from "react";
import {
    BrowserRouter,
    Routes,
    Route,
    Navigate,
    useSearchParams,
} from "react-router-dom";
import {
    QueryClient,
    QueryClientProvider,
    useQueryClient,
} from "@tanstack/react-query";
import { AccountProvider, useAccounts } from "./contexts/AccountContext";
import { api } from "./services/api";

import Index from "./pages/Index";
import Onboarding from "./pages/Onboarding";
import SelectBank from "./pages/SelectBank";
import NameAccount from "./pages/NameAccount";
import Transactions from "./pages/Transactions";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            retry: 1,
            refetchOnWindowFocus: false,
        },
    },
});

function AppRoutes() {
    const { hasAccounts, isLoading } = useAccounts();
    const [searchParams, setSearchParams] = useSearchParams();
    const queryClient = useQueryClient();
    const institutionId = searchParams.get("institutionId");

    useEffect(() => {
        const sync = async () => {
            if (institutionId) {
                try {
                    await api.syncInstitution(institutionId);
                    await queryClient.invalidateQueries({
                        queryKey: ["accounts"],
                    });
                    await queryClient.invalidateQueries({
                        queryKey: ["balances"],
                    });
                } catch (error) {

                    console.error("Failed to sync institution:", error);
                } finally {
                    // Remove the institutionId from the URL
                    const newParams = new URLSearchParams(searchParams);
                    newParams.delete("institutionId");
                    setSearchParams(newParams);
                }
            }
        };
        sync();
    }, [institutionId, queryClient, searchParams, setSearchParams]);

    if (isLoading || institutionId) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center text-white">
                Loading...
            </div>
        );
    }

    return (
        <Routes>
            <Route
                path="/"
                element={
                    hasAccounts ? (
                        <Index />
                    ) : (
                        <Navigate to="/onboarding" replace />
                    )
                }
            />
            <Route
                path="/onboarding"
                element={
                    hasAccounts ? <Navigate to="/" replace /> : <Onboarding />
                }
            />
            <Route path="/select-bank" element={<SelectBank />} />
            <Route path="/name-account" element={<NameAccount />} />
            <Route
                path="/accounts/:id/transactions"
                element={<Transactions />}
            />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}

function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <AccountProvider>
                <BrowserRouter>
                    <AppRoutes />
                </BrowserRouter>
            </AccountProvider>
        </QueryClientProvider>
    );
}

export default App;
