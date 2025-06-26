// store/effects/AppEffects.tsx
// Side effects and automatic triggers for Redux actions

import { useEffect } from 'react';
import { useAccounts, useAccountsInitialized, useAppDispatch, useMetrics } from '../hooks';
import { initializeAccounts } from '../slices/accountsSlice';
import { calculateMetrics } from '../slices/metricsSlice';

export default function AppEffects() {
  const dispatch = useAppDispatch();
  const accounts = useAccounts();
  const isInitialized = useAccountsInitialized();
  const metrics = useMetrics();

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

  // This component doesn't render anything
  return null;
}
