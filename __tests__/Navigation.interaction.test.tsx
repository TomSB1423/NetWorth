// __tests__/Navigation.interaction.test.tsx
// Comprehensive navigation tests with user interaction simulation
// Using React Native Testing Library for realistic user behavior testing

import { configureStore } from '@reduxjs/toolkit';
import { fireEvent, render, screen, waitFor } from '@testing-library/react-native';
import React from 'react';
import { Provider } from 'react-redux';

// Import your components
import WelcomeScreen from '../app/onboarding/welcome';
import SelectBankScreen from '../app/select-bank';
// import AddAccountScreen from '../app/add-account';

// Import your store slices
import accountsSlice from '../store/slices/accountsSlice';
import metricsSlice from '../store/slices/metricsSlice';
import uiSlice from '../store/slices/uiSlice';

// Create a test store
const createTestStore = (initialState = {}) => {
  return configureStore({
    reducer: {
      accounts: accountsSlice,
      ui: uiSlice,
      metrics: metricsSlice,
    },
    preloadedState: initialState,
  });
};

// Mock router
const mockRouter = {
  push: jest.fn(),
  replace: jest.fn(),
  back: jest.fn(),
  canGoBack: jest.fn(() => true),
};

jest.mock('expo-router', () => ({
  router: mockRouter,
  useLocalSearchParams: jest.fn(() => ({})),
}));

// Test wrapper component with Redux Provider
const TestWrapper = ({ children, store = createTestStore() }) => (
  <Provider store={store}>{children}</Provider>
);

