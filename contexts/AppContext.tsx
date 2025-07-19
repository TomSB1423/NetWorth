// contexts/AppContext.tsx
// Global state management using React Context with error boundaries and security

import { createContext, ReactNode, useContext, useEffect, useReducer } from 'react';
import { Account, Transaction } from '../services/accountMockService';
import { MetricsData, MetricsService } from '../services/metricsService';
import { NetWorthEntry } from '../services/netWorthService';
import {
    generateSecureId,
    isValidAccountName,
    isValidAmount,
    isValidTransactionDate,
    rateLimiter,
    sanitizeString
} from '../utils/security';

// Action types for type safety
export enum ActionType {
  INITIALIZE_DATA = 'INITIALIZE_DATA',
  ADD_ACCOUNT = 'ADD_ACCOUNT',
  UPDATE_ACCOUNT = 'UPDATE_ACCOUNT',
  REMOVE_ACCOUNT = 'REMOVE_ACCOUNT',
  ADD_TRANSACTION = 'ADD_TRANSACTION',
  SET_LOADING = 'SET_LOADING',
  SET_ERROR = 'SET_ERROR',
  CLEAR_ERROR = 'CLEAR_ERROR',
  REFRESH_METRICS = 'REFRESH_METRICS',
}

// State interface
export interface AppState {
  accounts: Account[];
  netWorthHistory: NetWorthEntry[];
  metrics: MetricsData | null;
  isLoading: boolean;
  error: string | null;
  isInitialized: boolean;
}

// Action interfaces
interface InitializeDataAction {
  type: ActionType.INITIALIZE_DATA;
  payload: {
    accounts: Account[];
    netWorthHistory: NetWorthEntry[];
    metrics: MetricsData;
  };
}

interface AddAccountAction {
  type: ActionType.ADD_ACCOUNT;
  payload: Account;
}

interface UpdateAccountAction {
  type: ActionType.UPDATE_ACCOUNT;
  payload: {
    accountId: string;
    updates: Partial<Omit<Account, 'id' | 'transactions'>>;
  };
}

interface RemoveAccountAction {
  type: ActionType.REMOVE_ACCOUNT;
  payload: string; // accountId
}

interface AddTransactionAction {
  type: ActionType.ADD_TRANSACTION;
  payload: {
    accountId: string;
    transaction: Transaction;
  };
}

interface SetLoadingAction {
  type: ActionType.SET_LOADING;
  payload: boolean;
}

interface SetErrorAction {
  type: ActionType.SET_ERROR;
  payload: string;
}

interface ClearErrorAction {
  type: ActionType.CLEAR_ERROR;
}

interface RefreshMetricsAction {
  type: ActionType.REFRESH_METRICS;
  payload: MetricsData;
}

type AppAction = 
  | InitializeDataAction
  | AddAccountAction
  | UpdateAccountAction
  | RemoveAccountAction
  | AddTransactionAction
  | SetLoadingAction
  | SetErrorAction
  | ClearErrorAction
  | RefreshMetricsAction;

// Initial state
const initialState: AppState = {
  accounts: [],
  netWorthHistory: [],
  metrics: null,
  isLoading: true,
  error: null,
  isInitialized: false,
};

// Input validation functions
const validateAccount = (account: Partial<Account>): string | null => {
  if (!account.name) {
    return 'Account name is required';
  }
  
  const sanitizedName = sanitizeString(account.name);
  if (!isValidAccountName(sanitizedName)) {
    return 'Account name contains invalid characters or is too long';
  }
  
  if (!account.type) {
    return 'Account type is required';
  }
  
  if (account.creditLimit !== undefined) {
    if (!isValidAmount(account.creditLimit)) {
      return 'Credit limit must be a valid number';
    }
    if (account.creditLimit < 0 || account.creditLimit > 1000000) {
      return 'Credit limit must be between $0 and $1,000,000';
    }
  }
  
  if (account.interestRate !== undefined) {
    if (!isValidAmount(account.interestRate)) {
      return 'Interest rate must be a valid number';
    }
    if (account.interestRate < 0 || account.interestRate > 1) {
      return 'Interest rate must be between 0% and 100%';
    }
  }
  
  return null;
};

const validateTransaction = (transaction: Partial<Transaction>): string | null => {
  if (!transaction.description) {
    return 'Transaction description is required';
  }
  
  const sanitizedDescription = sanitizeString(transaction.description);
  if (sanitizedDescription.length === 0) {
    return 'Transaction description is required';
  }
  if (sanitizedDescription.length > 200) {
    return 'Transaction description must be less than 200 characters';
  }
  
  if (transaction.amount === undefined || transaction.amount === null) {
    return 'Transaction amount is required';
  }
  if (!isValidAmount(transaction.amount)) {
    return 'Transaction amount must be a valid number';
  }
  if (Math.abs(transaction.amount) > 1000000) {
    return 'Transaction amount must be less than $1,000,000';
  }
  
  if (!transaction.date) {
    return 'Transaction date is required';
  }
  if (!isValidTransactionDate(transaction.date)) {
    return 'Transaction date is invalid or in the future';
  }
  
  return null;
};

