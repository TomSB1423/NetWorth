import { useState, useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from "recharts";
import { api } from "../../services/api";
import { Loader2 } from "lucide-react";
import { NetWorthDataPoint } from "../../types";

const PERIODS = [
    { label: "1M", days: 30 },
    { label: "3M", days: 90 },
    { label: "6M", days: 180 },
    { label: "1Y", days: 365 },
    { label: "All", days: null },
];

interface NetWorthChartProps {
    isSyncing: boolean;
    currency?: string;
}

export function NetWorthChart({
    isSyncing,
    currency = "GBP",
}: NetWorthChartProps) {
    const [selectedPeriod, setSelectedPeriod] = useState("1Y");

    const {
        data: history = [],
        isLoading,
        error,
    } = useQuery<NetWorthDataPoint[]>({
        queryKey: ["netWorthHistory"],
        queryFn: api.getNetWorthHistory,
    });

    const data = useMemo(() => {
        return [...history].sort(
            (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
        );
    }, [history]);

    const filteredData = useMemo(() => {
        if (selectedPeriod === "All") return data;

        const period = PERIODS.find((p) => p.label === selectedPeriod);
        if (!period || !period.days) return data;

        const cutoffDate = new Date();
        cutoffDate.setDate(cutoffDate.getDate() - period.days);

        return data.filter((d) => new Date(d.date) >= cutoffDate);
    }, [data, selectedPeriod]);

    const formatXAxis = (tickItem: string) => {
        const date = new Date(tickItem);
        return date.toLocaleDateString("en-GB", {
            month: "short",
            day: "numeric",
        });
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
            notation: "compact",
        }).format(value);
    };

    if (isLoading || isSyncing) {
        return (
            <div className="h-[300px] w-full flex items-center justify-center text-gray-400">
                <Loader2 className="w-8 h-8 animate-spin" />
                <span className="ml-2">Loading data...</span>
            </div>
        );
    }

    if (error) {
        return (
            <div className="h-[300px] w-full flex items-center justify-center text-red-500">
                {(error as Error).message || "Failed to load data"}
            </div>
        );
    }

    return (
        <div className="w-full space-y-4">
            <div className="flex justify-end space-x-2">
                {PERIODS.map((period) => (
                    <button
                        key={period.label}
                        onClick={() => setSelectedPeriod(period.label)}
                        className={`px-3 py-1 text-sm rounded-md transition-colors ${
                            selectedPeriod === period.label
                                ? "bg-blue-600 text-white"
                                : "bg-gray-700 text-gray-300 hover:bg-gray-600"
                        }`}
                    >
                        {period.label}
                    </button>
                ))}
            </div>

            <div className="h-[300px] w-full">
                <ResponsiveContainer width="100%" height="100%">
                    <AreaChart
                        data={filteredData}
                        margin={{
                            top: 10,
                            right: 30,
                            left: 0,
                            bottom: 0,
                        }}
                    >
                        <defs>
                            <linearGradient
                                id="colorValue"
                                x1="0"
                                y1="0"
                                x2="0"
                                y2="1"
                            >
                                <stop
                                    offset="5%"
                                    stopColor="#3b82f6"
                                    stopOpacity={0.3}
                                />
                                <stop
                                    offset="95%"
                                    stopColor="#3b82f6"
                                    stopOpacity={0}
                                />
                            </linearGradient>
                        </defs>
                        <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#374151"
                            vertical={false}
                        />
                        <XAxis
                            dataKey="date"
                            stroke="#9CA3AF"
                            tickFormatter={formatXAxis}
                            minTickGap={30}
                        />
                        <YAxis
                            stroke="#9CA3AF"
                            tickFormatter={formatCurrency}
                        />
                        <Tooltip
                            contentStyle={{
                                backgroundColor: "#1F2937",
                                borderColor: "#374151",
                                color: "#F3F4F6",
                            }}
                            itemStyle={{ color: "#F3F4F6" }}
                            labelFormatter={(label) =>
                                new Date(label).toLocaleDateString("en-GB", {
                                    dateStyle: "medium",
                                })
                            }
                            formatter={(value: number) => [
                                new Intl.NumberFormat("en-GB", {
                                    style: "currency",
                                    currency: currency,
                                }).format(value),
                                "Net Worth",
                            ]}
                        />
                        <Area
                            type="monotone"
                            dataKey="amount"
                            stroke="#3b82f6"
                            fillOpacity={1}
                            fill="url(#colorValue)"
                        />
                    </AreaChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}
