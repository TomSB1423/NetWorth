import { useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Plus, TrendingUp, TrendingDown, RefreshCw } from "lucide-react";
import { Card, CardContent } from "../../components/ui/card";
import { Button } from "../../components/ui/button";
import { NetWorthChart } from "../../components/dashboard/NetWorthChart";
import { AssetAllocationChart } from "../../components/dashboard/AssetAllocationChart";
import { TopAccounts } from "../../components/dashboard/TopAccounts";
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
            <CardContent className="p-6 relative z-10">
                <div className="flex items-center justify-between mb-3">
                    <p className="text-[11px] font-bold text-gray-400 tracking-widest uppercase">
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
                    className={`text-3xl font-black mb-2 ${accentColors[accentColor]} tracking-tight`}
                >
                    {value}
                </h3>
                <div className="flex items-center gap-2">
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
        <div className="space-y-8">
            {/* Syncing Banner */}
            {isSyncing && (
                <div className="bg-gradient-to-r from-blue-600 to-emerald-600 text-white py-3 px-4 rounded-xl">
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
                    <h1 className="text-4xl font-black tracking-tight text-white mb-2">
                        Financial Overview
                    </h1>
                    <p className="text-gray-400 text-lg font-medium">
                        Real-time snapshot of your wealth
                    </p>
                </div>
                <Button
                    onClick={() => navigate("/select-bank")}
                    className="h-12 px-6"
                >
                    <Plus size={20} className="mr-2" />
                    Add Account
                </Button>
            </div>

            {/* Metric Cards */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-5">
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

            {/* Net Worth Chart */}
            <NetWorthChart hasAccounts={accounts.length > 0} />

            {/* Two Column Layout */}
            <div className="grid lg:grid-cols-2 gap-8">
                <TopAccounts
                    onViewAll={() => navigate("/dashboard/accounts")}
                />
                <AssetAllocationChart />
            </div>
        </div>
    );
}