// Reducer function with error handling
function appReducer(state: AppState, action: AppAction): AppState {
  try {
    switch (action.type) {
      case ActionType.INITIALIZE_DATA:
        return {
          ...state,
          accounts: action.payload.accounts,
          netWorthHistory: action.payload.netWorthHistory,
          metrics: action.payload.metrics,
          isLoading: false,
          error: null,
          isInitialized: true,
        };

      case ActionType.ADD_ACCOUNT:
        // Validate account data
        const accountError = validateAccount(action.payload);
        if (accountError) {
          return {
            ...state,
            error: accountError,
          };
        }

        // Check for duplicate account names (case-insensitive)
        const sanitizedName = sanitizeString(action.payload.name);
        const isDuplicateName = state.accounts.some(
          account => sanitizeString(account.name).toLowerCase() === sanitizedName.toLowerCase()
        );
        if (isDuplicateName) {
          return {
            ...state,
            error: 'An account with this name already exists',
          };
        }

        return {
          ...state,
          accounts: [...state.accounts, action.payload],
          error: null,
        };

      case ActionType.UPDATE_ACCOUNT:
        const { accountId, updates } = action.payload;
        
        // Validate updates
        const updateError = validateAccount(updates);
        if (updateError) {
          return {
            ...state,
            error: updateError,
          };
        }

        // Check for duplicate names (excluding current account)
        if (updates.name) {
          const sanitizedUpdateName = sanitizeString(updates.name);
          const isDuplicateUpdateName = state.accounts.some(
            account => account.id !== accountId && 
            sanitizeString(account.name).toLowerCase() === sanitizedUpdateName.toLowerCase()
          );
          if (isDuplicateUpdateName) {
            return {
              ...state,
              error: 'An account with this name already exists',
            };
          }
        }

        return {
          ...state,
          accounts: state.accounts.map(account =>
            account.id === accountId ? { ...account, ...updates } : account
          ),
          error: null,
        };

      case ActionType.REMOVE_ACCOUNT:
        return {
          ...state,
          accounts: state.accounts.filter(account => account.id !== action.payload),
          error: null,
        };

      case ActionType.ADD_TRANSACTION:
        const { accountId: transAccountId, transaction } = action.payload;
        
        // Validate transaction
        const transactionError = validateTransaction(transaction);
        if (transactionError) {
          return {
            ...state,
            error: transactionError,
          };
        }

        return {
          ...state,
          accounts: state.accounts.map(account =>
            account.id === transAccountId
              ? { ...account, transactions: [...account.transactions, transaction] }
              : account
          ),
          error: null,
        };

      case ActionType.SET_LOADING:
        return {
          ...state,
          isLoading: action.payload,
        };

      case ActionType.SET_ERROR:
        return {
          ...state,
          error: action.payload,
          isLoading: false,
        };

      case ActionType.CLEAR_ERROR:
        return {
          ...state,
          error: null,
        };

      case ActionType.REFRESH_METRICS:
        return {
          ...state,
          metrics: action.payload,
        };

      default:
        console.warn('Unknown action type:', (action as any).type);
        return state;
    }
  } catch (error) {
    console.error('Error in appReducer:', error);
    return {
      ...state,
      error: error instanceof Error ? error.message : 'An unexpected error occurred',
      isLoading: false,
    };
  }
}

// Context interfaces
interface AppContextType {
  state: AppState;
  actions: {
    addAccount: (accountData: Omit<Account, 'id' | 'transactions'>) => Promise<void>;
    updateAccount: (accountId: string, updates: Partial<Omit<Account, 'id' | 'transactions'>>) => Promise<void>;
    removeAccount: (accountId: string) => Promise<void>;
    addTransaction: (accountId: string, transaction: Omit<Transaction, 'id'>) => Promise<void>;
    clearError: () => void;
    refreshData: () => Promise<void>;
  };
}

// Create contexts
const AppContext = createContext<AppContextType | undefined>(undefined);

// Provider component props
interface AppProviderProps {
  children: ReactNode;
}

