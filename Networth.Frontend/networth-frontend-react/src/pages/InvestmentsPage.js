/**
 * Investments Page - Portfolio tracking and analysis
 */

import React from 'react';
import {
    Plus,
    TrendingUp,
    ArrowUpRight,
    ArrowDownRight
} from 'lucide-react';
import { MOCK_INVESTMENTS_DATA } from '../constants/mockData';

const InvestmentsPage = () => {
    const investmentsData = MOCK_INVESTMENTS_DATA;
    const totalValue = investmentsData.reduce((sum, inv) => sum + inv.value, 0);

    return (
        <div>
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 mb-2">Investments</h1>
                    <p className="text-gray-600">Track your portfolio performance and allocation</p>
                </div>
                <button className="flex items-center space-x-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                    <Plus className="w-4 h-4" />
                    <span>Add Investment</span>
                </button>
            </div>

            {/* Summary Cards */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Total Portfolio Value</h3>
                    <p className="text-3xl font-bold text-gray-900">${totalValue.toLocaleString()}</p>
                    <div className="flex items-center space-x-1 mt-2">
                        <TrendingUp className="w-4 h-4 text-green-500" />
                        <span className="text-sm text-green-600 font-medium">+8.2% YTD</span>
                    </div>
                </div>

                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Today's Change</h3>
                    <p className="text-3xl font-bold text-green-600">+$2,847</p>
                    <div className="flex items-center space-x-1 mt-2">
                        <ArrowUpRight className="w-4 h-4 text-green-500" />
                        <span className="text-sm text-green-600 font-medium">+2.2%</span>
                    </div>
                </div>

                <div className="bg-white rounded-xl p-6 border border-gray-100">
                    <h3 className="text-sm font-medium text-gray-600 mb-2">Dividend Income</h3>
                    <p className="text-3xl font-bold text-gray-900">$1,248</p>
                    <div className="text-sm text-gray-500 mt-2">This month</div>
                </div>
            </div>

            {/* Holdings Table */}
            <div className="bg-white rounded-xl border border-gray-100">
                <div className="p-6 border-b border-gray-100">
                    <h3 className="text-lg font-semibold text-gray-900">Holdings</h3>
                </div>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Symbol</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Shares</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Value</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Change</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Allocation</th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {investmentsData.map((investment) => (
                                <tr key={investment.symbol} className="hover:bg-gray-50">
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div className="font-medium text-gray-900">{investment.symbol}</div>
                                    </td>
                                    <td className="px-6 py-4">
                                        <div className="text-sm text-gray-900 max-w-xs truncate">{investment.name}</div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {investment.shares.toLocaleString()}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        ${investment.price.toFixed(2)}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                        ${investment.value.toLocaleString()}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className={`inline-flex items-center text-sm font-medium ${investment.change >= 0 ? 'text-green-600' : 'text-red-600'
                                            }`}>
                                            {investment.change >= 0 ? (
                                                <ArrowUpRight className="w-4 h-4 mr-1" />
                                            ) : (
                                                <ArrowDownRight className="w-4 h-4 mr-1" />
                                            )}
                                            {investment.change > 0 ? '+' : ''}{investment.change}%
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div className="flex items-center">
                                            <div className="w-16 bg-gray-200 rounded-full h-2 mr-2">
                                                <div
                                                    className="bg-indigo-600 h-2 rounded-full"
                                                    style={{ width: `${investment.allocation}%` }}
                                                ></div>
                                            </div>
                                            <span className="text-sm text-gray-600">{investment.allocation}%</span>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default InvestmentsPage;
