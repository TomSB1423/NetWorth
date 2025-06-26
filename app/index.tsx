// app/index.tsx
// Entry point that determines onboarding vs main app

import { router } from 'expo-router';
import { useEffect } from 'react';
import LoadingSpinner from '../components/LoadingSpinner';
import { useAccounts, useAccountsInitialized, useAppDispatch, useIsFirstLaunch } from '../hooks';
import { setFirstLaunch } from '../store/slices/uiSlice';

export default function Index() {
  const accounts = useAccounts();
  const isInitialized = useAccountsInitialized();
  const isFirstLaunch = useIsFirstLaunch();
  const dispatch = useAppDispatch();

  useEffect(() => {
    if (!isInitialized) {
      // Still loading, wait
      return;
    }

    // TODO: Add authentication check here when login is implemented
    // Check if user has accounts to determine initial route
    if (accounts.length === 0) {
      // No accounts - always show welcome screen
      router.replace('/onboarding/welcome');
    } else {
      // User has accounts, go to main app
      // Also mark first launch as false since they're an existing user
      if (isFirstLaunch) {
        dispatch(setFirstLaunch(false));
      }
      router.replace('/(tabs)');
    }
  }, [accounts.length, isInitialized, isFirstLaunch, dispatch]);

  // Show loading while determining route
  return <LoadingSpinner message="Loading..." />;
}
