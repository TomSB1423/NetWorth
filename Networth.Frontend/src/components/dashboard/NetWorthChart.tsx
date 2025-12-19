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
import { Loader2, TrendingUp } from "lucide-react";
import { NetWorthDataPoint } from "../../types";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { Button } from "../ui/button";

const PERIODS = [
    { label: "1M", days: 30 },
    { label: "3M", days: 90 },
    { label: "6M", days: 180 },
    { label: "1Y", days: 365 },
    { label: "All", days: null },
];

interface NetWorthChartProps {
    currency?: string;
    isSyncing?: boolean;
    hasAccounts?: boolean;
}

export function NetWorthChart({
    currency = "GBP",
    isSyncing = false,
    hasAccounts = true,
}: NetWorthChartProps) {
    // Compute the initial period based on data span
    const getInitialPeriod = (dataPoints: NetWorthDataPoint[]): string => {
        if (dataPoints.length < 2) return "All";
        const firstDate = new Date(dataPoints[0].date);
        const lastDate = new Date(dataPoints[dataPoints.length - 1].date);
        const spanDays = Math.ceil(
            (lastDate.getTime() - firstDate.getTime()) / (1000 * 60 * 60 * 24)
        );
        // Default to 1Y if we have enough data, otherwise "All"
        return spanDays >= 365 ? "1Y" : "All";
    };

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

    const [selectedPeriod, setSelectedPeriod] = useState(() =>
        getInitialPeriod(history)
    );

    const dataSpanDays = useMemo(() => {
        if (data.length < 2) return 0;
        const firstDate = new Date(data[0].date);
        const lastDate = new Date(data[data.length - 1].date);
        return Math.ceil(
            (lastDate.getTime() - firstDate.getTime()) / (1000 * 60 * 60 * 24)
        );
    }, [data]);

    const availablePeriods = useMemo(() => {
        return PERIODS.filter((period) => {
            if (period.days === null) return true; // Always show "All"
            return dataSpanDays >= period.days;
        });
    }, [dataSpanDays]);

    // Compute valid period - if current selection is unavailable, default to "All"
    const validSelectedPeriod = useMemo(() => {
        const isCurrentPeriodAvailable = availablePeriods.some(
            (p) => p.label === selectedPeriod
        );
        return isCurrentPeriodAvailable ? selectedPeriod : "All";
    }, [availablePeriods, selectedPeriod]);

    const filteredData = useMemo(() => {
        if (validSelectedPeriod === "All") return data;

        const period = PERIODS.find((p) => p.label === validSelectedPeriod);
        if (!period || !period.days) return data;

        const cutoffDate = new Date();
        cutoffDate.setDate(cutoffDate.getDate() - period.days);

        return data.filter((d) => new Date(d.date) >= cutoffDate);
    }, [data, validSelectedPeriod]);

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

    const showLoading = isLoading || (isSyncing && data.length === 0);

    if (showLoading) {
        return (
            <Card>
                <CardContent className="h-[400px] flex items-center justify-center text-gray-400">
                    <Loader2 className="w-8 h-8 animate-spin" />
                    <span className="ml-2">
                        {isSyncing
                            ? "Syncing account data..."
                            : "Loading data..."}
                    </span>
                </CardContent>
            </Card>
        );
    }

    if (error) {
        return (
            <Card>
                <CardContent className="h-[400px] flex items-center justify-center text-red-500">
                    {(error as Error).message || "Failed to load data"}
                </CardContent>
            </Card>
        );
    }

    if (!hasAccounts) {
        return (
            <Card>
                <CardHeader className="flex flex-row items-center gap-4">
                    <div className="w-12 h-12 rounded-2xl bg-emerald-500/20 flex items-center justify-center">
                        <TrendingUp size={24} className="text-emerald-400" />
                    </div>
                    <div>
                        <CardTitle>Net Worth Trajectory</CardTitle>
                        <p className="text-sm text-gray-400 mt-1">
                            Growth over time
                        </p>
                    </div>
                </CardHeader>
                <CardContent className="h-[350px] flex flex-col items-center justify-center text-gray-400">
                    <p className="text-lg">Please add an account</p>
                    <p className="text-sm mt-2">
                        Link a bank account to see your net worth over time
                    </p>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between">
                <div className="flex items-center gap-4">
                    <div className="w-12 h-12 rounded-2xl bg-emerald-500/20 flex items-center justify-center">
                        <TrendingUp size={24} className="text-emerald-400" />
                    </div>
                    <div>
                        <CardTitle>Net Worth Trajectory</CardTitle>
                        <p className="text-sm text-gray-400 mt-1">
                            Growth over time
                        </p>
                    </div>
                </div>
                <div className="flex gap-2 p-1 rounded-xl bg-slate-800">
                    {availablePeriods.map((period) => (
                        <Button
                            key={period.label}
                            variant="ghost"
                            size="sm"
                            onClick={() => setSelectedPeriod(period.label)}
                            className={`min-w-[50px] h-9 rounded-lg font-semibold ${
                                validSelectedPeriod === period.label
                                    ? "bg-emerald-500 text-white hover:bg-emerald-600"
                                    : "text-gray-400 hover:text-white hover:bg-slate-700"
                            }`}
                        >
                            {period.label}
                        </Button>
                    ))}
                </div>
            </CardHeader>
            <CardContent>
                <div className="h-[350px]">
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
                                        stopColor="#10B981"
                                        stopOpacity={0.4}
                                    />
                                    <stop
                                        offset="95%"
                                        stopColor="#10B981"
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
                                    new Date(label).toLocaleDateString(
                                        "en-GB",
                                        {
                                            dateStyle: "medium",
                                        }
                                    )
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
                                stroke="#10B981"
                                strokeWidth={3}
                                fillOpacity={1}
                                fill="url(#colorValue)"
                            />
                        </AreaChart>
                    </ResponsiveContainer>
                </div>
            </CardContent>
        </Card>
    );
}
