import { useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { LogOut, ChevronLeft, Save, Loader2 } from "lucide-react";
import { useAuth } from "../../contexts/AuthContext";
import { useAccounts } from "../../contexts/AccountContext";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "../../services/api";
import { AccountCategory } from "../../types";

const ACCOUNT_CATEGORIES: {
    value: AccountCategory;
    label: string;
    description: string;
}[] = [
    {
        value: "Spending",
        label: "Spending",
        description: "Everyday transactions and bills",
    },
    {
        value: "Savings",
        label: "Savings",
        description: "Money set aside for the future",
    },
    {
        value: "Investment",
        label: "Investment",
        description: "Stocks, funds, and other investments",
    },
];

export default function NameAccount() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const accountId = searchParams.get("accountId");
    const { logout } = useAuth();
    const { accounts, isLoading: isLoadingAccounts } = useAccounts();
    const queryClient = useQueryClient();

    const account = accounts.find((a) => a.id === accountId);

    const [displayName, setDisplayName] = useState(account?.displayName || "");
    const [category, setCategory] = useState<AccountCategory | "">(
        account?.category || ""
    );

    const updateAccountMutation = useMutation({
        mutationFn: (data: {
            displayName?: string;
            category?: AccountCategory;
        }) => api.updateAccount(accountId!, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["accounts"] });
            navigate("/dashboard");
        },
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!accountId) return;

        updateAccountMutation.mutate({
            ...(displayName && { displayName }),
            ...(category && { category }),
        });
    };

    if (!accountId) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center">
                <div className="text-center space-y-4">
                    <h1 className="text-2xl font-bold text-white">
                        No Account Selected
                    </h1>
                    <p className="text-gray-400">
                        Please select an account to customize.
                    </p>
                    <button
                        onClick={() => navigate("/dashboard")}
                        className="text-emerald-400 hover:text-emerald-300"
                    >
                        Go back to dashboard
                    </button>
                </div>
            </div>
        );
    }

    if (isLoadingAccounts) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center">
                <Loader2 className="w-8 h-8 text-emerald-500 animate-spin" />
            </div>
        );
    }

    if (!account) {
        return (
            <div className="min-h-screen bg-slate-950 flex items-center justify-center">
                <div className="text-center space-y-4">
                    <h1 className="text-2xl font-bold text-white">
                        Account Not Found
                    </h1>
                    <p className="text-gray-400">
                        The account you're looking for doesn't exist.
                    </p>
                    <button
                        onClick={() => navigate("/dashboard")}
                        className="text-emerald-400 hover:text-emerald-300"
                    >
                        Go back to dashboard
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-950">
            {/* Header */}
            <header className="border-b border-slate-800 bg-slate-950/95 backdrop-blur sticky top-0 z-10">
                <div className="max-w-2xl mx-auto px-4 py-4 flex items-center justify-between">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => navigate(-1)}
                            className="p-2 hover:bg-slate-800 rounded-full text-gray-400 hover:text-white transition-colors"
                        >
                            <ChevronLeft size={24} />
                        </button>
                        <div className="flex items-center gap-2">
                            <img src="/networth-icon.svg" alt="NetWorth" className="w-7 h-7" />
                            <h1 className="text-lg font-semibold text-white">
                                Customize Account
                            </h1>
                        </div>
                    </div>
                    <button
                        onClick={async () => {
                            await logout();
                            navigate("/", { replace: true });
                        }}
                        className="flex items-center gap-2 px-3 py-1.5 rounded-lg border border-slate-700 text-gray-300 hover:text-white hover:border-slate-600 transition-colors"
                    >
                        <LogOut size={16} />
                        <span className="text-sm">Sign Out</span>
                    </button>
                </div>
            </header>

            <div className="max-w-2xl mx-auto px-4 py-8">
                <form onSubmit={handleSubmit} className="space-y-8">
                    {/* Account Info */}
                    <div className="bg-slate-900 rounded-xl p-6 border border-slate-800">
                        <h2 className="text-sm font-medium text-gray-400 mb-2">
                            Original Account Name
                        </h2>
                        <p className="text-lg text-white">
                            {account.name || "Unknown Account"}
                        </p>
                    </div>

                    {/* Display Name */}
                    <div className="space-y-2">
                        <label
                            htmlFor="displayName"
                            className="block text-sm font-medium text-gray-300"
                        >
                            Display Name
                        </label>
                        <input
                            type="text"
                            id="displayName"
                            value={displayName}
                            onChange={(e) => setDisplayName(e.target.value)}
                            placeholder="Enter a custom name for this account"
                            className="w-full px-4 py-3 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent transition-all"
                        />
                        <p className="text-sm text-gray-500">
                            Give your account a friendly name to identify it
                            easily.
                        </p>
                    </div>

                    {/* Account Category */}
                    <div className="space-y-3">
                        <label className="block text-sm font-medium text-gray-300">
                            Account Category
                        </label>
                        <div className="grid gap-3">
                            {ACCOUNT_CATEGORIES.map((cat) => (
                                <label
                                    key={cat.value}
                                    className={`flex items-center gap-4 p-4 bg-slate-900 border rounded-xl cursor-pointer transition-all ${
                                        category === cat.value
                                            ? "border-emerald-500 ring-1 ring-emerald-500"
                                            : "border-slate-700 hover:border-slate-600"
                                    }`}
                                >
                                    <input
                                        type="radio"
                                        name="category"
                                        value={cat.value}
                                        checked={category === cat.value}
                                        onChange={(e) =>
                                            setCategory(
                                                e.target
                                                    .value as AccountCategory
                                            )
                                        }
                                        className="sr-only"
                                    />
                                    <div
                                        className={`w-5 h-5 rounded-full border-2 flex items-center justify-center ${
                                            category === cat.value
                                                ? "border-emerald-500 bg-emerald-500"
                                                : "border-slate-600"
                                        }`}
                                    >
                                        {category === cat.value && (
                                            <div className="w-2 h-2 rounded-full bg-white" />
                                        )}
                                    </div>
                                    <div>
                                        <p className="text-white font-medium">
                                            {cat.label}
                                        </p>
                                        <p className="text-sm text-gray-500">
                                            {cat.description}
                                        </p>
                                    </div>
                                </label>
                            ))}
                        </div>
                    </div>

                    {/* Error Message */}
                    {updateAccountMutation.error && (
                        <div className="p-4 bg-red-500/10 border border-red-500/20 rounded-xl">
                            <p className="text-red-400 text-sm">
                                Failed to update account. Please try again.
                            </p>
                        </div>
                    )}

                    {/* Submit Button */}
                    <button
                        type="submit"
                        disabled={updateAccountMutation.isPending}
                        className="w-full flex items-center justify-center gap-2 px-6 py-4 bg-gradient-to-r from-emerald-500 to-blue-500 hover:from-emerald-600 hover:to-blue-600 text-white font-semibold rounded-xl shadow-lg shadow-emerald-500/25 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {updateAccountMutation.isPending ? (
                            <>
                                <Loader2 size={20} className="animate-spin" />
                                Saving...
                            </>
                        ) : (
                            <>
                                <Save size={20} />
                                Save Changes
                            </>
                        )}
                    </button>
                </form>
            </div>
        </div>
    );
}
