// providers/AppProvider.tsx
// Single unified provider for all application state

import React, { ReactNode } from 'react';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from '../store/index';
import { ColorSchemeProvider } from '../hooks/ColorSchemeContext';
import AppEffects from '../store/effects/AppEffects';
import ErrorBoundary from '../components/ErrorBoundary';
import LoadingSpinner from '../components/LoadingSpinner';

interface AppProviderProps {
  children: ReactNode;
}

/**
 * Single unified provider that wraps the entire application with:
 * - Redux state management with persistence
 * - Color scheme/theme management
 * - Error boundary for crash protection
 * - App-wide side effects
 */
export default function AppProvider({ children }: AppProviderProps) {
  return (
    <ErrorBoundary>
      <Provider store={store}>
        <PersistGate 
          loading={<LoadingSpinner message="Loading your data..." />} 
          persistor={persistor}
        >
          <ColorSchemeProvider>
            <AppEffects />
            {children}
          </ColorSchemeProvider>
        </PersistGate>
      </Provider>
    </ErrorBoundary>
  );
}
