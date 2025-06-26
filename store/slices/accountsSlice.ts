// store/slices/accountsSlice.ts
// Accounts slice for Redux store

import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Account, Transaction } from '../../services/accountMockService';
import {
    generateSecureId,
    rateLimiter,
    sanitizeString
} from '../../utils/security';
import { validateAccount, validateTransaction } from '../../utils/validation';

// State interface
interface AccountsState {
  accounts: Account[];
  isLoading: boolean;
  error: string | null;
  isInitialized: boolean;
}

// Initial state
const initialState: AccountsState = {
  accounts: [],
  isLoading: false,
  error: null,
  isInitialized: false,
};



// Async thunks
export const initializeAccounts = createAsyncThunk(
  'accounts/initialize',
  async (_, { rejectWithValue }) => {
    try {
      // Import service dynamically to avoid circular dependencies
      const { getNetWorthService } = await import('../../services/serviceProvider');
      const netWorthService = getNetWorthService();
      const accounts = netWorthService.accountService.getAccounts();
      return accounts;
    } catch (error) {
      console.error('Failed to initialize accounts:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to initialize accounts');
    }
  }
);

export const addAccount = createAsyncThunk(
  'accounts/add',
  async (accountData: Omit<Account, 'id' | 'transactions'>, { getState, rejectWithValue }) => {
    try {
      // Rate limiting check
      if (!rateLimiter.isAllowed('addAccount', 10, 60000)) {
        throw new Error('Too many account creation attempts. Please wait a moment.');
      }

      // Validate account data
      const accountError = validateAccount(accountData);
      if (accountError) {
        throw new Error(accountError);
      }

      const state = getState() as { accounts: AccountsState };
      
      // Check for duplicate account names (case-insensitive)
      const sanitizedName = sanitizeString(accountData.name);
      const isDuplicateName = state.accounts.accounts.some(
        account => sanitizeString(account.name).toLowerCase() === sanitizedName.toLowerCase()
      );
      if (isDuplicateName) {
        throw new Error('An account with this name already exists');
      }

      // Use the account service to create the account with realistic transaction data
      const { getNetWorthService } = await import('../../services/serviceProvider');
      const netWorthService = getNetWorthService();
      
      // Create account with sanitized name and generated transactions
      const accountWithSanitizedName = {
        ...accountData,
        name: sanitizedName,
      };
      
      const newAccount = netWorthService.accountService.addAccount(accountWithSanitizedName);

      return newAccount;
    } catch (error) {
      console.error('Failed to add account:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to add account');
    }
  }
);

export const updateAccount = createAsyncThunk(
  'accounts/update',
  async (
    { accountId, updates }: { accountId: string; updates: Partial<Omit<Account, 'id' | 'transactions'>> },
    { getState, rejectWithValue }
  ) => {
    try {
      // Validate updates
      const updateError = validateAccount(updates);
      if (updateError) {
        throw new Error(updateError);
      }

      const state = getState() as { accounts: AccountsState };

      // Check for duplicate names (excluding current account)
      if (updates.name) {
        const sanitizedName = sanitizeString(updates.name);
        const isDuplicateUpdateName = state.accounts.accounts.some(
          account => account.id !== accountId && 
          sanitizeString(account.name).toLowerCase() === sanitizedName.toLowerCase()
        );
        if (isDuplicateUpdateName) {
          throw new Error('An account with this name already exists');
        }
      }

      // Sanitize string inputs
      const sanitizedUpdates = {
        ...updates,
        ...(updates.name && { name: sanitizeString(updates.name) }),
      };

      return { accountId, updates: sanitizedUpdates };
    } catch (error) {
      console.error('Failed to update account:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to update account');
    }
  }
);

export const removeAccount = createAsyncThunk(
  'accounts/remove',
  async (accountId: string, { rejectWithValue }) => {
    try {
      return accountId;
    } catch (error) {
      console.error('Failed to remove account:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to remove account');
    }
  }
);

export const addTransaction = createAsyncThunk(
  'accounts/addTransaction',
  async (
    { accountId, transactionData }: { accountId: string; transactionData: Omit<Transaction, 'id'> },
    { rejectWithValue }
  ) => {
    try {
      // Rate limiting check
      if (!rateLimiter.isAllowed(`addTransaction_${accountId}`, 20, 60000)) {
        throw new Error('Too many transaction creation attempts. Please wait a moment.');
      }

      // Validate transaction
      const transactionError = validateTransaction(transactionData);
      if (transactionError) {
        throw new Error(transactionError);
      }

      // Generate a secure ID
      const newId = generateSecureId();
      
      const newTransaction: Transaction = {
        ...transactionData,
        id: newId,
        description: sanitizeString(transactionData.description),
      };

      return { accountId, transaction: newTransaction };
    } catch (error) {
      console.error('Failed to add transaction:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to add transaction');
    }
  }
);

// Slice
const accountsSlice = createSlice({
  name: 'accounts',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
  },
  extraReducers: (builder) => {
    builder
      // Initialize accounts
      .addCase(initializeAccounts.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(initializeAccounts.fulfilled, (state, action) => {
        state.isLoading = false;
        state.accounts = action.payload;
        state.isInitialized = true;
        state.error = null;
      })
      .addCase(initializeAccounts.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      
      // Add account
      .addCase(addAccount.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(addAccount.fulfilled, (state, action) => {
        state.isLoading = false;
        state.accounts.push(action.payload);
        state.error = null;
      })
      .addCase(addAccount.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      })
      
      // Update account
      .addCase(updateAccount.pending, (state) => {
        state.error = null;
      })
      .addCase(updateAccount.fulfilled, (state, action) => {
        const { accountId, updates } = action.payload;
        const accountIndex = state.accounts.findIndex(account => account.id === accountId);
        if (accountIndex !== -1) {
          state.accounts[accountIndex] = { ...state.accounts[accountIndex], ...updates };
        }
        state.error = null;
      })
      .addCase(updateAccount.rejected, (state, action) => {
        state.error = action.payload as string;
      })
      
      // Remove account
      .addCase(removeAccount.fulfilled, (state, action) => {
        state.accounts = state.accounts.filter(account => account.id !== action.payload);
        state.error = null;
      })
      .addCase(removeAccount.rejected, (state, action) => {
        state.error = action.payload as string;
      })
      
      // Add transaction
      .addCase(addTransaction.fulfilled, (state, action) => {
        const { accountId, transaction } = action.payload;
        const accountIndex = state.accounts.findIndex(account => account.id === accountId);
        if (accountIndex !== -1) {
          state.accounts[accountIndex].transactions.push(transaction);
        }
        state.error = null;
      })
      .addCase(addTransaction.rejected, (state, action) => {
        state.error = action.payload as string;
      });
  },
});

export const { clearError, setLoading } = accountsSlice.actions;
export default accountsSlice.reducer;
