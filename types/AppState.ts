// types/AppState.ts
// Unified application state interface for reference

import { RootState } from '../store';

/**
 * Complete application state interface
 * This provides a clear overview of all available state in the app
 */
export interface AppState extends RootState {
  // Accounts state - persisted
  accounts: {
    accounts: Array<any>;
    isLoading: boolean;
    error: string | null;
    isInitialized: boolean;
  };
  
  // Metrics state - calculated, not persisted
  metrics: {
    metrics: any | null;
    netWorthHistory: Array<any>;
    isLoading: boolean;
    error: string | null;
    lastUpdated: number;
  };
  
  // UI state - persisted
  ui: {
    selectedTimeRange: string;
    theme: 'light' | 'dark' | 'auto';
    isFirstLaunch: boolean;
  };
}

/**
 * Action types for common state updates
 */
export type AppAction = 
  | { type: 'accounts/add'; payload: any }
  | { type: 'accounts/remove'; payload: string }
  | { type: 'accounts/update'; payload: { accountId: string; updates: any } }
  | { type: 'ui/setTheme'; payload: 'light' | 'dark' | 'auto' }
  | { type: 'ui/setSelectedTimeRange'; payload: string }
  | { type: 'ui/setFirstLaunch'; payload: boolean };
