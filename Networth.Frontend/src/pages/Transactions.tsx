import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import {
    ArrowUpRight,
    ArrowDownLeft,
    TrendingUp,
    TrendingDown,
    Clock,
    ChevronLeft,
    ChevronRight,
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
    const [page, setPage] = useState(1);
    const pageSize = 20;

    // Fetch transactions for all accounts
    const { data, isLoading, error } = useQuery({
        queryKey: ["transactions", accounts.map((a) => a.id).join(","), page],
        queryFn: async () => {
            if (accounts.length === 0) return { items: [], totalPages: 0 };

            const promises = accounts.map((account) =>
                api
                    .getTransactions(account.id, page, pageSize)
                    .catch(() => ({
                        items: [],
                        totalPages: 0,
                        totalCount: 0,
                        page: 1,
                        pageSize,
                        hasNextPage: false,
                        hasPreviousPage: false,
                    })),
            );

            const results = await Promise.all(promises);
            const allTransactions = results.flatMap((r) => r.items);
            const maxPages = Math.max(...results.map((r) => r.totalPages));

            // Sort by date descending
            const sortedTransactions = allTransactions.sort(
                (a, b) =>
                    new Date(b.bookingDate ?? 0).getTime() -
                    new Date(a.bookingDate ?? 0).getTime(),
            );

            return { items: sortedTransactions, totalPages: maxPages };
        },
        enabled: accounts.length > 0,
    });

    const transactions = useMemo(() => data?.items ?? [], [data?.items]);
    const totalPages = data?.totalPages ?? 0;

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
                date.getMonth() + 1,
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
                txDate.getMonth() + 1,
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

    const getAccountName = (accountId: string) => {
        const account = accounts.find((a) => a.id === accountId);
        return account?.displayName || account?.name || "Unknown Account";
    };

    const maxChartValue = Math.max(
        ...monthlyChartData.flatMap((d) => [d.income, d.expense]),
        1,
    );

    return (
        <div className="space-y-5">
            {/* Page Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-white">
                        Transactions
                    </h1>
                    <div className="flex items-center gap-2 mt-0.5 text-sm text-gray-400">
                        <Clock size={14} />
                        <span>Last synced: {formatLastSynced(lastSynced)}</span>
                        <span className="text-gray-600">•</span>
                        <span>{filteredTransactions.length} transactions</span>
                    </div>
                </div>
            </div>

            {/* Rolling Year Chart */}
            <div className="p-4 rounded-lg bg-slate-900/50 border border-slate-800">
                <h3 className="text-sm font-medium text-gray-400 mb-3">
                    Income vs Expenses (Rolling Year)
                </h3>
                <div className="h-40 flex items-end gap-3">
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
                                className="flex-1 flex flex-col items-center gap-1.5"
                            >
                                <div
                                    className="w-full flex gap-1 items-end"
                                    style={{ height: "130px" }}
                                >
                                    <div
                                        className="flex-1 bg-emerald-500/80 rounded-t transition-all hover:bg-emerald-500"
                                        style={{
                                            height: `${incomeHeight}%`,
                                            minHeight:
                                                data.income > 0 ? "4px" : "0",
                                        }}
                                        title={`Income: ${formatCurrency(
                                            data.income,
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
                                            data.expense,
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
            <div className="border border-slate-800 rounded-lg overflow-hidden bg-slate-900/20">
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
                            className="flex items-center justify-between p-3 hover:bg-slate-800/30 transition-colors border-b border-slate-800 last:border-0"
                        >
                            <div className="flex items-center gap-3">
                                <div
                                    className={`p-1.5 rounded-full ${
                                        parseFloat(tx.amount) > 0
                                            ? "bg-green-500/10 text-green-500"
                                            : "bg-white/5 text-white"
                                    }`}
                                >
                                    {parseFloat(tx.amount) > 0 ? (
                                        <ArrowDownLeft size={16} />
                                    ) : (
                                        <ArrowUpRight size={16} />
                                    )}
                                </div>
                                <div>
                                    <div className="font-medium text-white text-sm">
                                        {getTransactionDescription(tx)}
                                    </div>
                                    <div className="flex items-center text-xs text-gray-400">
                                        <span>
                                            {tx.bookingDate
                                                ? formatDate(tx.bookingDate)
                                                : "Pending"}
                                        </span>
                                        <span className="mx-1.5 text-slate-600">
                                            •
                                        </span>
                                        <span className="text-slate-500">
                                            {getAccountName(tx.accountId)}
                                        </span>
                                    </div>
                                </div>
                            </div>
                            <div
                                className={`font-semibold text-sm ${
                                    parseFloat(tx.amount) > 0
                                        ? "text-green-400"
                                        : "text-white"
                                }`}
                            >
                                {formatCurrency(
                                    parseFloat(tx.amount),
                                    tx.currency,
                                )}
                            </div>
                        </div>
                    ))
                )}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
                <div className="flex items-center justify-center gap-2 py-4">
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setPage((p) => Math.max(1, p - 1))}
                        disabled={page === 1 || isLoading}
                        className="h-8 w-8 p-0"
                    >
                        <ChevronLeft size={16} />
                    </Button>
                    <span className="text-sm text-gray-400">
                        Page {page} of {totalPages}
                    </span>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() =>
                            setPage((p) => Math.min(totalPages, p + 1))
                        }
                        disabled={page === totalPages || isLoading}
                        className="h-8 w-8 p-0"
                    >
                        <ChevronRight size={16} />
                    </Button>
                </div>
            )}
        </div>
    );
}
