import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { accountService, devService } from '../services/api';
import { LoadingSpinner } from '../components';
import { PageHeader, PageContainer } from '../components/layout';
import { Wallet, Plus, ChevronRight, Trash2 } from 'lucide-react';
import './AccountsListPage.css';

/**
 * AccountsListPage - Page for displaying user's connected bank accounts
 */
function AccountsListPage() {
    const navigate = useNavigate();
    const [accounts, setAccounts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [refreshing, setRefreshing] = useState(false);
    const [resetting, setResetting] = useState(false);
    const isDevelopment = process.env.NODE_ENV === 'development';

    useEffect(() => {
        loadAccounts();
    }, []);

    const loadAccounts = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await accountService.getAccounts();
            setAccounts(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleRefresh = async () => {
        setRefreshing(true);
        await loadAccounts();
        setRefreshing(false);
    };

    const handleResetDatabase = async () => {
        if (!window.confirm('Are you sure you want to reset the database? This will delete all accounts and data!')) {
            return;
        }

        try {
            setResetting(true);
            setError(null);
            await devService.resetDatabase();
            await loadAccounts();
        } catch (err) {
            setError(err.message);
        } finally {
            setResetting(false);
        }
    };

    const handleAddAccount = () => {
        navigate('/institutions/link');
    };

    const handleSelectAccount = (account) => {
        navigate(`/accounts/${account.id}`);
    };

    if (loading) {
        return (
            <PageContainer>
                <LoadingSpinner text="Loading accounts..." size="large" />
            </PageContainer>
        );
    }

    const headerActions = (
        <>
            {isDevelopment && (
                <button
                    onClick={handleResetDatabase}
                    disabled={resetting}
                    className="btn-danger"
                    title="Reset database (Development only)"
                >
                    <Trash2 size={16} />
                    Reset DB
                </button>
            )}
            <button
                onClick={handleAddAccount}
                className="btn-primary"
            >
                <Plus size={16} />
                Add Account
            </button>
        </>
    );

    return (
        <PageContainer>
            <PageHeader
                title="My Accounts"
                subtitle="View and manage your connected bank accounts"
                showBack={true}
                backTo="/accounts"
                onRefresh={handleRefresh}
                refreshing={refreshing}
                actions={headerActions}
            />

            {error && (
                <div className="error-banner">
                    <p>{error}</p>
                    <button onClick={() => setError(null)}>Dismiss</button>
                </div>
            )}

            {accounts.length === 0 ? (
                <div className="empty-state">
                    <Wallet size={64} />
                    <h3>No Accounts Connected</h3>
                    <p>Start by connecting your first bank account to track your finances</p>
                    <button
                        onClick={handleAddAccount}
                        className="btn-primary"
                    >
                        <Plus size={16} />
                        Add Your First Account
                    </button>
                </div>
            ) : (
                <div className="accounts-list">
                    {accounts.map((account) => (
                        <div
                            key={account.id}
                            className="account-card"
                            onClick={() => handleSelectAccount(account)}
                        >
                            <div className="account-icon">
                                <Wallet size={24} />
                            </div>
                            <div className="account-info">
                                <h3>{account.name}</h3>
                                <div className="account-meta">
                                    <span className="account-currency">{account.currency}</span>
                                    {account.product && (
                                        <>
                                            <span className="separator">•</span>
                                            <span className="account-product">{account.product}</span>
                                        </>
                                    )}
                                    {account.iban && (
                                        <>
                                            <span className="separator">•</span>
                                            <span className="account-iban">{formatIban(account.iban)}</span>
                                        </>
                                    )}
                                </div>
                            </div>
                            <ChevronRight size={20} className="account-arrow" />
                        </div>
                    ))}
                </div>
            )}
        </PageContainer>
    );
}

/**
 * Format IBAN for display (show last 4 characters)
 */
function formatIban(iban) {
    if (!iban || iban.length < 4) return iban;
    return `•••• ${iban.slice(-4)}`;
}

export default AccountsListPage;
