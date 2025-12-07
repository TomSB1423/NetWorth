import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Plus } from "lucide-react";
import { MetricCard } from "../components/dashboard/MetricCard";
import { NetWorthChart } from "../components/dashboard/NetWorthChart";
import { AssetAllocationChart } from "../components/dashboard/AssetAllocationChart";
import { TopAccounts } from "../components/dashboard/TopAccounts";
import { GoalsSection } from "../components/dashboard/GoalsSection";
import { FinancialHealthMetrics } from "../components/dashboard/FinancialHealthMetrics";
import { PerformanceMetrics } from "../components/dashboard/PerformanceMetrics";
import { useAccounts } from "../contexts/AccountContext";
import { Account, AccountBalances, Balance } from "../types";

export default function Index() {
    const navigate = useNavigate();
    const { accounts, balances, isLoading } = useAccounts();

    const isSyncing = useMemo(() => {
        return accounts.some((a: Account) => !a.lastSynced);
    }, [accounts]);

    const metrics = useMemo(() => {
        let totalAssets = 0;
        let totalLiabilities = 0;

        balances.forEach((accountBalance: AccountBalances) => {
            const balanceList = accountBalance.balances || [];
            // Prefer 'interimAvailable' or first available
            const balanceObj =
                balanceList.find((b: Balance) => b.balanceType === "interimAvailable") ??
                balanceList[0] ??
                null;

            if (balanceObj) {
                const amount = parseFloat(balanceObj.amount);
                if (amount >= 0) {
                    totalAssets += amount;
                } else {
                    totalLiabilities += amount;
                }
            }
        });

        const netWorth = totalAssets + totalLiabilities;

        return {
            netWorth,
            totalAssets,
            totalLiabilities,
        };
    }, [balances]);

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: "GBP",
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
            {/* Header */}
            <header className="border-b border-slate-800 sticky top-0 z-50 bg-slate-950/95 backdrop-blur">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-bold text-white">
                            Financial Dashboard
                        </h1>
                        <p className="text-sm text-gray-400 mt-1">
                            Real-time portfolio overview
                        </p>
                    </div>
                    <button
                        onClick={() => navigate("/select-bank")}
                        className="flex items-center gap-2 px-4 py-2 rounded-lg border border-slate-700 text-gray-300 hover:text-white hover:border-slate-600 transition-colors"
                    >
                        <Plus size={18} />
                        <span className="text-sm">Add Account</span>
                    </button>
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
                                : formatCurrency(metrics.netWorth)
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
                                : formatCurrency(metrics.totalAssets)
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
                                : formatCurrency(metrics.totalLiabilities)
                        }
                        change="-0.0% MTD"
                        changeType="neutral"
                        accentColor="red"
                    />
                    <MetricCard
                        label="CASH FLOW"
                        value={isSyncing ? "Loading..." : formatCurrency(0)}
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
                            <NetWorthChart isSyncing={isSyncing} />
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
