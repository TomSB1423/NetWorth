import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { TrendingUp, TrendingDown, RefreshCw } from "lucide-react";
import { Card, CardContent } from "../../components/ui/card";
import { NetWorthChart } from "../../components/dashboard/NetWorthChart";
import { AssetAllocationChart } from "../../components/dashboard/AssetAllocationChart";
import { TopAccounts } from "../../components/dashboard/TopAccounts";
import { GoalsSection } from "../../components/dashboard/GoalsSection";
import { useAccounts } from "../../contexts/AccountContext";
import { Account, AccountBalances, Balance } from "../../types";

// Metric Card Component
function MetricCard({
    label,
    value,
    change,
    changeType,
    accentColor,
}: {
    label: string;
    value: string;
    change: string;
    changeType: "positive" | "negative" | "neutral";
    accentColor: "blue" | "green" | "red" | "purple";
}) {
    const gradients = {
        blue: "from-blue-500/20 to-blue-500/5",
        green: "from-emerald-500/20 to-emerald-500/5",
        red: "from-red-500/20 to-red-500/5",
        purple: "from-purple-500/20 to-purple-500/5",
    };

    const accentColors = {
        blue: "text-blue-400",
        green: "text-emerald-400",
        red: "text-red-400",
        purple: "text-purple-400",
    };

    return (
        <Card className="relative overflow-hidden">
            <div
                className={`absolute inset-0 bg-gradient-to-br ${gradients[accentColor]} opacity-50`}
            />
            <CardContent className="p-4 relative z-10">
                <div className="flex items-center justify-between mb-2">
                    <p className="text-[10px] font-semibold text-gray-400 tracking-wider uppercase">
                        {label}
                    </p>
                    {changeType !== "neutral" && (
                        <div
                            className={
                                changeType === "positive"
                                    ? "text-emerald-400"
                                    : "text-red-400"
                            }
                        >
                            {changeType === "positive" ? (
                                <TrendingUp size={18} />
                            ) : (
                                <TrendingDown size={18} />
                            )}
                        </div>
                    )}
                </div>
                <h3
                    className={`text-xl font-bold mb-1 ${accentColors[accentColor]} tracking-tight`}
                >
                    {value}
                </h3>
                <div className="flex items-center gap-1.5">
                    <span
                        className={`text-sm font-medium ${
                            changeType === "positive"
                                ? "text-emerald-400"
                                : changeType === "negative"
                                ? "text-red-400"
                                : "text-gray-400"
                        }`}
                    >
                        {change}
                    </span>
                </div>
            </CardContent>
        </Card>
    );
}

export default function Overview() {
    const navigate = useNavigate();
    const { accounts, balances, isLoading } = useAccounts();

    // Determine if we're waiting for data to sync
    const isSyncing = useMemo(() => {
        const hasAnyBalance = balances.some((b) => b.balances.length > 0);
        if (hasAnyBalance) return false;
        return (
            accounts.length > 0 && accounts.every((a: Account) => !a.lastSynced)
        );
    }, [accounts, balances]);

    const metrics = useMemo(() => {
        let totalAssets = 0;
        let totalLiabilities = 0;
        let currency = "GBP";

        balances.forEach((accountBalance: AccountBalances) => {
            const balanceList = accountBalance.balances || [];
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

        return { netWorth, totalAssets, totalLiabilities, currency };
    }, [balances]);

    const formatCurrency = (value: number, currency: string) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
        }).format(value);
    };

    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-[400px] text-white">
                Loading...
            </div>
        );
    }

    return (
        <div className="space-y-5">
            {/* Syncing Banner */}
            {isSyncing && (
                <div className="bg-gradient-to-r from-blue-600 to-emerald-600 text-white py-2 px-3 rounded-lg">
                    <div className="flex items-center justify-center gap-3">
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

            {/* Page Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold tracking-tight text-white">
                        Financial Overview
                    </h1>
                    <p className="text-gray-400 text-sm">
                        Real-time snapshot of your wealth
                    </p>
                </div>
            </div>

            {/* Charts Row */}
            <div className="grid lg:grid-cols-2 gap-5">
                <NetWorthChart hasAccounts={accounts.length > 0} />
                <AssetAllocationChart />
            </div>

            {/* Metric Cards */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                <MetricCard
                    label="NET WORTH"
                    value={
                        isSyncing
                            ? "Loading..."
                            : formatCurrency(metrics.netWorth, metrics.currency)
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
                                  Math.abs(metrics.totalLiabilities),
                                  metrics.currency
                              )
                    }
                    change="-0.0% MTD"
                    changeType="neutral"
                    accentColor="red"
                />
                <MetricCard
                    label="CASH FLOW"
                    value="--"
                    change="To be implemented"
                    changeType="neutral"
                    accentColor="purple"
                />
            </div>

            {/* Accounts & Goals Row */}
            <div className="grid lg:grid-cols-2 gap-5">
                <TopAccounts
                    onViewAll={() => navigate("/dashboard/accounts")}
                    onAddAccount={() => navigate("/select-bank")}
                />
                <GoalsSection />
            </div>
        </div>
    );
}
