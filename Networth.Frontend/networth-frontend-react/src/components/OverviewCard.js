/**
 * Reusable Card Component for displaying key metrics
 */

import React from 'react';
import PropTypes from 'prop-types';
import { TrendingUp, TrendingDown } from 'lucide-react';

const OverviewCard = ({ title, amount, change, changePercent, icon: Icon, iconBg, isPositive }) => {
    return (
        <div className="bg-white rounded-xl p-6 border border-gray-100 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-start justify-between mb-4">
                <div>
                    <p className="text-gray-600 text-sm font-medium mb-1">{title}</p>
                    <p className="text-3xl font-bold text-gray-900">{amount}</p>
                </div>
                <div className={`p-3 rounded-xl ${iconBg}`}>
                    <Icon className="w-6 h-6 text-white" />
                </div>
            </div>

            <div className="flex items-center space-x-1">
                {isPositive ? (
                    <TrendingUp className="w-4 h-4 text-green-500" />
                ) : (
                    <TrendingDown className="w-4 h-4 text-red-500" />
                )}
                <span className={`text-sm font-medium ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
                    {changePercent}%
                </span>
                <span className="text-gray-500 text-sm">vs last month</span>
            </div>
        </div>
    );
};

OverviewCard.propTypes = {
    title: PropTypes.string.isRequired,
    amount: PropTypes.string.isRequired,
    change: PropTypes.number,
    changePercent: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
    icon: PropTypes.elementType.isRequired,
    iconBg: PropTypes.string.isRequired,
    isPositive: PropTypes.bool.isRequired,
};

export default OverviewCard;
