import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
    ArrowUpRight,
    ArrowDownLeft,
    Tag,
    TrendingUp,
    TrendingDown,
    Clock,
} from "lucide-react";
import { api } from "../services/api";
import { Transaction } from "../types";
import { useAccounts } from "../contexts/AccountContext";
import { Button } from "../components/ui/button";

type TransactionType = "all" | "income" | "expense";

export default function Transactions() {
    const { accounts } = useAccounts();

    // Filter state
    const [selectedType, setSelectedType] = useState<TransactionType>("all");

    // Fetch transactions for all accounts
    const {
        data: transactions = [],
        isLoading,
        error,
    } = useQuery({
        queryKey: ["transactions", accounts.map((a) => a.id).join(",")],
        queryFn: async () => {
            if (accounts.length === 0) return [];

            const promises = accounts.map((account) =>
                api
                    .getTransactions(account.id, 1, 100)
                    .then((res) => res.items)
                    .catch(() => [])
            );

            const results = await Promise.all(promises);
            const allTransactions = results.flat();

            // Sort by date descending
            return allTransactions.sort(
                (a, b) =>
                    new Date(b.bookingDate ?? 0).getTime() -
                    new Date(a.bookingDate ?? 0).getTime()
            );
        },
        enabled: accounts.length > 0,
    });

    // Calculate most recent sync time
    const lastSynced = useMemo(() => {
        if (accounts.length === 0) return undefined;
        const dates = accounts
            .map((a) => (a.lastSynced ? new Date(a.lastSynced).getTime() : 0))
            .filter((d) => d > 0);
        if (dates.length === 0) return undefined;
        return new Date(Math.max(...dates)).toISOString();
    }, [accounts]);

    // Filter transactions based on selections
    const filteredTransactions = useMemo(() => {
        return transactions.filter((tx) => {
            const amount = parseFloat(tx.amount);
            if (selectedType === "income" && amount <= 0) return false;
            if (selectedType === "expense" && amount >= 0) return false;
            return true;
        });
    }, [transactions, selectedType]);

    // Calculate monthly data for the rolling year chart
    const monthlyChartData = useMemo(() => {
        const months: {
            month: string;
            monthKey: string;
            income: number;
            expense: number;
        }[] = [];
        const now = new Date();

        // Generate last 12 months
        for (let i = 11; i >= 0; i--) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            const monthKey = `${date.getFullYear()}-${String(
                date.getMonth() + 1
            ).padStart(2, "0")}`;
            const monthName = date.toLocaleDateString("en-GB", {
                month: "short",
            });
            months.push({ month: monthName, monthKey, income: 0, expense: 0 });
        }

        // Aggregate transactions by month
        transactions.forEach((tx) => {
            if (!tx.bookingDate) return;
            const txDate = new Date(tx.bookingDate);
            const txKey = `${txDate.getFullYear()}-${String(
                txDate.getMonth() + 1
            ).padStart(2, "0")}`;
            const monthData = months.find((m) => m.monthKey === txKey);
            if (monthData) {
                const amount = parseFloat(tx.amount);
                if (amount > 0) {
                    monthData.income += amount;
                } else {
                    monthData.expense += Math.abs(amount);
                }
            }
        });

        return months;
    }, [transactions]);

    // Calculate income/expense totals
    const totals = useMemo(() => {
        let income = 0;
        let expense = 0;
        filteredTransactions.forEach((tx) => {
            const amount = parseFloat(tx.amount);
            if (amount > 0) income += amount;
            else expense += Math.abs(amount);
        });
        return { income, expense, net: income - expense };
    }, [filteredTransactions]);

    // Get unique categories from transactions
    const categories = useMemo(() => {
        const cats = new Set<string>();
        transactions.forEach((tx) => {
            const desc =
                tx.remittanceInformationUnstructured ??
                tx.creditorName ??
                tx.debtorName ??
                "";
            // Extract potential category from description (simplified)
            if (desc) cats.add(desc.split(" ")[0]);
        });
        return Array.from(cats).slice(0, 10);
    }, [transactions]);

    const formatCurrency = (amount: number, currency: string = "GBP") => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
        }).format(amount);
    };

    const formatDate = (dateString: string) => {
        return new Date(dateString).toLocaleDateString("en-GB", {
            day: "numeric",
            month: "short",
            year: "numeric",
        });
    };

    const formatLastSynced = (dateString?: string): string => {
        if (!dateString) return "Never";
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffMins = Math.floor(diffMs / 60000);
        const diffHours = Math.floor(diffMs / 3600000);
        const diffDays = Math.floor(diffMs / 86400000);

        if (diffMins < 1) return "Just now";
        if (diffMins < 60) return `${diffMins}m ago`;
        if (diffHours < 24) return `${diffHours}h ago`;
        return `${diffDays}d ago`;
    };

    const getTransactionDescription = (tx: Transaction): string => {
        return (
            tx.remittanceInformationUnstructured ??
            tx.creditorName ??
            tx.debtorName ??
            "Transaction"
        );
    };

    const maxChartValue = Math.max(
        ...monthlyChartData.flatMap((d) => [d.income, d.expense]),
        1
    );

    return (
        <div className="space-y-6">
            {/* Page Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-white">
                        Transactions
                    </h1>
                    <div className="flex items-center gap-2 mt-1 text-sm text-gray-400">
                        <Clock size={14} />
                        <span>Last synced: {formatLastSynced(lastSynced)}</span>
                        <span className="text-gray-600">•</span>
                        <span>{filteredTransactions.length} transactions</span>
                    </div>
                </div>
            </div>

            {/* Rolling Year Chart */}
            <div className="p-6 rounded-xl bg-slate-900/50 border border-slate-800">
                <h3 className="text-sm font-medium text-gray-400 mb-4">
                    Income vs Expenses (Rolling Year)
                </h3>
                <div className="h-48 flex items-end gap-4">
                    {monthlyChartData.map((data) => {
                        const incomeHeight =
                            maxChartValue > 0
                                ? (data.income / maxChartValue) * 100
                                : 0;
                        const expenseHeight =
                            maxChartValue > 0
                                ? (data.expense / maxChartValue) * 100
                                : 0;

                        return (
                            <div
                                key={data.monthKey}
                                className="flex-1 flex flex-col items-center gap-2"
                            >
                                <div
                                    className="w-full flex gap-1 items-end"
                                    style={{ height: "160px" }}
                                >
                                    <div
                                        className="flex-1 bg-emerald-500/80 rounded-t transition-all hover:bg-emerald-500"
                                        style={{
                                            height: `${incomeHeight}%`,
                                            minHeight:
                                                data.income > 0 ? "4px" : "0",
                                        }}
                                        title={`Income: ${formatCurrency(
                                            data.income
                                        )}`}
                                    />
                                    <div
                                        className="flex-1 bg-red-500/80 rounded-t transition-all hover:bg-red-500"
                                        style={{
                                            height: `${expenseHeight}%`,
                                            minHeight:
                                                data.expense > 0 ? "4px" : "0",
                                        }}
                                        title={`Expenses: ${formatCurrency(
                                            data.expense
                                        )}`}
                                    />
                                </div>
                                <span className="text-xs text-gray-500">
                                    {data.month}
                                </span>
                            </div>
                        );
                    })}
                </div>
                <div className="flex items-center justify-center gap-6 mt-4">
                    <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded bg-emerald-500" />
                        <span className="text-xs text-gray-400">Income</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded bg-red-500" />
                        <span className="text-xs text-gray-400">Expenses</span>
                    </div>
                </div>
            </div>

            {/* Filters Row */}
            <div className="flex flex-wrap items-center gap-4 p-4 rounded-xl bg-slate-900/50 border border-slate-800">
                {/* Type Filter */}
                <div className="flex items-center gap-2">
                    <span className="text-sm text-gray-400">Type:</span>
                    <div className="flex gap-1">
                        <Button
                            variant={
                                selectedType === "all" ? "default" : "ghost"
                            }
                            size="sm"
                            onClick={() => setSelectedType("all")}
                            className={
                                selectedType === "all"
                                    ? "bg-emerald-500/20 text-emerald-400 hover:bg-emerald-500/30"
                                    : "text-gray-300"
                            }
                        >
                            All
                        </Button>
                        <Button
                            variant={
                                selectedType === "income" ? "default" : "ghost"
                            }
                            size="sm"
                            onClick={() => setSelectedType("income")}
                            className={
                                selectedType === "income"
                                    ? "bg-green-500/20 text-green-400 hover:bg-green-500/30"
                                    : "text-gray-300"
                            }
                        >
                            <TrendingUp size={14} className="mr-1" />
                            Income
                        </Button>
                        <Button
                            variant={
                                selectedType === "expense" ? "default" : "ghost"
                            }
                            size="sm"
                            onClick={() => setSelectedType("expense")}
                            className={
                                selectedType === "expense"
                                    ? "bg-red-500/20 text-red-400 hover:bg-red-500/30"
                                    : "text-gray-300"
                            }
                        >
                            <TrendingDown size={14} className="mr-1" />
                            Expenses
                        </Button>
                    </div>
                </div>

                {categories.length > 0 && (
                    <>
                        <div className="w-px h-6 bg-slate-700" />

                        {/* Categories */}
                        <div className="flex items-center gap-2">
                            <Tag size={16} className="text-gray-400" />
                            <span className="text-sm text-gray-400">
                                Categories:
                            </span>
                            <div className="flex gap-1 flex-wrap">
                                {categories.slice(0, 5).map((cat) => (
                                    <span
                                        key={cat}
                                        className="px-2 py-1 text-xs rounded-full bg-slate-800 text-gray-300"
                                    >
                                        {cat}
                                    </span>
                                ))}
                                {categories.length > 5 && (
                                    <span className="px-2 py-1 text-xs rounded-full bg-slate-800 text-gray-400">
                                        +{categories.length - 5} more
                                    </span>
                                )}
                            </div>
                        </div>
                    </>
                )}
            </div>

            {/* Summary Stats */}
            <div className="flex items-center gap-4">
                <div className="flex items-center gap-2 px-4 py-2 rounded-lg bg-green-500/10 border border-green-500/20">
                    <TrendingUp size={16} className="text-green-400" />
                    <span className="text-sm font-medium text-green-400">
                        {formatCurrency(totals.income)}
                    </span>
                </div>
                <div className="flex items-center gap-2 px-4 py-2 rounded-lg bg-red-500/10 border border-red-500/20">
                    <TrendingDown size={16} className="text-red-400" />
                    <span className="text-sm font-medium text-red-400">
                        {formatCurrency(totals.expense)}
                    </span>
                </div>
                <div
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg ${
                        totals.net >= 0
                            ? "bg-emerald-500/10 border border-emerald-500/20"
                            : "bg-orange-500/10 border border-orange-500/20"
                    }`}
                >
                    <span
                        className={`text-sm font-medium ${
                            totals.net >= 0
                                ? "text-emerald-400"
                                : "text-orange-400"
                        }`}
                    >
                        Net: {formatCurrency(totals.net)}
                    </span>
                </div>
            </div>

            {/* Transaction List */}
            <div className="space-y-2">
                {isLoading ? (
                    <div className="text-center text-gray-400 py-12">
                        Loading transactions...
                    </div>
                ) : error ? (
                    <div className="text-center text-red-400 py-12">
                        Failed to load transactions.
                    </div>
                ) : accounts.length === 0 ? (
                    <div className="text-center text-gray-500 py-12">
                        No accounts found. Link an account to view transactions.
                    </div>
                ) : filteredTransactions.length === 0 ? (
                    <div className="text-center text-gray-500 py-12">
                        No transactions found.
                    </div>
                ) : (
                    filteredTransactions.map((tx: Transaction) => (
                        <div
                            key={tx.transactionId}
                            className="bg-slate-900/50 rounded-xl p-4 flex items-center justify-between hover:bg-slate-900 transition-colors border border-slate-800/50"
                        >
                            <div className="flex items-center gap-4">
                                <div
                                    className={`p-2 rounded-full ${
                                        parseFloat(tx.amount) > 0
                                            ? "bg-green-500/10 text-green-500"
                                            : "bg-white/5 text-white"
                                    }`}
                                >
                                    {parseFloat(tx.amount) > 0 ? (
                                        <ArrowDownLeft size={20} />
                                    ) : (
                                        <ArrowUpRight size={20} />
                                    )}
                                </div>
                                <div>
                                    <div className="font-medium text-white">
                                        {getTransactionDescription(tx)}
                                    </div>
                                    <div className="text-sm text-gray-400">
                                        {tx.bookingDate
                                            ? formatDate(tx.bookingDate)
                                            : "Pending"}
                                    </div>
                                </div>
                            </div>
                            <div
                                className={`font-semibold ${
                                    parseFloat(tx.amount) > 0
                                        ? "text-green-400"
                                        : "text-white"
                                }`}
                            >
                                {formatCurrency(
                                    parseFloat(tx.amount),
                                    tx.currency
                                )}
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}
