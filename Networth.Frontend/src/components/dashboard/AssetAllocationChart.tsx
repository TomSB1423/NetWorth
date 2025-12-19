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
import { useMemo } from "react";

const COLORS = ["#10B981", "#3B82F6", "#8B5CF6", "#F59E0B", "#EF4444"];

interface AssetAllocationChartProps {
    isSyncing?: boolean;
}

export function AssetAllocationChart({ isSyncing }: AssetAllocationChartProps) {
    const { accounts, balances, isLoading } = useAccounts();

    // Group accounts by institution and calculate totals
    const chartData = useMemo(() => {
        const institutionTotals: Record<string, number> = {};

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
                if (amount > 0) {
                    const institution = account.institutionId || "Unknown";
                    institutionTotals[institution] =
                        (institutionTotals[institution] || 0) + amount;
                }
            }
        });

        return Object.entries(institutionTotals).map(([name, value]) => ({
            name,
            value,
        }));
    }, [accounts, balances]);

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
            <CardHeader>
                <CardTitle>Asset Allocation</CardTitle>
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
                                    {chartData.map((_, index) => (
                                        <Cell
                                            key={`cell-${index}`}
                                            fill={COLORS[index % COLORS.length]}
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
                                    formatter={(value: number) =>
                                        new Intl.NumberFormat("en-GB", {
                                            style: "currency",
                                            currency: "GBP",
                                        }).format(value)
                                    }
                                />
                                <Legend
                                    wrapperStyle={{ color: "#94A3B8" }}
                                    formatter={(value) => (
                                        <span className="text-gray-300">
                                            {value}
                                        </span>
                                    )}
                                />
                            </PieChart>
                        </ResponsiveContainer>
                    ) : (
                        <div className="h-full flex items-center justify-center">
                            <p className="text-gray-400">
                                No assets to display
                            </p>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    );
}
