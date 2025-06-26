// utils/debug.ts
// Debug utilities and commands

import { Alert } from 'react-native';
import { AccountType } from '../services/accountMockService';
import { store } from '../store';
import { addAccount, removeAccount } from '../store/slices/accountsSlice';

// Debug mode flag - set to false in production
export const DEBUG_MODE = __DEV__ && true;

// Sample account data for testing
const SAMPLE_ACCOUNTS = [
  {
    name: 'Main Checking',
    type: 'checking' as AccountType,
  },
  {
    name: 'Emergency Savings',
    type: 'savings' as AccountType,
  },
  {
    name: 'Credit Card',
    type: 'credit' as AccountType,
  },
  {
    name: 'Investment Account',
    type: 'investment' as AccountType,
  },
  {
    name: 'Home Mortgage',
    type: 'mortgage' as AccountType,
  },
];

export class DebugCommands {
  /**
   * Add a sample checking account
   */
  static async addSampleCheckingAccount(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      await store.dispatch(addAccount({
        name: `Test Checking ${Date.now()}`,
        type: 'checking',
      })).unwrap();
      
      Alert.alert('Debug', 'Added sample checking account');
    } catch (error) {
      Alert.alert('Debug Error', 'Failed to add checking account');
    }
  }

  /**
   * Add multiple sample accounts
   */
  static async addMultipleSampleAccounts(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      for (const account of SAMPLE_ACCOUNTS) {
        await store.dispatch(addAccount({
          name: `${account.name} ${Date.now()}`,
          type: account.type,
        })).unwrap();
      }
      
      Alert.alert('Debug', `Added ${SAMPLE_ACCOUNTS.length} sample accounts`);
    } catch (error) {
      Alert.alert('Debug Error', 'Failed to add sample accounts');
    }
  }

  /**
   * Remove all accounts
   */
  static async removeAllAccounts(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    Alert.alert(
      'Debug - Remove All Accounts',
      'This will remove ALL accounts. Are you sure?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Remove All',
          style: 'destructive',
          onPress: async () => {
            try {
              const state = store.getState();
              const accounts = state.accounts.accounts;
              
              for (const account of accounts) {
                await store.dispatch(removeAccount(account.id)).unwrap();
              }
              
              Alert.alert('Debug', `Removed ${accounts.length} accounts`);
            } catch (error) {
              Alert.alert('Debug Error', 'Failed to remove accounts');
            }
          },
        },
      ]
    );
  }

  /**
   * Reset user to fresh start (new user state)
   */
  static async resetToFreshStart(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    Alert.alert(
      'Debug - Fresh Start Reset',
      'This will completely reset the app to fresh state and restart.\n\nAre you sure?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Reset',
          style: 'destructive',
          onPress: async () => {
            try {
              // Import the reset utilities
              const { clearPersistedData } = await import('./clearStorage');
              
              // Clear all persisted data including AsyncStorage
              await clearPersistedData();
              
              Alert.alert('Debug', 'Reset complete! App will restart.');
            } catch (error) {
              console.error('Reset error:', error);
              Alert.alert('Debug Error', 'Failed to reset app state');
            }
          },
        },
      ]
    );
  }

  /**
   * Show debug info about current state
   */
  static showDebugInfo(): void {
    if (!DEBUG_MODE) return;
    
    const state = store.getState();
    const accounts = state.accounts.accounts;
    const isInitialized = state.accounts.isInitialized;
    
    const debugInfo = [
      `=== APP STATE DEBUG ===`,
      `Accounts: ${accounts.length}`,
      `Is Initialized: ${isInitialized}`,
      `Account Types: ${accounts.map(a => a.type).join(', ') || 'None'}`,
      `Debug Mode: ${DEBUG_MODE}`,
      ``,
      `=== ROUTING LOGIC ===`,
      `Should show onboarding: ${accounts.length === 0}`,
      `Should go to main app: ${accounts.length > 0}`,
    ].join('\n');
    
    Alert.alert('Debug Info', debugInfo, [
      { text: 'OK' },
      { 
        text: 'Copy to Clipboard', 
        onPress: () => {
          // Simple clipboard copy for debugging
          console.log('DEBUG INFO:', debugInfo);
        }
      }
    ]);
  }

  /**
   * Force navigate to welcome screen (for testing onboarding)
   */
  static forceNavigateToWelcome(): void {
    if (!DEBUG_MODE) return;
    
    const { router } = require('expo-router');
    router.push('/onboarding/welcome');
    Alert.alert('Debug', 'Navigated to welcome screen');
  }
}

/**
 * Debug menu component props
 */
export interface DebugMenuProps {
  colors: any;
  style?: any;
}
