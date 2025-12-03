import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { accountService } from '../services/api';
import { LoadingSpinner } from '../components';
import { PageHeader, PageContainer } from '../components/layout';
import { Wallet, TrendingUp, Calendar } from 'lucide-react';
import './AccountDetailsPage.css';

/**
 * AccountDetailsPage - Detailed view of a single bank account with balances and transactions
 */
function AccountDetailsPage() {
    const { accountId } = useParams();
    const navigate = useNavigate();
    const [account, setAccount] = useState(null);
    const [details, setDetails] = useState(null);
    const [balances, setBalances] = useState([]);
    const [transactions, setTransactions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [activeTab, setActiveTab] = useState('overview');

    const loadAccountData = async () => {
        if (!accountId) return;
        
        try {
            setLoading(true);
            setError(null);

            // Load all account data in parallel
            const [accountData, detailsData, balancesData] = await Promise.all([
                accountService.getAccount(accountId),
                accountService.getAccountDetails(accountId).catch(() => null),
                accountService.getAccountBalances(accountId).catch(() => []),
            ]);

            setAccount(accountData);
            setDetails(detailsData);
            setBalances(balancesData);

            // Load transactions (last 90 days)
            const dateTo = new Date().toISOString().split('T')[0];
            const dateFrom = new Date(Date.now() - 90 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

            try {
                const transactionsData = await accountService.getAccountTransactions(
                    accountId,
                    dateFrom,
                    dateTo
                );
                setTransactions(transactionsData || []);
            } catch (err) {
                console.warn('Failed to load transactions:', err);
            }

        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadAccountData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [accountId]);

    if (loading) {
        return (
            <PageContainer>
                <LoadingSpinner text="Loading account details..." size="large" />
            </PageContainer>
        );
    }

    if (error || !account) {
        return (
            <PageContainer>
                <div className="error-state">
                    <h3>Failed to Load Account</h3>
                    <p>{error || 'Account not found'}</p>
                    <button onClick={() => navigate('/accounts/list')} className="btn-primary">
                        Back to Accounts
                    </button>
                </div>
            </PageContainer>
        );
    }

    const displayName = details?.displayName || details?.name || account.name;
    const primaryBalance = balances.find(b => b.balanceType === 'expected') || balances[0];

    return (
        <PageContainer maxWidth="large">
            <PageHeader
                title={displayName}
                subtitle={details?.product || account.currency}
                showBack={true}
                backTo="/accounts/list"
                onRefresh={loadAccountData}
                refreshing={loading}
            />

            <div className="account-hero">
                <div className="account-hero-icon">
                    <Wallet size={32} />
                </div>
                <div className="account-hero-info">
                    <h1>{displayName}</h1>
                    <div className="account-hero-meta">
                        {details?.product && <span>{details.product}</span>}
                        {details?.product && account.currency && <span className="separator">•</span>}
                        {account.currency && <span>{account.currency}</span>}
                        {account.iban && (
                            <>
                                <span className="separator">•</span>
                                <span>{account.iban}</span>
                            </>
                        )}
                    </div>
                </div>
                {primaryBalance && (
                    <div className="account-balance-display">
                        <div className="balance-label">Current Balance</div>
                        <div className="balance-amount">
                            {formatCurrency(primaryBalance.amount, primaryBalance.currency)}
                        </div>
                        <div className="balance-type">{primaryBalance.balanceType}</div>
                    </div>
                )}
            </div>

            <div className="tabs">
                <button
                    className={`tab ${activeTab === 'overview' ? 'active' : ''}`}
                    onClick={() => setActiveTab('overview')}
                >
                    Overview
                </button>
                <button
                    className={`tab ${activeTab === 'transactions' ? 'active' : ''}`}
                    onClick={() => setActiveTab('transactions')}
                >
                    Transactions
                </button>
            </div>

            {activeTab === 'overview' && (
                <div className="tab-content">
                    {balances.length > 0 && (
                        <div className="card">
                            <h2>Balances</h2>
                            <div className="balances-grid">
                                {balances.map((balance, index) => (
                                    <div key={index} className="balance-item">
                                        <div className="balance-item-icon">
                                            <TrendingUp size={20} />
                                        </div>
                                        <div className="balance-item-info">
                                            <div className="balance-item-type">{balance.balanceType}</div>
                                            <div className="balance-item-amount">
                                                {formatCurrency(balance.amount, balance.currency)}
                                            </div>
                                            {balance.referenceDate && (
                                                <div className="balance-item-date">
                                                    As of {formatDate(balance.referenceDate)}
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}

                    {details && (
                        <div className="card">
                            <h2>Account Details</h2>
                            <div className="details-grid">
                                {details.cashAccountType && (
                                    <div className="detail-item">
                                        <span className="detail-label">Account Type</span>
                                        <span className="detail-value">{details.cashAccountType}</span>
                                    </div>
                                )}
                                {details.status && (
                                    <div className="detail-item">
                                        <span className="detail-label">Status</span>
                                        <span className="detail-value status">{details.status}</span>
                                    </div>
                                )}
                                {details.currency && (
                                    <div className="detail-item">
                                        <span className="detail-label">Currency</span>
                                        <span className="detail-value">{details.currency}</span>
                                    </div>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            )}

            {activeTab === 'transactions' && (
                <div className="tab-content">
                    <div className="card">
                        <h2>Recent Transactions</h2>
                        {transactions.length === 0 ? (
                            <div className="empty-transactions">
                                <Calendar size={48} />
                                <p>No transactions found</p>
                            </div>
                        ) : (
                            <div className="transactions-list">
                                {transactions.map((transaction, index) => (
                                    <div key={index} className="transaction-item">
                                        <div className="transaction-info">
                                            <div className="transaction-description">
                                                {transaction.remittanceInformationUnstructured ||
                                                    transaction.creditorName ||
                                                    transaction.debtorName ||
                                                    'Transaction'}
                                            </div>
                                            <div className="transaction-date">
                                                {formatDate(transaction.bookingDate || transaction.valueDate)}
                                            </div>
                                        </div>
                                        <div className={`transaction-amount ${parseFloat(transaction.amount) >= 0 ? 'positive' : 'negative'}`}>
                                            {formatCurrency(transaction.amount, transaction.currency)}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            )}
        </PageContainer>
    );
}

/**
 * Format currency for display
 */
function formatCurrency(amount, currency) {
    const value = parseFloat(amount);
    return new Intl.NumberFormat('en-GB', {
        style: 'currency',
        currency: currency || 'GBP',
    }).format(value);
}

/**
 * Format date for display
 */
function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-GB', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    }).format(date);
}

export default AccountDetailsPage;
