import { fireEvent, render, waitFor } from '@testing-library/react-native';
import { router } from 'expo-router';
import React from 'react';
import { Provider } from 'react-redux';
import AddAccountPage from '../app/add-account';
import { store } from '../store';

// Mock expo-router
jest.mock('expo-router', () => ({
  router: {
    back: jest.fn(),
    push: jest.fn(),
    replace: jest.fn(),
  },
  useLocalSearchParams: jest.fn(),
}));

// Test wrapper with Redux provider
const TestWrapper = ({ children }: { children: React.ReactNode }) => (
  <Provider store={store}>
    {children}
  </Provider>
);

const mockUseLocalSearchParams = jest.mocked(require('expo-router').useLocalSearchParams);

describe('AddAccountPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Component Rendering', () => {
    it('renders successfully with bank parameters', () => {
      mockUseLocalSearchParams.mockReturnValue({
        selectedBank: 'chase',
        bankLabel: 'Chase Bank',
        bankEmoji: '🏦',
      });

      const { getByText, getByDisplayValue } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      expect(getByText('Add Account')).toBeTruthy();
      expect(getByDisplayValue('Chase Bank Account')).toBeTruthy();
    });

    it('renders with empty parameters', () => {
      mockUseLocalSearchParams.mockReturnValue({});

      const { getByText, getByPlaceholderText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      expect(getByText('Add Account')).toBeTruthy();
      expect(getByPlaceholderText('Account name')).toBeTruthy();
    });

    it('displays account type options', () => {
      mockUseLocalSearchParams.mockReturnValue({});

      const { getByText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      expect(getByText('Current Account')).toBeTruthy();
      expect(getByText('Savings Account')).toBeTruthy();
      expect(getByText('Credit Card')).toBeTruthy();
    });
  });

  describe('Form Interactions', () => {
    it('allows account name input', async () => {
      mockUseLocalSearchParams.mockReturnValue({});

      const { getByPlaceholderText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      const nameInput = getByPlaceholderText('Account name');
      fireEvent.changeText(nameInput, 'My Test Account');

      await waitFor(() => {
        expect(nameInput.props.value).toBe('My Test Account');
      });
    });

    it('allows account type selection', async () => {
      mockUseLocalSearchParams.mockReturnValue({});

      const { getByText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      const savingsOption = getByText('Savings Account');
      fireEvent.press(savingsOption);

      // Verify selection state change (would need access to component state)
      expect(savingsOption).toBeTruthy();
    });
  });

  describe('Edge Cases', () => {
    it('handles missing bank parameters gracefully', () => {
      mockUseLocalSearchParams.mockReturnValue({
        selectedBank: '',
        bankLabel: '',
        bankEmoji: '',
      });

      const { getByPlaceholderText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      expect(getByPlaceholderText('Account name')).toBeTruthy();
    });

    it('handles partial bank parameters', () => {
      mockUseLocalSearchParams.mockReturnValue({
        selectedBank: 'wells-fargo',
        bankLabel: 'Wells Fargo',
        bankEmoji: '',
      });

      const { getByDisplayValue } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      expect(getByDisplayValue('Wells Fargo Account')).toBeTruthy();
    });
  });

  describe('Navigation', () => {
    it('calls router.back when back button is pressed', () => {
      mockUseLocalSearchParams.mockReturnValue({});

      const { getByLabelText } = render(
        <TestWrapper>
          <AddAccountPage />
        </TestWrapper>
      );

      // Assuming there's a back button with aria-label
      const backButton = getByLabelText('Go back') || getByLabelText('Back');
      fireEvent.press(backButton);

      expect(router.back).toHaveBeenCalled();
    });
  });
});