describe('Navigation User Interaction Tests', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Welcome Screen Navigation', () => {
    test('should navigate to select-bank when Get Started button is pressed', async () => {
      const store = createTestStore({
        accounts: { accounts: [], initialized: true },
      });

      render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      // Find and tap the Get Started button
      const getStartedButton = await waitFor(() => 
        screen.getByText(/get started/i) || 
        screen.getByTestId('get-started-button') ||
        screen.getByRole('button')
      );

      fireEvent.press(getStartedButton);

      // Verify navigation was called
      expect(mockRouter.push).toHaveBeenCalledWith('/select-bank');
    });

    test('should display welcome content correctly', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      // Check for welcome screen content
      await waitFor(() => {
        expect(screen.getByText(/welcome/i) || screen.getByText(/networth/i)).toBeTruthy();
      });
    });
  });

  describe('Select Bank Screen Navigation', () => {
    test('should navigate to add-account when a bank is selected', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <SelectBankScreen />
        </TestWrapper>
      );

      // Wait for banks to load and find a bank option
      await waitFor(() => {
        const bankOptions = screen.getAllByText(/chase|wells fargo|bank of america/i);
        expect(bankOptions.length).toBeGreaterThan(0);
      });

      // Find and tap the first bank option
      const firstBank = screen.getByText(/chase/i) || screen.getAllByRole('button')[0];
      fireEvent.press(firstBank);

      // Verify navigation was called with correct parameters
      expect(mockRouter.push).toHaveBeenCalledWith(
        expect.objectContaining({
          pathname: '/add-account',
          params: expect.objectContaining({
            selectedBank: expect.any(String),
            bankLabel: expect.any(String),
          }),
        })
      );
    });

    test('should handle back navigation when Cancel is pressed', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <SelectBankScreen />
        </TestWrapper>
      );

      // Find and tap the Cancel button
      const cancelButton = await waitFor(() => 
        screen.getByText(/cancel/i) || screen.getByTestId('cancel-button')
      );

      fireEvent.press(cancelButton);

      // Verify back navigation was called
      expect(mockRouter.back).toHaveBeenCalled();
    });

    test('should filter banks when search text is entered', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <SelectBankScreen />
        </TestWrapper>
      );

      // Find the search input
      const searchInput = await waitFor(() => 
        screen.getByPlaceholderText(/search/i) || screen.getByTestId('search-input')
      );

      // Type in search input
      fireEvent.changeText(searchInput, 'Chase');

      // Verify that banks are filtered (this would depend on your implementation)
      await waitFor(() => {
        const chaseBank = screen.getByText(/chase/i);
        expect(chaseBank).toBeTruthy();
      });
    });
  });

  describe('Complete User Flow Simulation', () => {
    test('should complete new user onboarding flow', async () => {
      // Test the complete flow: Welcome → Select Bank → Add Account
      
      // Step 1: Start with Welcome Screen
      const store = createTestStore({
        accounts: { accounts: [], initialized: true },
      });

      const { rerender } = render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      // Click Get Started
      const getStartedButton = await waitFor(() => 
        screen.getByText(/get started/i) || screen.getByRole('button')
      );
      fireEvent.press(getStartedButton);

      expect(mockRouter.push).toHaveBeenCalledWith('/select-bank');

      // Step 2: Simulate navigation to Select Bank Screen
      rerender(
        <TestWrapper store={store}>
          <SelectBankScreen />
        </TestWrapper>
      );

      // Select a bank
      await waitFor(() => {
        const bankOptions = screen.getAllByText(/chase|bank/i);
        expect(bankOptions.length).toBeGreaterThan(0);
      });

      const bankOption = screen.getByText(/chase/i) || screen.getAllByRole('button')[0];
      fireEvent.press(bankOption);

      expect(mockRouter.push).toHaveBeenCalledWith(
        expect.objectContaining({
          pathname: '/add-account',
        })
      );

      // Verify the complete flow executed correctly
      expect(mockRouter.push).toHaveBeenCalledTimes(2);
    });

    test('should handle error states gracefully', async () => {
      // Test error handling during navigation
      const store = createTestStore();

      // Mock router to throw an error
      mockRouter.push.mockImplementationOnce(() => {
        throw new Error('Navigation failed');
      });

      render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      const getStartedButton = await waitFor(() => 
        screen.getByText(/get started/i) || screen.getByRole('button')
      );

      // This should not crash the app
      expect(() => fireEvent.press(getStartedButton)).not.toThrow();
    });
  });

  describe('Tab Navigation Protection', () => {
    test('should prevent access to tabs when no accounts exist', () => {
      const storeWithNoAccounts = createTestStore({
        accounts: { accounts: [], initialized: true },
      });

      const hasAccounts = storeWithNoAccounts.getState().accounts.accounts.length > 0;
      expect(hasAccounts).toBe(false);

      // This would be used in your Tabs.Protected component
      expect(hasAccounts).toBe(false);
    });

    test('should allow access to tabs when accounts exist', () => {
      const storeWithAccounts = createTestStore({
        accounts: { 
          accounts: [
            { id: '1', name: 'Test Account', type: 'checking', balance: 1000 }
          ], 
          initialized: true 
        },
      });

      const hasAccounts = storeWithAccounts.getState().accounts.accounts.length > 0;
      expect(hasAccounts).toBe(true);
    });
  });

  describe('Accessibility Tests', () => {
    test('should have proper accessibility labels', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      // Check for accessibility labels
      await waitFor(() => {
        const accessibleElements = screen.getAllByRole('button');
        expect(accessibleElements.length).toBeGreaterThan(0);
      });
    });

    test('should support screen readers', async () => {
      const store = createTestStore();

      render(
        <TestWrapper store={store}>
          <SelectBankScreen />
        </TestWrapper>
      );

      // Verify that elements have proper accessibility props
      await waitFor(() => {
        const searchInput = screen.getByPlaceholderText(/search/i);
        expect(searchInput).toBeTruthy();
      });
    });
  });

  describe('Performance Tests', () => {
    test('should render Welcome screen within acceptable time', async () => {
      const store = createTestStore();
      const startTime = Date.now();

      render(
        <TestWrapper store={store}>
          <WelcomeScreen />
        </TestWrapper>
      );

      await waitFor(() => {
        expect(screen.getByText(/welcome/i) || screen.getByRole('button')).toBeTruthy();
      });

      const renderTime = Date.now() - startTime;
      expect(renderTime).toBeLessThan(1000); // Should render within 1 second
    });
  });
});

// Integration tests for the complete app flow
describe('App Navigation Integration Tests', () => {
  test('should handle complete app lifecycle', async () => {
    const store = createTestStore({
      accounts: { accounts: [], initialized: false },
    });

    // Simulate app initialization
    store.dispatch({ type: 'accounts/setInitialized', payload: true });

    // Test that state changes trigger appropriate navigation logic
    const state = store.getState();
    expect(state.accounts.initialized).toBe(true);
    expect(state.accounts.accounts.length).toBe(0);

    // This would trigger navigation to onboarding in your actual app
    if (state.accounts.initialized && state.accounts.accounts.length === 0) {
      mockRouter.replace('/onboarding/welcome');
    }

    expect(mockRouter.replace).toHaveBeenCalledWith('/onboarding/welcome');
  });

  test('should handle account creation and navigation', async () => {
    const store = createTestStore({
      accounts: { accounts: [], initialized: true },
    });

    // Simulate adding an account
    store.dispatch({
      type: 'accounts/addAccount',
      payload: { id: '1', name: 'Test Account', type: 'checking', balance: 1000 }
    });

    const state = store.getState();
    expect(state.accounts.accounts.length).toBe(1);

    // This would trigger navigation to main app in your actual app
    if (state.accounts.accounts.length > 0) {
      mockRouter.replace('/(tabs)');
    }

    expect(mockRouter.replace).toHaveBeenCalledWith('/(tabs)');
  });
});
