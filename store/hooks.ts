// store/hooks.ts
// Typed hooks for Redux usage

import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import type { AppDispatch, RootState } from './index';

// Use throughout the app instead of plain `useDispatch` and `useSelector`
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

// Custom hooks for specific state slices
export const useAccounts = () => useAppSelector(state => state.accounts.accounts);
export const useAccountsLoading = () => useAppSelector(state => state.accounts.isLoading);
export const useAccountsError = () => useAppSelector(state => state.accounts.error);
export const useAccountsInitialized = () => useAppSelector(state => state.accounts.isInitialized);

export const useMetrics = () => useAppSelector(state => state.metrics.metrics);
export const useNetWorthHistory = () => useAppSelector(state => state.metrics.netWorthHistory);
export const useMetricsLoading = () => useAppSelector(state => state.metrics.isLoading);
export const useMetricsError = () => useAppSelector(state => state.metrics.error);

export const useSelectedTimeRange = () => useAppSelector(state => state.ui.selectedTimeRange);
export const useTheme = () => useAppSelector(state => state.ui.theme);
export const useIsFirstLaunch = () => useAppSelector(state => state.ui.isFirstLaunch);

// Combined selectors for convenience
export const useAppLoading = () => {
  const accountsLoading = useAccountsLoading();
  const metricsLoading = useMetricsLoading();
  return accountsLoading || metricsLoading;
};

export const useAppError = () => {
  const accountsError = useAccountsError();
  const metricsError = useMetricsError();
  return accountsError || metricsError;
};

// Account-specific selectors
export const useAccount = (accountId: string) => 
  useAppSelector(state => state.accounts.accounts.find(account => account.id === accountId) || null);

export const useTotalNetWorth = () => 
  useAppSelector(state => {
    return state.accounts.accounts.reduce((total, account) => {
      const balance = account.transactions.reduce((sum, transaction) => sum + transaction.amount, 0);
      // For debt accounts (credit, mortgage), negative balance means we owe money
      if (account.type === 'credit' || account.type === 'mortgage') {
        return total - Math.abs(balance); // Subtract debt from net worth
      }
      return total + balance;
    }, 0);
  });
