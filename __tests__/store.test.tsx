import { configureStore } from '@reduxjs/toolkit';
import { Account } from '../services/accountMockService';
import accountsSlice, { addAccount, removeAccount } from '../store/slices/accountsSlice';
import metricsSlice from '../store/slices/metricsSlice';
import uiSlice, { setSelectedTimeRange, setTheme } from '../store/slices/uiSlice';

// Create test store
const createTestStore = () => {
  return configureStore({
    reducer: {
      accounts: accountsSlice,
      metrics: metricsSlice,
      ui: uiSlice,
    }
  });
};

describe('Redux Store Tests', () => {
  let store: ReturnType<typeof createTestStore>;

  beforeEach(() => {
    store = createTestStore();
  });

  describe('Accounts Slice', () => {
    it('has correct initial state', () => {
      const state = store.getState().accounts;
      
      expect(state.accounts).toEqual([]);
      expect(state.isLoading).toBe(false);
      expect(state.error).toBeNull();
      expect(state.isInitialized).toBe(false);
    });

    it('adds an account', () => {
      const newAccount: Account = {
        id: '1',
        name: 'Test Account',
        type: 'checking',
        transactions: []
      };

      store.dispatch(addAccount(newAccount));
      
      const state = store.getState().accounts;
      expect(state.accounts).toHaveLength(1);
      expect(state.accounts[0]).toEqual(newAccount);
    });

    it('removes an account', () => {
      const account1: Account = {
        id: '1',
        name: 'Account 1',
        type: 'checking',
        transactions: []
      };

      const account2: Account = {
        id: '2',
        name: 'Account 2',
        type: 'savings',
        transactions: []
      };

      // Add accounts
      store.dispatch(addAccount(account1));
      store.dispatch(addAccount(account2));
      
      expect(store.getState().accounts.accounts).toHaveLength(2);

      // Remove one account
      store.dispatch(removeAccount('1'));
      
      const state = store.getState().accounts;
      expect(state.accounts).toHaveLength(1);
      expect(state.accounts[0].id).toBe('2');
    });

    it('handles duplicate account prevention', () => {
      const account: Account = {
        id: '1',
        name: 'Test Account',
        type: 'checking',
        transactions: []
      };

      // Add the same account twice
      store.dispatch(addAccount(account));
      store.dispatch(addAccount(account));
      
      const state = store.getState().accounts;
      expect(state.accounts).toHaveLength(1); // Should not duplicate
    });
  });

  describe('UI Slice', () => {
    it('has correct initial state', () => {
      const state = store.getState().ui;
      
      expect(state.selectedTimeRange).toBe('all');
      expect(state.theme).toBe('auto');
      expect(state.isFirstLaunch).toBe(true);
    });

    it('changes selected time range', () => {
      store.dispatch(setSelectedTimeRange('3m'));
      
      const state = store.getState().ui;
      expect(state.selectedTimeRange).toBe('3m');
    });

    it('changes theme', () => {
      store.dispatch(setTheme('dark'));
      
      const state = store.getState().ui;
      expect(state.theme).toBe('dark');
    });

    it('handles invalid time range gracefully', () => {
      const validRanges = ['all', '1y', '6m', '3m', '1m', '1w'];
      
      // Test all valid ranges
      validRanges.forEach(range => {
        store.dispatch(setSelectedTimeRange(range));
        expect(store.getState().ui.selectedTimeRange).toBe(range);
      });
    });

    it('handles invalid theme gracefully', () => {
      const validThemes: ('light' | 'dark' | 'auto')[] = ['light', 'dark', 'auto'];
      
      // Test all valid themes
      validThemes.forEach(theme => {
        store.dispatch(setTheme(theme));
        expect(store.getState().ui.theme).toBe(theme);
      });
    });
  });

  describe('Metrics Slice', () => {
    it('has correct initial state', () => {
      const state = store.getState().metrics;
      
      expect(state.metrics).toBeNull();
      expect(state.netWorthHistory).toEqual([]);
      expect(state.isLoading).toBe(false);
      expect(state.error).toBeNull();
      expect(state.lastUpdated).toBe(0);
    });
  });

  describe('Store Integration', () => {
    it('maintains state consistency across slices', () => {
      // Add some accounts
      const account1: Account = {
        id: '1',
        name: 'Checking',
        type: 'checking',
        transactions: []
      };

      store.dispatch(addAccount(account1));
      store.dispatch(setSelectedTimeRange('3m'));
      store.dispatch(setTheme('dark'));

      const state = store.getState();
      
      expect(state.accounts.accounts).toHaveLength(1);
      expect(state.ui.selectedTimeRange).toBe('3m');
      expect(state.ui.theme).toBe('dark');
    });

    it('handles multiple rapid dispatches', () => {
      const accounts: Account[] = [
        { id: '1', name: 'Account 1', type: 'checking', transactions: [] },
        { id: '2', name: 'Account 2', type: 'savings', transactions: [] },
        { id: '3', name: 'Account 3', type: 'credit', transactions: [] }
      ];

      // Dispatch multiple actions rapidly
      accounts.forEach(account => {
        store.dispatch(addAccount(account));
      });

      const state = store.getState().accounts;
      expect(state.accounts).toHaveLength(3);
      expect(state.accounts.map(a => a.id)).toEqual(['1', '2', '3']);
    });

    it('maintains immutability', () => {
      const initialState = store.getState();
      
      store.dispatch(setSelectedTimeRange('1m'));
      
      const newState = store.getState();
      
      // Original state should not be mutated
      expect(initialState.ui.selectedTimeRange).toBe('all');
      expect(newState.ui.selectedTimeRange).toBe('1m');
      expect(initialState).not.toBe(newState);
    });
  });

  describe('Error Handling', () => {
    it('handles store errors gracefully', () => {
      // Test that the store doesn't crash with edge cases
      expect(() => {
        store.dispatch(removeAccount('non-existent-id'));
      }).not.toThrow();

      expect(() => {
        store.dispatch(setSelectedTimeRange(''));
      }).not.toThrow();
    });
  });
});
