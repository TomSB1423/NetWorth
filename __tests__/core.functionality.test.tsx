import { configureStore } from '@reduxjs/toolkit';
import { render } from '@testing-library/react-native';
import React from 'react';
import { Provider } from 'react-redux';
import Dashboard from '../app/(tabs)/dashboard';
import AccountsList from '../components/AccountsList';
import MetricsCard from '../components/MetricsCard';
import NetWorthChart from '../components/NetWorthChart';
import { Account, AccountType } from '../services/accountMockService';
import accountsSlice from '../store/slices/accountsSlice';
import metricsSlice from '../store/slices/metricsSlice';
import uiSlice from '../store/slices/uiSlice';

// Mock the hooks to return consistent test data
jest.mock('../hooks', () => ({
  useAccounts: jest.fn(() => [
    { id: '1', name: 'Chase Checking', type: 'checking', balance: 5000, transactions: [] },
    { id: '2', name: 'Savings', type: 'savings', balance: 15000, transactions: [] }
  ]),
  useMetrics: jest.fn(() => ({
    totalBalance: 20000,
    monthlyIncome: 8000,
    monthlyExpenses: 6000,
    savingsRate: 25,
    averageTransactionSize: 125,
    largestExpense: 2500,
    monthlyFlow: [
      { month: 'Jan', income: 8000, expenses: 6000, net: 2000 },
      { month: 'Feb', income: 8200, expenses: 5800, net: 2400 }
    ],
    expenseCategories: [
      { category: 'Food', amount: 2000, percentage: 33 }
    ],
    incomeCategories: [
      { category: 'Salary', amount: 8000, percentage: 100 }
    ]
  })),
  useNetWorthHistory: jest.fn(() => [
    { date: '2024-01-01', value: 18000 },
    { date: '2024-02-01', value: 19000 },
    { date: '2024-03-01', value: 20000 }
  ]),
  useAppLoading: jest.fn(() => false),
  useSelectedTimeRange: jest.fn(() => '3m'),
  useAppDispatch: jest.fn(() => jest.fn()),
  usePalette: jest.fn(() => ({
    primary: '#007AFF',
    background: '#FFFFFF',
    card: '#F2F2F7',
    text: '#000000',
    secondaryText: '#8E8E93',
    success: '#34C759',
    error: '#FF3B30'
  }))
}));

// Create a simple test store
const createTestStore = () => {
  return configureStore({
    reducer: {
      accounts: accountsSlice,
      metrics: metricsSlice,
      ui: uiSlice,
    }
  });
};

// Test wrapper with Redux provider
const TestWrapper = ({ children }: { children: React.ReactNode }) => (
  <Provider store={createTestStore()}>
    {children}
  </Provider>
);

