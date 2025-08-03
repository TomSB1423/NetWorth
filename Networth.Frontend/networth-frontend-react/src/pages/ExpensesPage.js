/**
 * Expenses Page - Spending tracking and budget management
 */

import React from 'react';
import {
    Plus,
    Filter,
    TrendingDown
} from 'lucide-react';
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer
} from 'recharts';
import { MOCK_EXPENSES_DATA, MOCK_MONTHLY_EXPENSES_DATA } from '../constants/mockData';

const ExpensesPage = () => {
    const expensesData = MOCK_EXPENSES_DATA;
    const monthlyExpensesData = MOCK_MONTHLY_EXPENSES_DATA;

    const totalExpenses = expensesData.reduce((sum, exp) => sum + exp.amount, 0);
    const totalBudget = expensesData.reduce((sum, exp) => sum + exp.budget, 0);

    return (
        <div>
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 mb-2">Expenses</h1>
                    <p className="text-gray-600">Track your spending and budget performance</p>
                </div>
                <div className="flex items-center space-x-3">
                    <button className="flex items-center space-x-2 px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors">
                        <Filter className="w-4 h-4" />
                        <span>Filter</span>
                    </button>
                    <button className="flex items-center space-x-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                        <Plus className="w-4 h-4" />
                        <span>Add Expense</span>
                    </button>
                </div>
            </div>

            {/* Summary Cards */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Total Expenses</h3>
                    <p className="text-3xl font-bold text-gray-900">${totalExpenses.toLocaleString()}</p>
                    <div className="text-sm text-gray-500 mt-2">This month</div>
                </div>

                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Budget Remaining</h3>
                    <p className="text-3xl font-bold text-green-600">${(totalBudget - totalExpenses).toLocaleString()}</p>
                    <div className="text-sm text-gray-500 mt-2">
                        {((totalBudget - totalExpenses) / totalBudget * 100).toFixed(1)}% of budget
                    </div>
                </div>

                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Avg Daily Spend</h3>
                    <p className="text-3xl font-bold text-gray-900">${(totalExpenses / 30).toFixed(0)}</p>
                    <div className="flex items-center space-x-1 mt-2">
                        <TrendingDown className="w-4 h-4 text-green-500" />
                        <span className="text-sm text-green-600 font-medium">-5% vs last month</span>
                    </div>
                </div>
            </div>

            {/* Charts Section */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Spending by Category */}
                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-lg font-semibold text-gray-900 mb-6">Spending by Category</h3>
                    <div className="space-y-4">
                        {expensesData.map((category) => {
                            const percentage = (category.amount / category.budget) * 100;
                            return (
                                <div key={category.category} className="space-y-2">
                                    <div className="flex items-center justify-between">
                                        <span className="text-sm font-medium text-gray-700">{category.category}</span>
                                        <span className="text-sm text-gray-600">
                                            ${category.amount} / ${category.budget}
                                        </span>
                                    </div>
                                    <div className="w-full bg-gray-200 rounded-full h-2">
                                        <div
                                            className="h-2 rounded-full transition-all duration-300"
                                            style={{
                                                width: `${Math.min(percentage, 100)}%`,
                                                backgroundColor: category.color
                                            }}
                                        ></div>
                                    </div>
                                    <div className="flex justify-between text-xs text-gray-500">
                                        <span>{percentage.toFixed(1)}% of budget</span>
                                        <span className={percentage > 90 ? 'text-red-600' : percentage > 75 ? 'text-yellow-600' : 'text-green-600'}>
                                            {percentage > 90 ? 'Over budget' : percentage > 75 ? 'Close to limit' : 'On track'}
                                        </span>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>

                {/* Monthly Trend */}
                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-lg font-semibold text-gray-900 mb-6">Monthly Trend</h3>
                    <div className="h-64">
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={monthlyExpensesData}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                                <XAxis
                                    dataKey="month"
                                    stroke="#64748b"
                                    fontSize={12}
                                    tickLine={false}
                                    axisLine={false}
                                />
                                <YAxis
                                    stroke="#64748b"
                                    fontSize={12}
                                    tickLine={false}
                                    axisLine={false}
                                    tickFormatter={(value) => `${(value / 1000).toFixed(0)}k`}
                                />
                                <Tooltip
                                    formatter={(value) => [`$${value.toLocaleString()}`, 'Expenses']}
                                    contentStyle={{
                                        backgroundColor: 'white',
                                        border: '1px solid #e5e7eb',
                                        borderRadius: '8px',
                                        boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                                    }}
                                />
                                <Bar dataKey="amount" fill="#6366F1" radius={[4, 4, 0, 0]} />
                            </BarChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ExpensesPage;
