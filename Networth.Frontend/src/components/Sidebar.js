/**
 * Sidebar Navigation Component
 * Provides navigation between different sections of the application
 */

import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
    DollarSign,
    Home,
    CreditCard,
    TrendingUp,
    Receipt,
    Target,
    BarChart3
} from 'lucide-react';

const Sidebar = () => {
    const navigate = useNavigate();
    const location = useLocation();
    
    const menuItems = [
        { id: 'overview', label: 'Overview', icon: Home, path: '/overview' },
        { id: 'accounts', label: 'Accounts', icon: CreditCard, path: '/accounts' },
        { id: 'investments', label: 'Investments', icon: TrendingUp, path: '/investments' },
        { id: 'expenses', label: 'Expenses', icon: Receipt, path: '/expenses' },
        { id: 'goals', label: 'Goals', icon: Target, path: '/goals' },
        { id: 'transactions', label: 'Transactions', icon: BarChart3, path: '/transactions' },
    ];

    const isActive = (path) => location.pathname === path;

    return (
        <div className="w-64 bg-white border-r border-gray-200 min-h-screen">
            <div className="p-6">
                {/* Logo and App Name */}
                <div className="flex items-center space-x-2 mb-8">
                    <div className="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center">
                        <DollarSign className="w-5 h-5 text-white" />
                    </div>
                    <span className="text-xl font-bold text-gray-900">NetWorth</span>
                </div>

                {/* Development Badge */}
                <div className="bg-gray-800 text-white px-3 py-1 rounded text-sm mb-6">
                    Development Mode
                </div>

                {/* Navigation Menu */}
                <nav className="space-y-1">
                    {menuItems.map((item) => {
                        const IconComponent = item.icon;
                        const active = isActive(item.path);
                        return (
                            <button
                                key={item.id}
                                onClick={() => navigate(item.path)}
                                className={`w-full flex items-center space-x-3 px-3 py-2 rounded-lg text-left transition-colors ${
                                    active
                                        ? 'bg-indigo-600 text-white'
                                        : 'text-gray-600 hover:bg-gray-100'
                                    }`}
                                aria-label={`Navigate to ${item.label}`}
                            >
                                <IconComponent className="w-5 h-5" />
                                <span className="font-medium">{item.label}</span>
                            </button>
                        );
                    })}
                </nav>
            </div>
        </div>
    );
};

export default Sidebar;