describe('Core App Functionality', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Dashboard Component', () => {
    it('renders without crashing', () => {
      const { getByText } = render(
        <TestWrapper>
          <Dashboard />
        </TestWrapper>
      );
      
      // Check if component renders basic elements
      expect(getByText('Net Worth')).toBeTruthy();
    });

    it('displays key financial metrics', () => {
      const { getByText } = render(
        <TestWrapper>
          <Dashboard />
        </TestWrapper>
      );
      
      // Check for metrics cards
      expect(getByText('Total Balance')).toBeTruthy();
      expect(getByText('Monthly Income')).toBeTruthy();
      expect(getByText('Monthly Expenses')).toBeTruthy();
    });

    it('handles loading state gracefully', () => {
      // Mock loading state
      require('../hooks').useAppLoading.mockReturnValueOnce(true);

      const { getByText } = render(
        <TestWrapper>
          <Dashboard />
        </TestWrapper>
      );

      expect(getByText('Net Worth')).toBeTruthy();
    });

    it('handles empty accounts state', () => {
      // Mock empty accounts
      require('../hooks').useAccounts.mockReturnValueOnce([]);
      require('../hooks').useMetrics.mockReturnValueOnce({
        totalBalance: 0,
        monthlyIncome: 0,
        monthlyExpenses: 0,
        savingsRate: 0,
        averageTransactionSize: 0,
        largestExpense: 0,
        monthlyFlow: [],
        expenseCategories: [],
        incomeCategories: []
      });

      const { getByText } = render(
        <TestWrapper>
          <Dashboard />
        </TestWrapper>
      );

      expect(getByText('Net Worth')).toBeTruthy();
    });
  });

  describe('NetWorth Chart Component', () => {
    const mockData = [
      { date: '2024-01-01', value: 18000 },
      { date: '2024-02-01', value: 19000 },
      { date: '2024-03-01', value: 20000 }
    ];

    it('renders with valid data', () => {
      const { getByText } = render(
        <TestWrapper>
          <NetWorthChart data={mockData} selectedRange="3m" />
        </TestWrapper>
      );

      // Should display current net worth value
      expect(getByText('$20,000.00')).toBeTruthy();
    });

    it('handles empty data gracefully', () => {
      const { getByText } = render(
        <TestWrapper>
          <NetWorthChart data={[]} selectedRange="3m" />
        </TestWrapper>
      );

      // Should render without crashing even with empty data
      expect(getByText('$0.00')).toBeTruthy();
    });

    it('formats large currency values correctly', () => {
      const largeValueData = [
        { date: '2024-01-01', value: 1500000 }
      ];

      const { getByText } = render(
        <TestWrapper>
          <NetWorthChart data={largeValueData} selectedRange="3m" />
        </TestWrapper>
      );

      expect(getByText('$1,500,000.00')).toBeTruthy();
    });
  });

  describe('AccountsList Component', () => {
    const mockAccounts: Account[] = [
      { id: '1', name: 'Chase Checking', type: 'checking' as AccountType, transactions: [] },
      { id: '2', name: 'Savings', type: 'savings' as AccountType, transactions: [] }
    ];

    it('displays account information correctly', () => {
      const { getByText } = render(
        <TestWrapper>
          <AccountsList accounts={mockAccounts} />
        </TestWrapper>
      );

      expect(getByText('Chase Checking')).toBeTruthy();
      expect(getByText('Savings')).toBeTruthy();
    });

    it('handles empty accounts list', () => {
      const { getByText } = render(
        <TestWrapper>
          <AccountsList accounts={[]} />
        </TestWrapper>
      );

      // Should render empty state message
      expect(getByText('No accounts added yet')).toBeTruthy();
    });

    it('displays different account types', () => {
      const mixedAccounts: Account[] = [
        { id: '1', name: 'Checking', type: 'checking' as AccountType, transactions: [] },
        { id: '2', name: 'Credit Card', type: 'credit' as AccountType, transactions: [] },
        { id: '3', name: 'Investment', type: 'investment' as AccountType, transactions: [] }
      ];

      const { getByText } = render(
        <TestWrapper>
          <AccountsList accounts={mixedAccounts} />
        </TestWrapper>
      );

      expect(getByText('Checking')).toBeTruthy();
      expect(getByText('Credit Card')).toBeTruthy();
      expect(getByText('Investment')).toBeTruthy();
    });
  });

  describe('MetricsCard Component', () => {
    it('displays metric value and label', () => {
      const { getByText } = render(
        <TestWrapper>
          <MetricsCard
            title="Total Balance"
            value="$20,000.00"
            icon="💰"
            trend={{ direction: 'up', percentage: 2.5 }}
          />
        </TestWrapper>
      );

      expect(getByText('Total Balance')).toBeTruthy();
      expect(getByText('$20,000.00')).toBeTruthy();
    });

    it('handles positive trend correctly', () => {
      const { getByText } = render(
        <TestWrapper>
          <MetricsCard
            title="Total Balance"
            value="$20,000.00"
            icon="💰"
            trend={{ direction: 'up', percentage: 2.5 }}
          />
        </TestWrapper>
      );

      expect(getByText('2.5%')).toBeTruthy();
    });

    it('handles negative trend correctly', () => {
      const { getByText } = render(
        <TestWrapper>
          <MetricsCard
            title="Total Balance"
            value="$20,000.00"
            icon="💰"
            trend={{ direction: 'down', percentage: 1.2 }}
          />
        </TestWrapper>
      );

      expect(getByText('1.2%')).toBeTruthy();
    });

    it('handles neutral trend', () => {
      const { getByText } = render(
        <TestWrapper>
          <MetricsCard
            title="Total Balance"
            value="$20,000.00"
            icon="💰"
            trend={{ direction: 'neutral', percentage: 0 }}
          />
        </TestWrapper>
      );

      expect(getByText('0%')).toBeTruthy();
    });
  });
});

describe('Integration Tests', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('displays correct total balance from multiple accounts', () => {
    // Mock multiple accounts with different transactions
    require('../hooks').useAccounts.mockReturnValueOnce([
      { id: '1', name: 'Checking', type: 'checking', transactions: [] },
      { id: '2', name: 'Savings', type: 'savings', transactions: [] },
      { id: '3', name: 'Credit', type: 'credit', transactions: [] }
    ]);
    require('../hooks').useMetrics.mockReturnValueOnce({
      totalBalance: 5500, // Calculated from transactions
      monthlyIncome: 8000,
      monthlyExpenses: 6000,
      savingsRate: 25,
      averageTransactionSize: 125,
      largestExpense: 2500,
      monthlyFlow: [],
      expenseCategories: [],
      incomeCategories: []
    });

    const { getByText } = render(
      <TestWrapper>
        <Dashboard />
      </TestWrapper>
    );

    expect(getByText('$5,500.00')).toBeTruthy();
  });

  it('handles time range changes', () => {
    require('../hooks').useSelectedTimeRange.mockReturnValueOnce('1y');

    const { getByText } = render(
      <TestWrapper>
        <Dashboard />
      </TestWrapper>
    );

    // Should display current selected range
    expect(getByText('1Y')).toBeTruthy();
  });

  it('handles error states gracefully', () => {
    // Mock error state
    require('../hooks').useAccounts.mockReturnValueOnce([]);
    require('../hooks').useMetrics.mockReturnValueOnce({
      totalBalance: 0,
      monthlyIncome: 0,
      monthlyExpenses: 0,
      savingsRate: 0,
      averageTransactionSize: 0,
      largestExpense: 0,
      monthlyFlow: [],
      expenseCategories: [],
      incomeCategories: []
    });

    const { getByText } = render(
      <TestWrapper>
        <Dashboard />
      </TestWrapper>
    );

    // Should still render the main components
    expect(getByText('Net Worth')).toBeTruthy();
  });
});
