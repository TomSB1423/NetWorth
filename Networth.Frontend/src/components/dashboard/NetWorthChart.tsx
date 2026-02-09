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
import { Loader2, TrendingUp, Clock } from "lucide-react";
import { NetWorthDataPoint, NetWorthHistoryResponse } from "../../types";
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
            (lastDate.getTime() - firstDate.getTime()) / (1000 * 60 * 60 * 24),
        );
        // Default to 1Y if we have enough data, otherwise "All"
        return spanDays >= 365 ? "1Y" : "All";
    };

    // Determine if we should poll for updates
    const shouldPoll = (status: string | undefined): boolean => {
        return status === "NotCalculated" || status === "Calculating";
    };

    const {
        data: historyResponse,
        isLoading,
        error,
        isFetched,
    } = useQuery<NetWorthHistoryResponse>({
        queryKey: ["netWorthHistory"],
        queryFn: api.getNetWorthHistory,
        // Poll every 3 seconds when not yet calculated
        refetchInterval: (query) => {
            const currentStatus = query.state.data?.status;
            return shouldPoll(currentStatus) ? 3000 : false;
        },
    });

    // Only derive status after data has been fetched to avoid race conditions
    const history = useMemo(
        () => historyResponse?.dataPoints ?? [],
        [historyResponse?.dataPoints],
    );
    const status = historyResponse?.status;
    const lastCalculated = historyResponse?.lastCalculated;

    const data = useMemo(() => {
        return [...history].sort(
            (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime(),
        );
    }, [history]);

    const formatLastCalculated = (
        dateString: string | null | undefined,
    ): string => {
        if (!dateString) return "Never";
        const date = new Date(dateString);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffMins = Math.floor(diffMs / (1000 * 60));
        const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
        const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

        if (diffMins < 1) return "Just now";
        if (diffMins < 60)
            return `${diffMins} minute${diffMins === 1 ? "" : "s"} ago`;
        if (diffHours < 24)
            return `${diffHours} hour${diffHours === 1 ? "" : "s"} ago`;
        if (diffDays < 7)
            return `${diffDays} day${diffDays === 1 ? "" : "s"} ago`;

        return date.toLocaleDateString("en-GB", {
            day: "numeric",
            month: "short",
            year: "numeric",
        });
    };

    const [selectedPeriod, setSelectedPeriod] = useState(() =>
        getInitialPeriod(history),
    );

    const dataSpanDays = useMemo(() => {
        if (data.length < 2) return 0;
        const firstDate = new Date(data[0].date);
        const lastDate = new Date(data[data.length - 1].date);
        return Math.ceil(
            (lastDate.getTime() - firstDate.getTime()) / (1000 * 60 * 60 * 24),
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
            (p) => p.label === selectedPeriod,
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

    const filteredDataSpanDays = useMemo(() => {
        if (filteredData.length < 2) return 0;
        const firstDate = new Date(filteredData[0].date);
        const lastDate = new Date(filteredData[filteredData.length - 1].date);
        return Math.ceil(
            (lastDate.getTime() - firstDate.getTime()) / (1000 * 60 * 60 * 24),
        );
    }, [filteredData]);

    const chartData = useMemo(() => {
        return filteredData.map((d) => ({
            ...d,
            timestamp: new Date(d.date).getTime(),
        }));
    }, [filteredData]);

    const xTicks = useMemo(() => {
        if (chartData.length < 2) return undefined;

        const min = chartData[0].timestamp;
        const max = chartData[chartData.length - 1].timestamp;

        // For short periods (<= 60 days), let Recharts auto-generate ticks
        if (filteredDataSpanDays <= 60) return undefined;

        // Calculate optimal number of ticks based on data span
        // Aim for roughly one tick per month, but evenly spaced
        const monthsInSpan = filteredDataSpanDays / 30;
        const targetTicks = Math.min(Math.max(Math.round(monthsInSpan), 4), 12);

        // Generate evenly spaced ticks across the data range
        const ticks: number[] = [];
        const step = (max - min) / (targetTicks - 1);

        for (let i = 0; i < targetTicks; i++) {
            ticks.push(Math.round(min + step * i));
        }

        return ticks;
    }, [chartData, filteredDataSpanDays]);

    // Track the first year displayed to show year on first tick
    const firstDataYear = useMemo(() => {
        if (chartData.length === 0) return null;
        return new Date(chartData[0].timestamp).getFullYear();
    }, [chartData]);

    const formatXAxis = (tickItem: number) => {
        const date = new Date(tickItem);
        const tickYear = date.getFullYear();
        const isJanuary = date.getMonth() === 0;

        if (filteredDataSpanDays <= 60) {
            // Short period: show "15 Jan" or "15 Jan '25" for January/first year
            const dayMonth = date.toLocaleDateString("en-GB", {
                day: "numeric",
                month: "short",
            });
            const showYear = isJanuary || tickYear === firstDataYear;
            return showYear
                ? `${dayMonth} '${String(tickYear).slice(-2)}`
                : dayMonth;
        }

        // Longer periods: show "Jan '25" format for all ticks
        // This provides clear context since ticks are evenly spaced, not on month boundaries
        return date.toLocaleDateString("en-GB", {
            month: "short",
            year: "2-digit",
        });
    };

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat("en-GB", {
            style: "currency",
            currency: currency,
            notation: "compact",
        }).format(value);
    };

    // Show loading when:
    // 1. Initial fetch is in progress
    // 2. Syncing with no existing data to show
    // 3. Backend reports data is not yet calculated or calculation is in progress
    const isCalculationPending =
        status === "NotCalculated" || status === "Calculating";
    const showLoading =
        isLoading ||
        !isFetched ||
        (isSyncing && data.length === 0) ||
        isCalculationPending;

    if (showLoading) {
        return (
            <Card>
                <CardContent className="h-[280px] flex items-center justify-center text-gray-400">
                    <Loader2 className="w-6 h-6 animate-spin" />
                    <span className="ml-2 text-sm">
                        {isCalculationPending
                            ? "Calculating net worth..."
                            : isSyncing
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
                <CardContent className="h-[280px] flex items-center justify-center text-red-500 text-sm">
                    {(error as Error).message || "Failed to load data"}
                </CardContent>
            </Card>
        );
    }

    if (!hasAccounts) {
        return (
            <Card>
                <CardHeader className="flex flex-row items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-emerald-500/20 flex items-center justify-center">
                        <TrendingUp size={18} className="text-emerald-400" />
                    </div>
                    <div>
                        <CardTitle className="text-base">
                            Net Worth Trajectory
                        </CardTitle>
                        <p className="text-xs text-gray-400">
                            Growth over time
                        </p>
                    </div>
                </CardHeader>
                <CardContent className="h-[240px] flex flex-col items-center justify-center text-gray-400">
                    <p className="text-sm">Please add an account</p>
                    <p className="text-xs mt-1">
                        Link a bank account to see your net worth over time
                    </p>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between">
                <div className="flex items-center gap-3">
                    <div className="w-9 h-9 rounded-xl bg-emerald-500/20 flex items-center justify-center">
                        <TrendingUp size={18} className="text-emerald-400" />
                    </div>
                    <div>
                        <CardTitle className="text-base">
                            Net Worth Trajectory
                        </CardTitle>
                        <div className="flex items-center gap-2 text-xs text-gray-400">
                            <span>Growth over time</span>
                            {lastCalculated && (
                                <span className="flex items-center gap-1 text-[11px] text-gray-500">
                                    <Clock size={10} />
                                    Updated{" "}
                                    {formatLastCalculated(lastCalculated)}
                                </span>
                            )}
                        </div>
                    </div>
                </div>
                <div className="flex gap-1 p-0.5 rounded-lg bg-slate-800">
                    {availablePeriods.map((period) => (
                        <Button
                            key={period.label}
                            variant="ghost"
                            size="sm"
                            onClick={() => setSelectedPeriod(period.label)}
                            className={`min-w-[40px] h-7 rounded-md font-medium text-xs ${
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
                <div className="h-[260px]">
                    <ResponsiveContainer width="100%" height="100%">
                        <AreaChart
                            data={chartData}
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
                                dataKey="timestamp"
                                type="number"
                                domain={["dataMin", "dataMax"]}
                                ticks={xTicks}
                                stroke="#9CA3AF"
                                tickFormatter={formatXAxis}
                                tick={{ fontSize: 12 }}
                                tickMargin={8}
                            />
                            <YAxis
                                stroke="#9CA3AF"
                                tickFormatter={formatCurrency}
                                width={65}
                                domain={[
                                    (dataMin: number) =>
                                        Math.floor(dataMin * 0.95),
                                    (dataMax: number) =>
                                        Math.ceil(dataMax * 1.02),
                                ]}
                                tickCount={5}
                            />
                            <Tooltip
                                cursor={false}
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
                                        },
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
