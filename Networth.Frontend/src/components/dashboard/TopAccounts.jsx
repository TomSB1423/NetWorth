import React from "react";
import { useNavigate } from "react-router-dom";
import { useAccounts } from "../../contexts/AccountContext";
import { ArrowUpRight, ArrowDownRight } from "lucide-react";

export function TopAccounts() {
    const { accounts, balances } = useAccounts();
    const navigate = useNavigate();

    // Helper to get balance for an account
    const getAccountBalance = (accountId) => {
        const accountBalances =
            balances.find((b) => b.accountId === accountId)?.balances || [];
        // Prefer 'interimAvailable' or first available
        const balance =
            accountBalances.find((b) => b.balanceType === "interimAvailable") ||
            accountBalances[0];
        return balance;
    };

    return (
        <div>
            <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-semibold text-white">
                    Top Accounts
                </h3>
                <button className="text-sm text-blue-400 hover:text-blue-300 transition-colors">
                    View All
                </button>
            </div>
            <div className="space-y-4">
                {accounts.slice(0, 5).map((account) => {
                    const balance = getAccountBalance(account.id);
                    const amount = balance ? parseFloat(balance.amount) : 0;
                    const currency = balance ? balance.currency : "GBP";

                    // Dummy change data for now
                    const isPositive = Math.random() > 0.5;
                    const change = (Math.random() * 5).toFixed(1);

                    return (
                        <div
                            key={account.id}
                            onClick={() => navigate(`/accounts/${account.id}/transactions`)}
                            className="flex items-center justify-between p-4 rounded-lg bg-slate-700/30 border border-slate-700/50 hover:bg-slate-700/50 transition-colors cursor-pointer"
                        >
                            <div className="flex items-center gap-4">
                                <div className="w-10 h-10 rounded-full bg-slate-600 flex items-center justify-center text-white font-bold">
                                    {account.name
                                        ? account.name.charAt(0).toUpperCase()
                                        : "A"}
                                </div>
                                <div>
                                    <div className="font-medium text-white">
                                        {account.name || "Unnamed Account"}
                                    </div>
                                    <div className="text-sm text-gray-400">
                                        {account.institutionId || "Bank"}
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
                                <div
                                    className={`text-xs flex items-center justify-end gap-1 ${
                                        isPositive
                                            ? "text-green-400"
                                            : "text-red-400"
                                    }`}
                                >
                                    {isPositive ? (
                                        <ArrowUpRight size={12} />
                                    ) : (
                                        <ArrowDownRight size={12} />
                                    )}
                                    {change}%
                                </div>
                            </div>
                        </div>
                    );
                })}
                {accounts.length === 0 && (
                    <div className="text-gray-400 text-center py-4">
                        No accounts linked yet.
                    </div>
                )}
            </div>
        </div>
    );
}
