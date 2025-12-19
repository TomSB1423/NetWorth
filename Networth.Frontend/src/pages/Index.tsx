import { useMemo, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { Plus, LogOut, LineChart, RefreshCw } from "lucide-react";
import { MetricCard } from "../components/dashboard/MetricCard";
import { NetWorthChart } from "../components/dashboard/NetWorthChart";
import { AssetAllocationChart } from "../components/dashboard/AssetAllocationChart";
import { TopAccounts } from "../components/dashboard/TopAccounts";
import { GoalsSection } from "../components/dashboard/GoalsSection";
import { FinancialHealthMetrics } from "../components/dashboard/FinancialHealthMetrics";
import { PerformanceMetrics } from "../components/dashboard/PerformanceMetrics";
import { useAccounts } from "../contexts/AccountContext";
import { useAuth } from "../contexts/AuthContext";
import { Account, AccountBalances, Balance } from "../types";

export default function Index() {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { accounts, balances, isLoading } = useAccounts();
    const { logout } = useAuth();
    const pollIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

    // Determine if we're waiting for data to sync
    const isSyncing = useMemo(() => {
        const hasAnyBalance = balances.some((b) => b.balances.length > 0);
        if (hasAnyBalance) return false;

        return (
            accounts.length > 0 && accounts.every((a: Account) => !a.lastSynced)
        );
    }, [accounts, balances]);

    // Poll for data when syncing
    useEffect(() => {
        if (isSyncing && !pollIntervalRef.current) {
            pollIntervalRef.current = setInterval(async () => {
                console.log("Polling for balance updates...");
                await queryClient.invalidateQueries({ queryKey: ["accounts"] });
                await queryClient.invalidateQueries({ queryKey: ["balances"] });
            }, 15000); // Poll every 15 seconds
        } else if (!isSyncing && pollIntervalRef.current) {
            clearInterval(pollIntervalRef.current);
            pollIntervalRef.current = null;
        }

        return () => {
            if (pollIntervalRef.current) {
                clearInterval(pollIntervalRef.current);
                pollIntervalRef.current = null;
            }
        };
    }, [isSyncing, queryClient]);

    const metrics = useMemo(() => {
        let totalAssets = 0;
        let totalLiabilities = 0;
        let currency = "GBP";

        balances.forEach((accountBalance: AccountBalances) => {
            const balanceList = accountBalance.balances || [];
            // Prefer 'interimAvailable' or first available
            const balanceObj =
                balanceList.find(
                    (b: Balance) => b.balanceType === "interimAvailable"
                ) ??
                balanceList[0] ??
                null;

            if (balanceObj) {
                const amount = parseFloat(balanceObj.amount);
                if (amount >= 0) {
                    totalAssets += amount;
                } else {
                    totalLiabilities += amount;
                }
                if (balanceObj.currency) {
                    currency = balanceObj.currency;
                }
            }
        });

        const netWorth = totalAssets + totalLiabilities;

        return {
            netWorth,
            totalAssets,
            totalLiabilities,
            currency,
        };
    }, [balances]);

    const formatCurrency = (value: number, currency: string) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
        }).format(value);
    };

    if (isLoading) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center text-white">
                Loading...
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-b from-slate-950 to-slate-900">
            {/* Syncing Banner */}
            {isSyncing && (
                <div className="bg-gradient-to-r from-blue-600 to-emerald-600 text-white py-3 px-4">
                    <div className="max-w-7xl mx-auto flex items-center justify-center gap-3">
                        <RefreshCw className="w-5 h-5 animate-spin" />
                        <span className="font-medium">
                            Syncing your accounts...
                        </span>
                        <span className="text-blue-100">
                            This may take a few moments. We'll refresh
                            automatically.
                        </span>
                    </div>
                </div>
            )}

            {/* Header */}
            <header className="border-b border-slate-800 sticky top-0 z-50 bg-slate-950/95 backdrop-blur">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-emerald-500 via-blue-500 to-emerald-500 flex items-center justify-center">
                            <LineChart size={18} className="text-white" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-white">
                                NetWorth
                            </h1>
                            <p className="text-xs text-gray-400">
                                Real-time portfolio overview
                            </p>
                        </div>
                    </div>
                    <div className="flex items-center gap-3">
                        <button
                            onClick={() => navigate("/select-bank")}
                            className="flex items-center gap-2 px-4 py-2 rounded-lg bg-emerald-500 hover:bg-emerald-600 text-white transition-colors"
                        >
                            <Plus size={18} />
                            <span className="text-sm font-medium">
                                Add Account
                            </span>
                        </button>
                        <button
                            onClick={() => logout()}
                            className="flex items-center gap-2 px-4 py-2 rounded-lg border border-slate-700 text-gray-300 hover:text-white hover:border-slate-600 transition-colors"
                        >
                            <LogOut size={18} />
                            <span className="text-sm">Sign Out</span>
                        </button>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {/* Top Metrics */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
                    <MetricCard
                        label="NET WORTH"
                        value={
                            isSyncing
                                ? "Loading..."
                                : formatCurrency(
                                      metrics.netWorth,
                                      metrics.currency
                                  )
                        }
                        change="+$0.00 MTD"
                        changeType="neutral"
                        accentColor="blue"
                    />
                    <MetricCard
                        label="TOTAL ASSETS"
                        value={
                            isSyncing
                                ? "Loading..."
                                : formatCurrency(
                                      metrics.totalAssets,
                                      metrics.currency
                                  )
                        }
                        change="+0.0% QTD"
                        changeType="neutral"
                        accentColor="green"
                    />
                    <MetricCard
                        label="LIABILITIES"
                        value={
                            isSyncing
                                ? "Loading..."
                                : formatCurrency(
                                      metrics.totalLiabilities,
                                      metrics.currency
                                  )
                        }
                        change="-0.0% MTD"
                        changeType="neutral"
                        accentColor="red"
                    />
                    <MetricCard
                        label="CASH FLOW"
                        value={
                            isSyncing
                                ? "Loading..."
                                : formatCurrency(0, metrics.currency)
                        }
                        change="Monthly surplus"
                        changeType="neutral"
                        accentColor="orange"
                    />
                </div>

                {/* Main Grid */}
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 mb-8">
                    {/* Left Column - Charts */}
                    <div className="lg:col-span-2 space-y-8">
                        {/* Net Worth Trend */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <div className="flex items-center justify-between mb-6">
                                <div>
                                    <h3 className="text-lg font-semibold text-white">
                                        Net Worth Trend
                                    </h3>
                                </div>
                            </div>
                            <NetWorthChart
                                isSyncing={isSyncing}
                                currency={metrics.currency}
                            />
                        </div>

                        {/* Top Accounts */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <TopAccounts isSyncing={isSyncing} />
                        </div>

                        {/* Performance Metrics */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <PerformanceMetrics />
                        </div>
                    </div>

                    {/* Right Column - Side Panels */}
                    <div className="space-y-8">
                        {/* Asset Allocation */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <h3 className="text-lg font-semibold text-white mb-6">
                                Asset Allocation
                            </h3>
                            <AssetAllocationChart isSyncing={isSyncing} />
                        </div>

                        {/* Goals */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <GoalsSection />
                        </div>

                        {/* Financial Health */}
                        <div className="bg-slate-800/40 rounded-lg border border-slate-700/50 p-6 backdrop-blur-sm">
                            <FinancialHealthMetrics />
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
}
