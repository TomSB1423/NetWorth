// store/effects/AppEffects.tsx
// Side effects and automatic triggers for Redux actions

import { useEffect } from 'react';
import { useAppDispatch, useAccounts, useAccountsInitialized, useMetrics, useHasEverHadAccounts } from '../hooks';
import { initializeAccounts } from '../slices/accountsSlice';
import { calculateMetrics } from '../slices/metricsSlice';
import { setHasEverHadAccounts } from '../slices/uiSlice';

export default function AppEffects() {
  const dispatch = useAppDispatch();
  const accounts = useAccounts();
  const isInitialized = useAccountsInitialized();
  const metrics = useMetrics();
  const hasEverHadAccounts = useHasEverHadAccounts();

  // Initialize accounts on app start
  useEffect(() => {
    if (!isInitialized) {
      dispatch(initializeAccounts());
    }
  }, [dispatch, isInitialized]);

  // Recalculate metrics when accounts change
  useEffect(() => {
    if (isInitialized && accounts.length > 0) {
      dispatch(calculateMetrics());
    }
  }, [dispatch, accounts, isInitialized]);

  // Track when user has ever had accounts
  useEffect(() => {
    if (isInitialized && accounts.length > 0 && !hasEverHadAccounts) {
      dispatch(setHasEverHadAccounts(true));
    }
  }, [dispatch, accounts.length, isInitialized, hasEverHadAccounts]);

  // This component doesn't render anything
  return null;
}
