/**
 * Overview Page - Main dashboard view
 */

import React, { useState } from 'react';
import {
    AreaChart,
    Area,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer
} from 'recharts';
import {
    DollarSign,
    PiggyBank,
    TrendingUp,
    CreditCard,
    Brain,
    Zap,
    AlertTriangle
} from 'lucide-react';
import { OverviewCard, LoadingSpinner } from '../components';
import { useInstitutions } from '../hooks/useInstitutions';
import { MOCK_NETWORTH_TREND_DATA } from '../constants/mockData';

// AI Insights Component
const AIInsightsPanel = () => {
    const insights = [
        {
            id: 1,
            type: 'opportunity',
            priority: 'high',
            title: 'Optimize Your Emergency Fund',
            description: 'You have $25,200 in low-yield savings. Consider moving $15,000 to a high-yield savings account earning 4.5% APY.',
            impact: 'Save $675 annually',
            confidence: 94,
            icon: PiggyBank,
            color: 'bg-green-100 text-green-700 border-green-200'
        },
        {
            id: 2,
            type: 'alert',
            priority: 'medium',
            title: 'Credit Card Spending Spike',
            description: 'Your Amex Gold spending increased 15.2% this month.',
            impact: 'Potential overspend of $900',
            confidence: 87,
            icon: AlertTriangle,
            color: 'bg-yellow-100 text-yellow-700 border-yellow-200'
        }
    ];

    return (
        <div className="bg-white rounded-xl p-6 border border-gray-100">
            <div className="flex items-center justify-between mb-6">
                <div className="flex items-center space-x-3">
                    <div className="w-10 h-10 bg-gradient-to-r from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center">
                        <Brain className="w-6 h-6 text-white" />
                    </div>
                    <div>
                        <h3 className="text-lg font-semibold text-gray-900">AI Insights</h3>
                        <p className="text-sm text-gray-600">{insights.length} actionable recommendations</p>
                    </div>
                </div>
            </div>

            <div className="space-y-4">
                {insights.map((insight) => {
                    const Icon = insight.icon;
                    return (
                        <div
                            key={insight.id}
                            className={`border rounded-lg p-4 ${insight.color} hover:shadow-md transition-shadow`}
                        >
                            <div className="flex items-start justify-between mb-3">
                                <div className="flex items-center space-x-3">
                                    <Icon className="w-5 h-5" />
                                    <div>
                                        <h4 className="font-semibold text-sm">{insight.title}</h4>
                                        <span className="text-xs opacity-75">{insight.priority} priority</span>
                                    </div>
                                </div>
                            </div>

                            <p className="text-sm mb-3 opacity-90">{insight.description}</p>

                            <div className="flex items-center justify-between">
                                <div className="flex items-center space-x-2">
                                    <Zap className="w-4 h-4" />
                                    <span className="text-sm font-medium">{insight.impact}</span>
                                </div>
                                <button className="px-3 py-1 bg-white bg-opacity-80 hover:bg-opacity-100 rounded-lg text-sm font-medium transition-colors">
                                    Take Action
                                </button>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

// Net Worth Trend Chart Component
const NetWorthTrendChart = () => {
    const [timeframe, setTimeframe] = useState('12M');

    return (
        <div className="bg-white rounded-xl p-6 border border-gray-100">
            <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-semibold text-gray-900">Net Worth Trend</h3>
                <div className="flex space-x-1 bg-gray-100 rounded-lg p-1">
                    {['6M', '12M', '24M'].map((period) => (
                        <button
                            key={period}
                            onClick={() => setTimeframe(period)}
                            className={`px-3 py-1 rounded-md text-sm font-medium transition-colors ${timeframe === period
                                    ? 'bg-indigo-600 text-white'
                                    : 'text-gray-600 hover:text-gray-900'
                                }`}
                        >
                            {period}
                        </button>
                    ))}
                </div>
            </div>

            <div className="h-64">
                <ResponsiveContainer width="100%" height="100%">
                    <AreaChart data={MOCK_NETWORTH_TREND_DATA}>
                        <defs>
                            <linearGradient id="netWorthGradient" x1="0" y1="0" x2="0" y2="1">
                                <stop offset="5%" stopColor="#6366F1" stopOpacity={0.3} />
                                <stop offset="95%" stopColor="#6366F1" stopOpacity={0} />
                            </linearGradient>
                        </defs>
                        <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                        <XAxis dataKey="month" stroke="#64748b" fontSize={12} tickLine={false} axisLine={false} />
                        <YAxis
                            stroke="#64748b"
                            fontSize={12}
                            tickLine={false}
                            axisLine={false}
                            tickFormatter={(value) => `$${(value / 1000).toFixed(0)}k`}
                        />
                        <Tooltip
                            formatter={(value) => [`$${value.toLocaleString()}`, 'Net Worth']}
                            contentStyle={{
                                backgroundColor: 'white',
                                border: '1px solid #e5e7eb',
                                borderRadius: '8px',
                                boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                            }}
                        />
                        <Area
                            type="monotone"
                            dataKey="value"
                            stroke="#6366F1"
                            strokeWidth={3}
                            fill="url(#netWorthGradient)"
                            dot={false}
                        />
                    </AreaChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};

// Key Metrics Component
const KeyMetrics = () => {
    const metrics = [
        { label: 'Credit Score', value: '785', subtitle: 'Excellent', change: '+15 this month', color: 'text-green-600' },
        { label: 'Emergency Fund', value: '$25,200', subtitle: '6 months expenses', change: '84% of goal', color: 'text-blue-600' },
        { label: 'Investment Growth', value: '12.4%', subtitle: 'YTD Performance', change: 'Above benchmark', color: 'text-green-600' },
        { label: 'Debt-to-Income', value: '16.4%', subtitle: 'Monthly ratio', change: 'Excellent range', color: 'text-indigo-600' }
    ];

    return (
        <div className="bg-white rounded-xl p-6 border border-gray-100">
            <h3 className="text-lg font-semibold text-gray-900 mb-6">Key Metrics</h3>
            <div className="space-y-6">
                {metrics.map((metric, index) => (
                    <div key={index} className="flex items-center justify-between">
                        <div>
                            <p className="text-gray-600 text-sm font-medium">{metric.label}</p>
                            <p className="text-gray-500 text-xs">{metric.subtitle}</p>
                        </div>
                        <div className="text-right">
                            <p className={`text-2xl font-bold ${metric.color}`}>{metric.value}</p>
                            <p className="text-gray-500 text-xs">{metric.change}</p>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

const OverviewPage = () => {
    const { accounts, loading, error } = useInstitutions();

    // Calculate overview metrics from accounts
    const calculateMetrics = () => {
        if (!accounts.length) return null;

        const totalAssets = accounts.filter(acc => acc.balance > 0).reduce((sum, acc) => sum + acc.balance, 0);
        const totalLiabilities = Math.abs(accounts.filter(acc => acc.balance < 0).reduce((sum, acc) => sum + acc.balance, 0));
        const netWorth = totalAssets - totalLiabilities;
        const liquidCash = accounts.filter(acc => acc.type === 'Checking' || acc.type === 'Savings').reduce((sum, acc) => sum + acc.balance, 0);

        return {
            netWorth: `$${netWorth.toLocaleString()}`,
            totalAssets: `$${totalAssets.toLocaleString()}`,
            totalLiabilities: `$${totalLiabilities.toLocaleString()}`,
            liquidCash: `$${liquidCash.toLocaleString()}`
        };
    };

    if (loading) {
        return <LoadingSpinner text="Loading overview..." />;
    }

    if (error) {
        return (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
                Error loading data: {error}
            </div>
        );
    }

    const metrics = calculateMetrics();

    return (
        <div>
            {/* Header */}
            <div className="mb-8">
                <h1 className="text-2xl font-bold text-gray-900 mb-2">Dashboard Overview</h1>
                <p className="text-gray-600">Welcome back! Here's your financial snapshot.</p>
            </div>

            {/* Overview Cards */}
            {metrics && (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
                    <OverviewCard
                        title="Net Worth"
                        amount={metrics.netWorth}
                        changePercent="8.2"
                        icon={DollarSign}
                        iconBg="bg-indigo-600"
                        isPositive={true}
                    />
                    <OverviewCard
                        title="Total Assets"
                        amount={metrics.totalAssets}
                        changePercent="5.1"
                        icon={TrendingUp}
                        iconBg="bg-green-600"
                        isPositive={true}
                    />
                    <OverviewCard
                        title="Total Liabilities"
                        amount={metrics.totalLiabilities}
                        changePercent="2.3"
                        icon={CreditCard}
                        iconBg="bg-red-600"
                        isPositive={false}
                    />
                    <OverviewCard
                        title="Liquid Cash"
                        amount={metrics.liquidCash}
                        changePercent="1.8"
                        icon={PiggyBank}
                        iconBg="bg-blue-600"
                        isPositive={true}
                    />
                </div>
            )}

            {/* Charts and Insights */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
                <div className="lg:col-span-2">
                    <NetWorthTrendChart />
                </div>
                <div>
                    <KeyMetrics />
                </div>
            </div>

            {/* AI Insights */}
            <AIInsightsPanel />
        </div>
    );
};

export default OverviewPage;
