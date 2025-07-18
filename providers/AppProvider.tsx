// providers/AppProvider.tsx
// Single unified provider for all application state

import React, { ReactNode } from 'react';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import ErrorBoundary from '../components/ErrorBoundary';
import LoadingSpinner from '../components/LoadingSpinner';
import { persistor, store } from '../store/index';

interface AppProviderProps {
  children: ReactNode;
}

/**
 * Single unified provider that wraps the entire application with:
 * - Redux state management with persistence (including theme)
 * - Error boundary for crash protection
 */
export default function AppProvider({ children }: AppProviderProps) {
  return (
    <ErrorBoundary>
      <Provider store={store}>
        <PersistGate 
          loading={<LoadingSpinner message="Loading your data..." />} 
          persistor={persistor}
        >
          {children}
        </PersistGate>
      </Provider>
    </ErrorBoundary>
  );
}