// Provider component
export function AppProvider({ children }: AppProviderProps) {
  const [state, dispatch] = useReducer(appReducer, initialState);

  // Initialize data on mount
  useEffect(() => {
    initializeData();
  }, []);

  // Refresh metrics when accounts change
  useEffect(() => {
    if (state.accounts.length > 0 && state.isInitialized) {
      refreshMetrics();
    }
  }, [state.accounts, state.isInitialized]);

  const initializeData = async () => {
    try {
      dispatch({ type: ActionType.SET_LOADING, payload: true });
      
      // Import services dynamically to avoid circular dependencies
      const { getNetWorthService } = await import('../services/serviceProvider');
      
      const netWorthService = getNetWorthService();
      const accounts = netWorthService.accountService.getAccounts();
      const netWorthHistory = netWorthService.getNetWorthHistory(365);
      
      const metricsService = new MetricsService(accounts);
      const metrics = metricsService.getMetrics();

      dispatch({
        type: ActionType.INITIALIZE_DATA,
        payload: { accounts, netWorthHistory, metrics },
      });
    } catch (error) {
      console.error('Failed to initialize data:', error);
      dispatch({
        type: ActionType.SET_ERROR,
        payload: error instanceof Error ? error.message : 'Failed to initialize application data',
      });
    }
  };

  const refreshMetrics = async () => {
    try {
      if (state.accounts.length === 0) return;
      
      const metricsService = new MetricsService(state.accounts);
      const metrics = metricsService.getMetrics();
      
      dispatch({ type: ActionType.REFRESH_METRICS, payload: metrics });
    } catch (error) {
      console.error('Failed to refresh metrics:', error);
    }
  };

  const addAccount = async (accountData: Omit<Account, 'id' | 'transactions'>): Promise<void> => {
    try {
      // Rate limiting check
      if (!rateLimiter.isAllowed('addAccount', 10, 60000)) {
        throw new Error('Too many account creation attempts. Please wait a moment.');
      }
      
      // Generate a secure ID
      const newId = generateSecureId();
      
      const newAccount: Account = {
        ...accountData,
        id: newId,
        transactions: [],
        name: sanitizeString(accountData.name), // Sanitize input
      };

      dispatch({ type: ActionType.ADD_ACCOUNT, payload: newAccount });
    } catch (error) {
      console.error('Failed to add account:', error);
      dispatch({
        type: ActionType.SET_ERROR,
        payload: error instanceof Error ? error.message : 'Failed to add account',
      });
      throw error;
    }
  };

  const updateAccount = async (
    accountId: string,
    updates: Partial<Omit<Account, 'id' | 'transactions'>>
  ): Promise<void> => {
    try {
      // Sanitize string inputs
      const sanitizedUpdates = {
        ...updates,
        ...(updates.name && { name: sanitizeString(updates.name) }),
      };

      dispatch({
        type: ActionType.UPDATE_ACCOUNT,
        payload: { accountId, updates: sanitizedUpdates },
      });
    } catch (error) {
      console.error('Failed to update account:', error);
      dispatch({
        type: ActionType.SET_ERROR,
        payload: error instanceof Error ? error.message : 'Failed to update account',
      });
      throw error;
    }
  };

  const removeAccount = async (accountId: string): Promise<void> => {
    try {
      dispatch({ type: ActionType.REMOVE_ACCOUNT, payload: accountId });
    } catch (error) {
      console.error('Failed to remove account:', error);
      dispatch({
        type: ActionType.SET_ERROR,
        payload: error instanceof Error ? error.message : 'Failed to remove account',
      });
      throw error;
    }
  };

  const addTransaction = async (
    accountId: string,
    transactionData: Omit<Transaction, 'id'>
  ): Promise<void> => {
    try {
      // Rate limiting check
      if (!rateLimiter.isAllowed(`addTransaction_${accountId}`, 20, 60000)) {
        throw new Error('Too many transaction creation attempts. Please wait a moment.');
      }
      
      // Generate a secure ID
      const newId = generateSecureId();
      
      const newTransaction: Transaction = {
        ...transactionData,
        id: newId,
        description: sanitizeString(transactionData.description), // Sanitize input
      };

      dispatch({
        type: ActionType.ADD_TRANSACTION,
        payload: { accountId, transaction: newTransaction },
      });
    } catch (error) {
      console.error('Failed to add transaction:', error);
      dispatch({
        type: ActionType.SET_ERROR,
        payload: error instanceof Error ? error.message : 'Failed to add transaction',
      });
      throw error;
    }
  };

  const clearError = () => {
    dispatch({ type: ActionType.CLEAR_ERROR });
  };

  const refreshData = async (): Promise<void> => {
    await initializeData();
  };

  const contextValue: AppContextType = {
    state,
    actions: {
      addAccount,
      updateAccount,
      removeAccount,
      addTransaction,
      clearError,
      refreshData,
    },
  };

  return (
    <AppContext.Provider value={contextValue}>
      {children}
    </AppContext.Provider>
  );
}

// Custom hook to use the app context
export function useAppContext(): AppContextType {
  const context = useContext(AppContext);
  if (context === undefined) {
    throw new Error('useAppContext must be used within an AppProvider');
  }
  return context;
}

// Utility hooks for specific data
export function useAccounts() {
  const { state } = useAppContext();
  return state.accounts;
}

export function useAccount(accountId: string) {
  const { state } = useAppContext();
  return state.accounts.find(account => account.id === accountId) || null;
}

export function useMetrics() {
  const { state } = useAppContext();
  return state.metrics;
}

export function useNetWorthHistory() {
  const { state } = useAppContext();
  return state.netWorthHistory;
}

export function useAppError() {
  const { state, actions } = useAppContext();
  return {
    error: state.error,
    clearError: actions.clearError,
  };
}

export function useAppLoading() {
  const { state } = useAppContext();
  return state.isLoading;
}
