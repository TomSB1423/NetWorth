/**
 * Main Dashboard Layout Component
 * Wraps the entire application with sidebar navigation
 */

import React, { useState } from 'react';
import Sidebar from './Sidebar';
import OverviewPage from '../pages/OverviewPage';
import AccountsPage from '../pages/AccountsPage';
import InvestmentsPage from '../pages/InvestmentsPage';
import ExpensesPage from '../pages/ExpensesPage';
import GoalsPage from '../pages/GoalsPage';
import TransactionsPage from '../pages/TransactionsPage';

const DashboardLayout = () => {
    const [activeTab, setActiveTab] = useState('overview');

    const renderPage = () => {
        switch (activeTab) {
            case 'overview':
                return <OverviewPage />;
            case 'accounts':
                return <AccountsPage />;
            case 'investments':
                return <InvestmentsPage />;
            case 'expenses':
                return <ExpensesPage />;
            case 'goals':
                return <GoalsPage />;
            case 'transactions':
                return <TransactionsPage />;
            default:
                return <OverviewPage />;
        }
    };

    return (
        <div className="flex bg-gray-50 min-h-screen">
            <Sidebar activeTab={activeTab} setActiveTab={setActiveTab} />
            <main className="flex-1 p-8 overflow-auto">
                {renderPage()}
            </main>
        </div>
    );
};

export default DashboardLayout;
