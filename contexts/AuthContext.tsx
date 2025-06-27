// contexts/AuthContext.tsx
// Authentication context following Expo Router patterns

import { router, useSegments } from 'expo-router';
import React, { createContext, useContext, useEffect } from 'react';
import { useAccounts, useAccountsInitialized, useAppDispatch } from '../store/hooks';
import { initializeAccounts } from '../store/slices/accountsSlice';

interface AuthContextType {
  hasAccounts: boolean;
  isInitialized: boolean;
}

const AuthContext = createContext<AuthContextType>({
  hasAccounts: false,
  isInitialized: false,
});

export function useAuth() {
  return useContext(AuthContext);
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const dispatch = useAppDispatch();
  const accounts = useAccounts();
  const isInitialized = useAccountsInitialized();
  const segments = useSegments();

  const hasAccounts = accounts.length > 0;

  // Initialize accounts on app start
  useEffect(() => {
    if (!isInitialized) {
      dispatch(initializeAccounts());
    }
  }, [dispatch, isInitialized]);

  // Handle navigation based on authentication state
  useEffect(() => {
    if (!isInitialized) return;

    const inTabsGroup = segments[0] === '(tabs)';
    const inOnboarding = segments[0] === 'onboarding';
    const isAddingAccount = segments[0] === 'add-account' || segments[0] === 'select-bank';

    if (hasAccounts && !inTabsGroup && !isAddingAccount) {
      // User has accounts but not in tabs and not adding account -> redirect to dashboard
      router.replace('/(tabs)/dashboard');
    } else if (!hasAccounts && !inOnboarding && !isAddingAccount) {
      // User has no accounts and not in onboarding flow -> redirect to onboarding
      router.replace('/onboarding/welcome');
    }
  }, [hasAccounts, isInitialized, segments]);

  return (
    <AuthContext.Provider value={{ hasAccounts, isInitialized }}>
      {children}
    </AuthContext.Provider>
  );
}
