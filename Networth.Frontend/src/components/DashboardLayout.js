/**
 * Main Dashboard Layout Component
 * Wraps the entire application with sidebar navigation
 */

import React from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';

const DashboardLayout = () => {
    return (
        <div className="flex bg-gray-50 min-h-screen">
            <Sidebar />
            <main className="flex-1 overflow-auto">
                <Outlet />
            </main>
        </div>
    );
};

export default DashboardLayout;
