import {
    PieChart,
    Pie,
    Cell,
    ResponsiveContainer,
    Legend,
    Tooltip,
} from "recharts";
import { Loader2 } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { useAccounts } from "../../contexts/AccountContext";
import { useMemo, useState } from "react";

const ASSET_COLORS = ["#10B981", "#3B82F6", "#8B5CF6", "#F59E0B", "#EC4899", "#6366F1"];

interface AssetAllocationChartProps {
    isSyncing?: boolean;
}

export function AssetAllocationChart({ isSyncing }: AssetAllocationChartProps) {
    const { accounts, balances, isLoading } = useAccounts();
    const [groupBy, setGroupBy] = useState<"type" | "account">("type");

    // Group accounts by type or name and calculate totals (including liabilities)
    const chartData = useMemo(() => {
        const totals: Record<string, { value: number; isLiability: boolean }> = {};

        accounts.forEach((account) => {
            const accountBalance = balances.find(
                (b) => b.accountId === account.id
            );
            if (accountBalance && accountBalance.balances.length > 0) {
                const balance =
                    accountBalance.balances.find(
                        (b) => b.balanceType === "interimAvailable"
                    ) ?? accountBalance.balances[0];
                const amount = parseFloat(balance.amount);
                
                if (amount !== 0) {
                    let key = "Unknown";
                    const isLiability = amount < 0;
                    
                    if (groupBy === "type") {
                        // Capitalize first letter of product/type
                        const type = account.product || "Other";
                        key = type.charAt(0).toUpperCase() + type.slice(1);
                    } else {
                        key = account.displayName || account.name || "Unnamed Account";
                    }
                    
                    if (!totals[key]) {
                        totals[key] = { value: 0, isLiability };
                    }
                    totals[key].value += Math.abs(amount);
                }
            }
        });

        return Object.entries(totals)
            .map(([name, data]) => ({
                name: name,
                value: data.value,
                isLiability: data.isLiability,
            }))
            .sort((a, b) => b.value - a.value);
    }, [accounts, balances, groupBy]);

    if (isLoading || isSyncing) {
        return (
            <Card>
                <CardContent className="h-[300px] flex items-center justify-center text-gray-400">
                    <Loader2 className="w-8 h-8 animate-spin" />
                    <span className="ml-2">Loading data...</span>
                </CardContent>
            </Card>
        );
    }

    const hasData = chartData.length > 0;

    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle>Asset Allocation</CardTitle>
                <div className="flex bg-slate-800/50 rounded-lg p-1">
                    <button
                        onClick={() => setGroupBy("type")}
                        className={`px-3 py-1 text-xs font-medium rounded-md transition-all ${
                            groupBy === "type"
                                ? "bg-slate-600 text-white shadow-sm"
                                : "text-slate-400 hover:text-slate-200"
                        }`}
                    >
                        Type
                    </button>
                    <button
                        onClick={() => setGroupBy("account")}
                        className={`px-3 py-1 text-xs font-medium rounded-md transition-all ${
                            groupBy === "account"
                                ? "bg-slate-600 text-white shadow-sm"
                                : "text-slate-400 hover:text-slate-200"
                        }`}
                    >
                        Account
                    </button>
                </div>
            </CardHeader>
            <CardContent>
                <div className="h-[280px] w-full relative">
                    {hasData ? (
                        <ResponsiveContainer width="100%" height="100%">
                            <PieChart>
                                <Pie
                                    data={chartData}
                                    cx="50%"
                                    cy="50%"
                                    innerRadius={60}
                                    outerRadius={90}
                                    fill="#8884d8"
                                    paddingAngle={3}
                                    dataKey="value"
                                >
                                    {chartData.map((entry, index) => (
                                        <Cell
                                            key={`cell-${index}`}
                                            fill={ASSET_COLORS[index % ASSET_COLORS.length]}
                                        />
                                    ))}
                                </Pie>
                                <Tooltip
                                    contentStyle={{
                                        backgroundColor: "#1E293B",
                                        borderColor: "#334155",
                                        color: "#F1F5F9",
                                        borderRadius: "8px",
                                    }}
                                    itemStyle={{ color: "#F1F5F9" }}
                                    formatter={(value: number, name: string, item: { payload: { isLiability: boolean } }) => {
                                        const isLiability = item.payload.isLiability;
                                        const formatted = new Intl.NumberFormat("en-GB", {
                                            style: "currency",
                                            currency: "GBP",
                                        }).format(value);
                                        return [isLiability ? `-${formatted}` : formatted, name];
                                    }}
                                />
                                <Legend wrapperStyle={{ color: "#94A3B8" }} />
                            </PieChart>
                        </ResponsiveContainer>
                    ) : (
                        <div className="h-full flex items-center justify-center">
                            <p className="text-gray-400">
                                No data to display
                            </p>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
