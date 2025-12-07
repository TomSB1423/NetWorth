import React, { useState, useEffect, useMemo } from "react";
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

const PERIODS = [
    { label: "1M", days: 30 },
    { label: "3M", days: 90 },
    { label: "6M", days: 180 },
    { label: "1Y", days: 365 },
    { label: "All", days: null },
];

export function NetWorthChart({ isSyncing }) {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedPeriod, setSelectedPeriod] = useState("1Y");

    useEffect(() => {
        const fetchData = async () => {
            try {
                const history = await api.getNetWorthHistory();
                // Ensure data is sorted by date
                const sorted = history.sort(
                    (a, b) => new Date(a.date) - new Date(b.date)
                );
                setData(sorted);
            } catch (err) {
                console.error("Failed to fetch net worth history:", err);
                setError("Failed to load data");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const filteredData = useMemo(() => {
        if (selectedPeriod === "All") return data;

        const period = PERIODS.find((p) => p.label === selectedPeriod);
        if (!period || !period.days) return data;

        const cutoffDate = new Date();
        cutoffDate.setDate(cutoffDate.getDate() - period.days);

        return data.filter((d) => new Date(d.date) >= cutoffDate);
    }, [data, selectedPeriod]);

    const formatXAxis = (tickItem) => {
        const date = new Date(tickItem);
        return date.toLocaleDateString(undefined, {
            month: "short",
            day: "numeric",
        });
    };

    const formatCurrency = (value) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: "GBP",
            notation: "compact",
        }).format(value);
    };

    if (loading || isSyncing) {
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
                {error}
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
                                new Date(label).toLocaleDateString(undefined, {
                                    dateStyle: "medium",
                                })
                            }
                            formatter={(value) => [
                                new Intl.NumberFormat("en-GB", {
                                    style: "currency",
                                    currency: "GBP",
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
