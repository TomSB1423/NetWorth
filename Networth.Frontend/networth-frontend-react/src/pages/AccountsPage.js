/**
 * Accounts Page - Display and manage all financial accounts
 */

import React, { useState } from 'react';
import {
    Eye,
    EyeOff,
    Plus,
    TrendingUp,
    TrendingDown,
    Wallet,
    PiggyBank,
    Shield,
    Building,
    CreditCard,
    RefreshCw
} from 'lucide-react';
import { LoadingSpinner } from '../components';
import { useInstitutions } from '../hooks/useInstitutions';

const AccountsPage = () => {
    const [showBalance, setShowBalance] = useState(true);
    const { accounts, institutions, loading, error, syncing, syncInstitution } = useInstitutions();

    const getAccountIcon = (type) => {
        switch (type) {
            case 'Checking': return Wallet;
            case 'Savings': return PiggyBank;
            case 'Retirement': return Shield;
            case 'Mortgage': return Building;
            case 'Credit Card': return CreditCard;
            default: return Wallet;
        }
    };

    const handleSync = async (institutionId) => {
        try {
            await syncInstitution(institutionId);
        } catch (err) {
            console.error('Failed to sync institution:', err);
        }
    };

    if (loading) {
        return <LoadingSpinner text="Loading accounts..." />;
    }

    if (error) {
        return (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
                Error loading accounts: {error}
            </div>
        );
    }

    return (
        <div>
            {/* Header */}
            <div className="flex items-center justify-between mb-8">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900 mb-2">Accounts</h1>
                    <p className="text-gray-600">Manage all your financial accounts in one place</p>
                </div>
                <div className="flex items-center space-x-3">
                    <button
                        onClick={() => setShowBalance(!showBalance)}
                        className="flex items-center space-x-2 px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
                    >
                        {showBalance ? <Eye className="w-4 h-4" /> : <EyeOff className="w-4 h-4" />}
                        <span className="text-sm">{showBalance ? 'Hide' : 'Show'} Balances</span>
                    </button>
                    <button className="flex items-center space-x-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                        <Plus className="w-4 h-4" />
                        <span>Add Account</span>
                    </button>
                </div>
            </div>

            {/* Accounts Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {accounts.map((account) => {
                    const Icon = getAccountIcon(account.type);
                    const isNegative = account.balance < 0;
                    const institution = institutions.find(inst => inst.id === account.institutionId);
                    const isSyncing = syncing.has(account.institutionId);

                    return (
                        <div key={account.id} className="bg-white rounded-xl p-6 border border-gray-100 hover:shadow-lg transition-shadow">
                            <div className="flex items-start justify-between mb-4">
                                <div className="flex items-center space-x-3">
                                    <div className={`p-2 rounded-lg ${isNegative ? 'bg-red-100' : 'bg-indigo-100'}`}>
                                        <Icon className={`w-5 h-5 ${isNegative ? 'text-red-600' : 'text-indigo-600'}`} />
                                    </div>
                                    <div>
                                        <h3 className="font-semibold text-gray-900 text-sm">{account.name}</h3>
                                        <p className="text-gray-500 text-xs">{account.bank}</p>
                                    </div>
                                </div>
                                <div className="flex items-center space-x-2">
                                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${account.type === 'Credit Card' || account.type === 'Mortgage'
                                            ? 'bg-red-100 text-red-700'
                                            : 'bg-green-100 text-green-700'
                                        }`}>
                                        {account.type}
                                    </span>
                                    <button
                                        onClick={() => handleSync(account.institutionId)}
                                        disabled={isSyncing}
                                        className="p-1 text-gray-400 hover:text-gray-600 transition-colors disabled:opacity-50"
                                        title="Sync account"
                                    >
                                        <RefreshCw className={`w-4 h-4 ${isSyncing ? 'animate-spin' : ''}`} />
                                    </button>
                                </div>
                            </div>

                            <div className="mb-4">
                                <p className="text-2xl font-bold text-gray-900">
                                    {showBalance ? `$${Math.abs(account.balance).toLocaleString()}` : '••••••'}
                                </p>
                                <div className="flex items-center space-x-1 mt-1">
                                    {account.change > 0 ? (
                                        <TrendingUp className="w-4 h-4 text-green-500" />
                                    ) : (
                                        <TrendingDown className="w-4 h-4 text-red-500" />
                                    )}
                                    <span className={`text-sm ${account.change > 0 ? 'text-green-600' : 'text-red-600'}`}>
                                        {account.change > 0 ? '+' : ''}{account.change}%
                                    </span>
                                </div>
                            </div>

                            <div className="flex items-center justify-between text-xs text-gray-500">
                                <span>Last updated: {account.lastUpdated}</span>
                                {institution && (
                                    <span className="text-indigo-600 font-medium">
                                        {institution.name}
                                    </span>
                                )}
                            </div>
                        </div>
                    );
                })}
            </div>

            {/* Empty State */}
            {accounts.length === 0 && (
                <div className="text-center py-12">
                    <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <CreditCard className="w-8 h-8 text-gray-400" />
                    </div>
                    <h3 className="text-lg font-medium text-gray-900 mb-2">No accounts connected</h3>
                    <p className="text-gray-600 mb-6">Get started by connecting your first financial account.</p>
                    <button className="px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
                        Connect Your First Account
                    </button>
                </div>
            )}
        </div>
    );
};

export default AccountsPage;
