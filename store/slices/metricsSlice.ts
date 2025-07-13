// store/slices/metricsSlice.ts
// Metrics slice for Redux store

import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { Transaction } from '../../services/accountMockService';
import { MetricsData, MetricsService } from '../../services/metricsService';
import { NetWorthEntry } from '../../services/netWorthService';

// State interface
interface MetricsState {
  metrics: MetricsData | null;
  netWorthHistory: NetWorthEntry[];
  isLoading: boolean;
  error: string | null;
  lastUpdated: number;
}

// Initial state
const initialState: MetricsState = {
  metrics: null,
  netWorthHistory: [],
  isLoading: false,
  error: null,
  lastUpdated: 0,
};

// Async thunks
export const calculateMetrics = createAsyncThunk(
  'metrics/calculate',
  async (_, { getState, rejectWithValue }) => {
    try {
      const state = getState() as any; // Temporary - will fix with proper typing
      const accounts = state.accounts.accounts;
      
      if (accounts.length === 0) {
        return {
          metrics: null,
          netWorthHistory: [],
        };
      }
      
      // Calculate metrics
      const metricsService = new MetricsService(accounts);
      const metrics = metricsService.getMetrics();
      
      // Calculate net worth history
      const netWorthHistory: NetWorthEntry[] = [];
      const now = new Date();
      const days = 365;
      
      for (let i = days - 1; i >= 0; i--) {
        const date = new Date(now);
        date.setDate(now.getDate() - i);
        const dateString = date.toISOString().slice(0, 10);
        
        let totalNetWorth = 0;
        
        // Calculate net worth for this specific date
        for (const account of accounts) {
          // Get balance up to this date
          const accountBalance = account.transactions
            .filter((t: Transaction) => t.date <= dateString)
            .reduce((sum: number, t: Transaction) => sum + t.amount, 0);
          
          // Handle different account types
          if (account.type === 'credit' || account.type === 'mortgage') {
            // For debt accounts, negative balance means we owe money
            // So we subtract the absolute value of the balance from net worth
            totalNetWorth -= Math.abs(accountBalance);
          } else {
            // For asset accounts (checking, savings, investment), add to net worth
            totalNetWorth += accountBalance;
          }
        }
        
        netWorthHistory.push({
          date: dateString,
          value: Math.round(totalNetWorth),
        });
      }
      
      return {
        metrics,
        netWorthHistory,
      };
    } catch (error) {
      console.error('Failed to calculate metrics:', error);
      return rejectWithValue(error instanceof Error ? error.message : 'Failed to calculate metrics');
    }
  }
);

export const refreshMetrics = createAsyncThunk(
  'metrics/refresh',
  async (_, { dispatch }) => {
    // Simply re-calculate metrics
    return dispatch(calculateMetrics());
  }
);

// Slice
const metricsSlice = createSlice({
  name: 'metrics',
  initialState,
  reducers: {
    clearMetricsError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Calculate metrics
      .addCase(calculateMetrics.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(calculateMetrics.fulfilled, (state, action) => {
        state.isLoading = false;
        if (action.payload) {
          state.metrics = action.payload.metrics;
          state.netWorthHistory = action.payload.netWorthHistory;
          state.lastUpdated = Date.now();
        }
        state.error = null;
      })
      .addCase(calculateMetrics.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearMetricsError } = metricsSlice.actions;
export default metricsSlice.reducer;
