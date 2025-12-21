import { useNavigate } from "react-router-dom";
import { useAccounts } from "../../contexts/AccountContext";
import { Loader2, Plus } from "lucide-react";
import { Balance } from "../../types";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { Button } from "../ui/button";

interface TopAccountsProps {
    onViewAll?: () => void;
    onAddAccount?: () => void;
    isSyncing?: boolean;
}

export function TopAccounts({ onViewAll, onAddAccount, isSyncing }: TopAccountsProps) {
    const { accounts, balances, isLoading } = useAccounts();
    const navigate = useNavigate();

    if (isLoading || isSyncing) {
        return (
            <Card>
                <CardContent className="h-[200px] flex items-center justify-center text-gray-400">
                    <Loader2 className="w-8 h-8 animate-spin" />
                    <span className="ml-2">Loading data...</span>
                </CardContent>
            </Card>
        );
    }

    // Helper to get balance for an account
    const getAccountBalance = (accountId: string): Balance | null => {
        const accountBalances =
            balances.find((b) => b.accountId === accountId)?.balances || [];
        // Prefer 'interimAvailable' or first available
        const balance =
            accountBalances.find((b) => b.balanceType === "interimAvailable") ||
            accountBalances[0];
        return balance || null;
    };

    // Get display name: prefer displayName, fallback to name
    const getDisplayName = (account: (typeof accounts)[0]): string => {
        return account.displayName || account.name || "Unnamed Account";
    };

    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>Accounts</CardTitle>
                <div className="flex items-center gap-2">
                    {onAddAccount && (
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={onAddAccount}
                            className="h-8 px-2 text-xs"
                        >
                            <Plus className="w-3 h-3 mr-1.5" />
                            Add
                        </Button>
                    )}
                    {onViewAll && (
                        <Button
                            variant="link"
                            onClick={onViewAll}
                            className="text-emerald-400 text-sm"
                        >
                            View All
                        </Button>
                    )}
                </div>
            </CardHeader>
            <CardContent>
                <div className="space-y-2">
                    {accounts.slice(0, 5).map((account) => {
                        const balance = getAccountBalance(account.id);
                        const amount = balance ? parseFloat(balance.amount) : 0;
                        const currency = balance ? balance.currency : "GBP";

                        return (
                            <div
                                key={account.id}
                                onClick={() =>
                                    navigate(`/dashboard/transactions`)
                                }
                                className="flex items-center justify-between p-3 rounded-lg bg-slate-700/30 border border-slate-700/50 hover:bg-slate-700/50 transition-colors cursor-pointer"
                            >
                                <div className="flex items-center gap-3">
                                    {account.institutionLogo ? (
                                        <img
                                            src={account.institutionLogo}
                                            alt={
                                                account.institutionName ||
                                                "Bank"
                                            }
                                            className="w-8 h-8 rounded-full object-contain bg-white p-0.5"
                                        />
                                    ) : (
                                        <div className="w-8 h-8 rounded-full bg-slate-600 flex items-center justify-center text-white font-bold text-sm">
                                            {(
                                                account.institutionName ||
                                                account.name ||
                                                "A"
                                            )
                                                .charAt(0)
                                                .toUpperCase()}
                                        </div>
                                    )}
                                    <div>
                                        <div className="font-medium text-white text-sm">
                                            {getDisplayName(account)}
                                        </div>
                                        <div className="text-xs text-gray-400 flex items-center gap-1.5">
                                            <span>
                                                {account.product ||
                                                    account.institutionName ||
                                                    "Account"}
                                            </span>
                                            {account.category && (
                                                <>
                                                    <span className="text-slate-600">
                                                        •
                                                    </span>
                                                    <span className="text-emerald-400">
                                                        {account.category}
                                                    </span>
                                                </>
                                            )}
                                        </div>
                                    </div>
                                </div>
                                <div className="text-right">
                                    <div className="font-semibold text-white text-sm">
                                        {new Intl.NumberFormat("en-GB", {
                                            style: "currency",
                                            currency: currency,
                                        }).format(amount)}
                                    </div>
                                    {account.lastSynced && (
                                        <div className="text-[11px] text-slate-500">
                                            Synced{" "}
                                            {new Date(
                                                account.lastSynced
                                            ).toLocaleDateString()}
                                        </div>
                                    )}
                                </div>
                            </div>
                        );
                    })}
                    {accounts.length === 0 && (
                        <div className="text-gray-400 text-center py-6">
                            <p className="text-sm">Please add an account</p>
                            <p className="text-xs mt-1">
                                Link a bank account to see your accounts here
                            </p>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
