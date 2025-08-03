/**
 * Mock data for the NetWorth application
 */

export const MOCK_NETWORTH_TREND_DATA = [
    { month: 'Jan', value: 445000 },
    { month: 'Feb', value: 452000 },
    { month: 'Mar', value: 460000 },
    { month: 'Apr', value: 468000 },
    { month: 'May', value: 475000 },
    { month: 'Jun', value: 485750 },
];

export const MOCK_ACCOUNTS_DATA = [
    {
        id: 1,
        name: 'Chase Premier Checking',
        type: 'Checking',
        balance: 15420,
        change: 2.3,
        bank: 'Chase',
        lastUpdated: '2 hours ago',
        institutionId: 'chase'
    },
    {
        id: 2,
        name: 'Ally High Yield Savings',
        type: 'Savings',
        balance: 45200,
        change: 0.8,
        bank: 'Ally Bank',
        lastUpdated: '1 day ago',
        institutionId: 'ally'
    },
    {
        id: 3,
        name: 'Fidelity 401(k)',
        type: 'Retirement',
        balance: 187500,
        change: 5.2,
        bank: 'Fidelity',
        lastUpdated: '1 day ago',
        institutionId: 'fidelity'
    },
    {
        id: 4,
        name: 'Vanguard Roth IRA',
        type: 'Retirement',
        balance: 95300,
        change: 4.8,
        bank: 'Vanguard',
        lastUpdated: '1 day ago',
        institutionId: 'vanguard'
    },
    {
        id: 5,
        name: 'Wells Fargo Mortgage',
        type: 'Mortgage',
        balance: -285000,
        change: -0.5,
        bank: 'Wells Fargo',
        lastUpdated: '5 days ago',
        institutionId: 'wellsfargo'
    },
    {
        id: 6,
        name: 'Amex Gold Card',
        type: 'Credit Card',
        balance: -2850,
        change: 15.2,
        bank: 'American Express',
        lastUpdated: '3 hours ago',
        institutionId: 'amex'
    },
];

export const MOCK_INSTITUTIONS_DATA = [
    {
        id: 'chase',
        name: 'JPMorgan Chase',
        logo: '/logos/chase.png',
        type: 'bank',
        isConnected: true,
        lastSync: '2 hours ago',
        accountCount: 1
    },
    {
        id: 'ally',
        name: 'Ally Bank',
        logo: '/logos/ally.png',
        type: 'bank',
        isConnected: true,
        lastSync: '1 day ago',
        accountCount: 1
    },
    {
        id: 'fidelity',
        name: 'Fidelity Investments',
        logo: '/logos/fidelity.png',
        type: 'investment',
        isConnected: true,
        lastSync: '1 day ago',
        accountCount: 1
    },
    {
        id: 'vanguard',
        name: 'Vanguard',
        logo: '/logos/vanguard.png',
        type: 'investment',
        isConnected: true,
        lastSync: '1 day ago',
        accountCount: 1
    },
    {
        id: 'wellsfargo',
        name: 'Wells Fargo',
        logo: '/logos/wellsfargo.png',
        type: 'bank',
        isConnected: true,
        lastSync: '5 days ago',
        accountCount: 1
    },
    {
        id: 'amex',
        name: 'American Express',
        logo: '/logos/amex.png',
        type: 'credit',
        isConnected: true,
        lastSync: '3 hours ago',
        accountCount: 1
    },
];

export const MOCK_INVESTMENTS_DATA = [
    { symbol: 'VTI', name: 'Vanguard Total Stock Market ETF', value: 45200, shares: 180, price: 251.11, change: 2.4, allocation: 35 },
    { symbol: 'VXUS', name: 'Vanguard Total International Stock ETF', value: 28600, shares: 450, price: 63.56, change: -0.8, allocation: 22 },
    { symbol: 'BND', name: 'Vanguard Total Bond Market ETF', value: 19800, shares: 250, price: 79.20, change: 0.3, allocation: 15 },
    { symbol: 'VNQ', name: 'Vanguard Real Estate ETF', value: 15400, shares: 150, price: 102.67, change: 1.8, allocation: 12 },
    { symbol: 'AAPL', name: 'Apple Inc.', value: 12500, shares: 65, price: 192.31, change: 3.2, allocation: 10 },
    { symbol: 'CASH', name: 'Cash & Money Market', value: 8500, shares: 1, price: 8500, change: 0.1, allocation: 6 },
];

export const MOCK_EXPENSES_DATA = [
    { category: 'Housing', amount: 2800, budget: 3000, color: '#3B82F6' },
    { category: 'Transportation', amount: 650, budget: 800, color: '#10B981' },
    { category: 'Food & Dining', amount: 890, budget: 1000, color: '#F59E0B' },
    { category: 'Utilities', amount: 340, budget: 400, color: '#EF4444' },
    { category: 'Healthcare', amount: 280, budget: 350, color: '#8B5CF6' },
    { category: 'Entertainment', amount: 420, budget: 500, color: '#EC4899' },
    { category: 'Shopping', amount: 580, budget: 600, color: '#06B6D4' },
    { category: 'Other', amount: 230, budget: 300, color: '#84CC16' },
];

export const MOCK_MONTHLY_EXPENSES_DATA = [
    { month: 'Jan', amount: 6200 },
    { month: 'Feb', amount: 6800 },
    { month: 'Mar', amount: 6500 },
    { month: 'Apr', amount: 6900 },
    { month: 'May', amount: 6100 },
    { month: 'Jun', amount: 6190 },
];

export const MOCK_GOALS_DATA = [
    { id: 1, name: 'Emergency Fund', target: 30000, current: 25200, deadline: '2024-12-31', category: 'Safety Net', color: '#3B82F6' },
    { id: 2, name: 'House Down Payment', target: 100000, current: 45000, deadline: '2025-06-30', category: 'Real Estate', color: '#10B981' },
    { id: 3, name: 'Vacation Fund', target: 8000, current: 3200, deadline: '2024-07-15', category: 'Lifestyle', color: '#F59E0B' },
    { id: 4, name: 'New Car', target: 35000, current: 12500, deadline: '2025-03-01', category: 'Transportation', color: '#EF4444' },
];

export const MOCK_TRANSACTIONS_DATA = [
    { id: 1, date: '2024-08-03', description: 'Salary Deposit', amount: 8500, category: 'Income', account: 'Chase Checking', type: 'credit' },
    { id: 2, date: '2024-08-02', description: 'Rent Payment', amount: -2800, category: 'Housing', account: 'Chase Checking', type: 'debit' },
    { id: 3, date: '2024-08-02', description: 'Grocery Store', amount: -125.50, category: 'Food', account: 'Chase Checking', type: 'debit' },
    { id: 4, date: '2024-08-01', description: 'Investment Transfer', amount: -2000, category: 'Investment', account: 'Chase Checking', type: 'debit' },
    { id: 5, date: '2024-08-01', description: 'Gas Station', amount: -45.20, category: 'Transportation', account: 'Amex Gold', type: 'debit' },
    { id: 6, date: '2024-07-31', description: 'Dividend Payment', amount: 285.50, category: 'Investment', account: 'Fidelity 401k', type: 'credit' },
    { id: 7, date: '2024-07-30', description: 'Restaurant', amount: -89.75, category: 'Dining', account: 'Amex Gold', type: 'debit' },
    { id: 8, date: '2024-07-30', description: 'Electric Bill', amount: -156.80, category: 'Utilities', account: 'Chase Checking', type: 'debit' },
];
