import { useNavigate } from "react-router-dom";
import { Plus, Trash2, Pencil } from "lucide-react";
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from "../../components/ui/card";
import { Button } from "../../components/ui/button";
import { Badge } from "../../components/ui/badge";
import { Avatar, AvatarFallback } from "../../components/ui/avatar";
import { useAccounts } from "../../contexts/AccountContext";
import { Account } from "../../types";

export default function AccountsPage() {
    const navigate = useNavigate();
    const { accounts, balances, isLoading } = useAccounts();

    const formatCurrency = (value: number, currency: string = "GBP") => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
        }).format(value);
    };

    const formatRelativeTime = (date: string | undefined) => {
        if (!date) return "Never synced";
        const now = new Date();
        const dateObj = new Date(date);
        const diff = now.getTime() - dateObj.getTime();
        const minutes = Math.floor(diff / 60000);

        if (minutes < 1) return "just now";
        if (minutes < 60) return `${minutes}m ago`;
        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `${hours}h ago`;
        const days = Math.floor(hours / 24);
        return `${days}d ago`;
    };

    const getAccountBalance = (
        accountId: string
    ): { amount: number; currency: string } => {
        const accountBalance = balances.find((b) => b.accountId === accountId);
        if (!accountBalance || accountBalance.balances.length === 0) {
            return { amount: 0, currency: "GBP" };
        }
        const balance =
            accountBalance.balances.find(
                (b) => b.balanceType === "interimAvailable"
            ) ?? accountBalance.balances[0];
        return {
            amount: parseFloat(balance.amount),
            currency: balance.currency,
        };
    };

    const getAccountCategory = (account: Account): string => {
        return account.category || "Not Set";
    };

    const handleEditAccount = (accountId: string) => {
        navigate(`/name-account?accountId=${accountId}`);
    };

    const handleRemoveAccount = (accountId: string) => {
        // TODO: To be implemented - Need backend endpoint for account removal
        console.log("Remove account:", accountId);
    };

    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-[400px] text-white">
                Loading...
            </div>
        );
    }

    if (accounts.length === 0) {
        return (
            <div className="space-y-5">
                {/* Page Header */}
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-bold tracking-tight text-white">
                            Accounts
                        </h1>
                        <p className="text-gray-400 text-sm">
                            Manage your connected financial accounts
                        </p>
                    </div>
                </div>

                <Card className="p-8">
                    <div className="text-center">
                        <h3 className="text-xl font-semibold text-white mb-2">
                            No Accounts Connected
                        </h3>
                        <p className="text-gray-400 mb-6">
                            Connect your first account to start tracking your
                            net worth
                        </p>
                        <Button onClick={() => navigate("/select-bank")}>
                            <Plus size={18} className="mr-2" />
                            Connect Your First Account
                        </Button>
                    </div>
                </Card>
            </div>
        );
    }

    return (
        <div className="space-y-5">
            {/* Page Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold tracking-tight text-white">
                        Accounts
                    </h1>
                    <p className="text-gray-400 text-sm">
                        Manage your connected financial accounts
                    </p>
                </div>
                <Button onClick={() => navigate("/select-bank")} size="sm">
                    <Plus size={16} className="mr-1.5" />
                    Add Account
                </Button>
            </div>

            {/* Account Summary Stats - Condensed inline */}
            <div className="flex items-center gap-6 text-sm">
                <div className="flex items-center gap-2">
                    <span className="text-gray-400">Accounts:</span>
                    <span className="font-semibold text-white">
                        {accounts.length}
                    </span>
                </div>
                <div className="h-4 w-px bg-slate-700" />
                <div className="flex items-center gap-2">
                    <span className="text-gray-400">Institutions:</span>
                    <span className="font-semibold text-white">
                        {new Set(accounts.map((a) => a.institutionId)).size}
                    </span>
                </div>
                <div className="h-4 w-px bg-slate-700" />
                <div className="flex items-center gap-2">
                    <span className="text-gray-400">Categories:</span>
                    <span className="font-semibold text-white">
                        {
                            new Set(
                                accounts.map((a) => a.category).filter(Boolean)
                            ).size
                        }
                    </span>
                </div>
            </div>

            {/* Accounts List */}
            <Card>
                <CardHeader>
                    <CardTitle>Connected Accounts</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b border-slate-700">
                                    <th className="text-left py-3 px-4 text-sm font-semibold text-gray-400">
                                        Institution
                                    </th>
                                    <th className="text-left py-3 px-4 text-sm font-semibold text-gray-400">
                                        Account Name
                                    </th>
                                    <th className="text-left py-3 px-4 text-sm font-semibold text-gray-400">
                                        Category
                                    </th>
                                    <th className="text-right py-3 px-4 text-sm font-semibold text-gray-400">
                                        Balance
                                    </th>
                                    <th className="text-left py-3 px-4 text-sm font-semibold text-gray-400">
                                        Last Updated
                                    </th>
                                    <th className="py-3 px-4 w-[100px]"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {accounts.map((account) => {
                                    const balance = getAccountBalance(
                                        account.id
                                    );
                                    const accountCategory =
                                        getAccountCategory(account);

                                    return (
                                        <tr
                                            key={account.id}
                                            className="border-b border-slate-800 hover:bg-slate-800/50 transition-colors"
                                        >
                                            <td className="py-4 px-4">
                                                <div className="flex items-center gap-3">
                                                    {account.institutionLogo ? (
                                                        <img
                                                            src={
                                                                account.institutionLogo
                                                            }
                                                            alt={
                                                                account.institutionName ||
                                                                "Bank"
                                                            }
                                                            className="h-10 w-10 rounded-full object-contain bg-white p-1"
                                                        />
                                                    ) : (
                                                        <Avatar className="h-10 w-10 bg-slate-700">
                                                            <AvatarFallback className="text-sm">
                                                                {(
                                                                    account.institutionName ||
                                                                    account.name ||
                                                                    "?"
                                                                )
                                                                    .charAt(0)
                                                                    .toUpperCase()}
                                                            </AvatarFallback>
                                                        </Avatar>
                                                    )}
                                                    <span className="font-medium text-white">
                                                        {account.institutionName ||
                                                            "Unknown Bank"}
                                                    </span>
                                                </div>
                                            </td>
                                            <td className="py-4 px-4">
                                                <div className="flex flex-col">
                                                    <span className="text-gray-300">
                                                        {account.displayName ||
                                                            account.name ||
                                                            `Account ${account.id.slice(
                                                                0,
                                                                8
                                                            )}`}
                                                    </span>
                                                    {account.product && (
                                                        <span className="text-xs text-gray-500">
                                                            {account.product}
                                                        </span>
                                                    )}
                                                </div>
                                            </td>
                                            <td className="py-4 px-4">
                                                <Badge variant="secondary">
                                                    {accountCategory}
                                                </Badge>
                                            </td>
                                            <td
                                                className={`py-4 px-4 text-right font-semibold ${
                                                    balance.amount < 0
                                                        ? "text-red-400"
                                                        : "text-emerald-400"
                                                }`}
                                            >
                                                {formatCurrency(
                                                    balance.amount,
                                                    balance.currency
                                                )}
                                            </td>
                                            <td className="py-4 px-4 text-gray-400 text-sm">
                                                {formatRelativeTime(
                                                    account.lastSynced
                                                )}
                                            </td>
                                            <td className="py-4 px-4">
                                                <div className="flex items-center gap-2 justify-end">
                                                    <Button
                                                        variant="ghost"
                                                        size="icon"
                                                        onClick={() =>
                                                            handleEditAccount(
                                                                account.id
                                                            )
                                                        }
                                                        title="Edit Account"
                                                    >
                                                        <Pencil size={16} />
                                                    </Button>
                                                    <Button
                                                        variant="ghost"
                                                        size="icon"
                                                        onClick={() =>
                                                            handleRemoveAccount(
                                                                account.id
                                                            )
                                                        }
                                                        title="Remove Account"
                                                        className="text-red-400 hover:text-red-300 hover:bg-red-500/10"
                                                    >
                                                        <Trash2 size={16} />
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
}
