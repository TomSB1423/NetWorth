import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import DashboardLayout from './components/DashboardLayout';
import OverviewPage from './pages/OverviewPage';
import AccountsPage from './pages/AccountsPage';
import InvestmentsPage from './pages/InvestmentsPage';
import ExpensesPage from './pages/ExpensesPage';
import GoalsPage from './pages/GoalsPage';
import TransactionsPage from './pages/TransactionsPage';
import InstitutionsPage from './pages/InstitutionsPage';
import LinkInstitutionPage from './pages/LinkInstitutionPage';
import AccountsListPage from './pages/AccountsListPage';
import AccountDetailsPage from './pages/AccountDetailsPage';
import './App.css';

function App() {
    return (
        <Router>
            <div className="App">
                <Routes>
                    {/* Main dashboard routes with sidebar */}
                    <Route path="/" element={<DashboardLayout />}>
                        <Route index element={<Navigate to="/overview" replace />} />
                        <Route path="overview" element={<OverviewPage />} />
                        <Route path="accounts" element={<AccountsPage />} />
                        <Route path="investments" element={<InvestmentsPage />} />
                        <Route path="expenses" element={<ExpensesPage />} />
                        <Route path="goals" element={<GoalsPage />} />
                        <Route path="transactions" element={<TransactionsPage />} />
                    </Route>

                    {/* Account management routes (full page, no sidebar) */}
                    <Route path="/accounts/list" element={<AccountsListPage />} />
                    <Route path="/accounts/:accountId" element={<AccountDetailsPage />} />
                    <Route path="/institutions" element={<InstitutionsPage />} />
                    <Route path="/institutions/link" element={<LinkInstitutionPage />} />

                    {/* Fallback route */}
                    <Route path="*" element={<Navigate to="/overview" replace />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
