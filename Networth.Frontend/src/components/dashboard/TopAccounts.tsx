import { useNavigate } from "react-router-dom";
import { useAccounts } from "../../contexts/AccountContext";
import { Loader2 } from "lucide-react";
import { Balance } from "../../types";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { Button } from "../ui/button";

interface TopAccountsProps {
    onViewAll?: () => void;
    isSyncing?: boolean;
}

export function TopAccounts({ onViewAll, isSyncing }: TopAccountsProps) {
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
                <CardTitle>Top Accounts</CardTitle>
                {onViewAll && (
                    <Button
                        variant="link"
                        onClick={onViewAll}
                        className="text-emerald-400"
                    >
                        View All
                    </Button>
                )}
            </CardHeader>
            <CardContent>
                <div className="space-y-4">
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
                                className="flex items-center justify-between p-4 rounded-lg bg-slate-700/30 border border-slate-700/50 hover:bg-slate-700/50 transition-colors cursor-pointer"
                            >
                                <div className="flex items-center gap-4">
                                    {account.institutionLogo ? (
                                        <img
                                            src={account.institutionLogo}
                                            alt={
                                                account.institutionName ||
                                                "Bank"
                                            }
                                            className="w-10 h-10 rounded-full object-contain bg-white p-1"
                                        />
                                    ) : (
                                        <div className="w-10 h-10 rounded-full bg-slate-600 flex items-center justify-center text-white font-bold">
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
                                        <div className="font-medium text-white">
                                            {getDisplayName(account)}
                                        </div>
                                        <div className="text-sm text-gray-400 flex items-center gap-2">
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
                                    <div className="font-bold text-white">
                                        {new Intl.NumberFormat("en-GB", {
                                            style: "currency",
                                            currency: currency,
                                        }).format(amount)}
                                    </div>
                                    {account.lastSynced && (
                                        <div className="text-xs text-slate-500 mt-1">
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
                        <div className="text-gray-400 text-center py-8">
                            <p className="text-lg">Please add an account</p>
                            <p className="text-sm mt-2">
                                Link a bank account to see your accounts here
                            </p>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
