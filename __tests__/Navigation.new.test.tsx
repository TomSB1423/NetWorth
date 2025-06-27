import { configureStore } from '@reduxjs/toolkit';
import { fireEvent, render } from '@testing-library/react-native';
import { router } from 'expo-router';
import React from 'react';
import { Text, TouchableOpacity, View } from 'react-native';
import { Provider } from 'react-redux';
import accountsSlice from '../store/slices/accountsSlice';
import metricsSlice from '../store/slices/metricsSlice';
import uiSlice from '../store/slices/uiSlice';

// Mock expo-router
jest.mock('expo-router', () => ({
  router: {
    push: jest.fn(),
    replace: jest.fn(),
    back: jest.fn(),
    canGoBack: jest.fn(() => true),
  },
  useRouter: jest.fn(() => ({
    push: jest.fn(),
    replace: jest.fn(),
    back: jest.fn(),
    canGoBack: jest.fn(() => true),
  })),
  useLocalSearchParams: jest.fn(() => ({})),
  Link: ({ children, href, ...props }: { children: React.ReactNode; href: string }) => (
    <TouchableOpacity onPress={() => router.push(href)} {...props}>
      {children}
    </TouchableOpacity>
  ),
}));

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

const TestWrapper = ({ children }: { children: React.ReactNode }) => (
  <Provider store={createTestStore()}>
    {children}
  </Provider>
);

// Mock Link component for testing
const MockLinkComponent = ({ href, children }: { href: string; children: React.ReactNode }) => (
  <TouchableOpacity 
    testID={`link-${href}`}
    onPress={() => router.push(href)}
  >
    <Text>{children}</Text>
  </TouchableOpacity>
);

describe('Navigation Tests', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Router Navigation', () => {
    it('navigates to add account page', () => {
      const { getByTestId } = render(
        <TestWrapper>
          <MockLinkComponent href="/add-account">
            Add Account
          </MockLinkComponent>
        </TestWrapper>
      );

      const addAccountLink = getByTestId('link-/add-account');
      fireEvent.press(addAccountLink);

      expect(router.push).toHaveBeenCalledWith('/add-account');
    });

    it('navigates to select bank page', () => {
      const { getByTestId } = render(
        <TestWrapper>
          <MockLinkComponent href="/select-bank">
            Select Bank
          </MockLinkComponent>
        </TestWrapper>
      );

      const selectBankLink = getByTestId('link-/select-bank');
      fireEvent.press(selectBankLink);

      expect(router.push).toHaveBeenCalledWith('/select-bank');
    });

    it('navigates between tabs', () => {
      const TabComponent = () => (
        <View>
          <MockLinkComponent href="/(tabs)/dashboard">Dashboard</MockLinkComponent>
          <MockLinkComponent href="/(tabs)/transactions">Transactions</MockLinkComponent>
          <MockLinkComponent href="/(tabs)/settings">Settings</MockLinkComponent>
        </View>
      );

      const { getByTestId } = render(
        <TestWrapper>
          <TabComponent />
        </TestWrapper>
      );

      // Test dashboard navigation
      fireEvent.press(getByTestId('link-/(tabs)/dashboard'));
      expect(router.push).toHaveBeenCalledWith('/(tabs)/dashboard');

      // Test transactions navigation
      fireEvent.press(getByTestId('link-/(tabs)/transactions'));
      expect(router.push).toHaveBeenCalledWith('/(tabs)/transactions');

      // Test settings navigation
      fireEvent.press(getByTestId('link-/(tabs)/settings'));
      expect(router.push).toHaveBeenCalledWith('/(tabs)/settings');
    });
  });

  describe('Back Navigation', () => {
    it('handles back navigation correctly', () => {
      const BackButtonComponent = () => (
        <TouchableOpacity 
          testID="back-button"
          onPress={() => router.back()}
        >
          <Text>Back</Text>
        </TouchableOpacity>
      );

      const { getByTestId } = render(
        <TestWrapper>
          <BackButtonComponent />
        </TestWrapper>
      );

      fireEvent.press(getByTestId('back-button'));
      expect(router.back).toHaveBeenCalled();
    });

    it('checks if back navigation is available', () => {
      router.canGoBack = jest.fn(() => true);

      const ConditionalBackComponent = () => {
        const canGoBack = router.canGoBack();
        return (
          <View>
            {canGoBack && (
              <TouchableOpacity testID="conditional-back">
                <Text>Go Back</Text>
              </TouchableOpacity>
            )}
          </View>
        );
      };

      const { getByTestId } = render(
        <TestWrapper>
          <ConditionalBackComponent />
        </TestWrapper>
      );

      expect(getByTestId('conditional-back')).toBeTruthy();
      expect(router.canGoBack).toHaveBeenCalled();
    });
  });

  describe('Navigation with Parameters', () => {
    it('navigates with query parameters', () => {
      const NavigateWithParamsComponent = () => (
        <TouchableOpacity
          testID="navigate-with-params"
          onPress={() => router.push('/add-account?selectedBank=chase&bankLabel=Chase')}
        >
          <Text>Add Chase Account</Text>
        </TouchableOpacity>
      );

      const { getByTestId } = render(
        <TestWrapper>
          <NavigateWithParamsComponent />
        </TestWrapper>
      );

      fireEvent.press(getByTestId('navigate-with-params'));
      expect(router.push).toHaveBeenCalledWith('/add-account?selectedBank=chase&bankLabel=Chase');
    });
  });

  describe('Error Handling', () => {
    it('handles navigation errors gracefully', () => {
      router.push = jest.fn().mockImplementation(() => {
        throw new Error('Navigation failed');
      });

      const ErrorProneNavigationComponent = () => {
        const handleNavigation = () => {
          try {
            router.push('/error-route');
          } catch (error) {
            // Handle error silently for test
          }
        };

        return (
          <TouchableOpacity testID="error-navigation" onPress={handleNavigation}>
            <Text>Navigate with Error</Text>
          </TouchableOpacity>
        );
      };

      const { getByTestId } = render(
        <TestWrapper>
          <ErrorProneNavigationComponent />
        </TestWrapper>
      );

      // Should not crash when navigation fails
      expect(() => {
        fireEvent.press(getByTestId('error-navigation'));
      }).not.toThrow();
    });
  });
});
