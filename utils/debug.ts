// utils/debug.ts
// Development debug utilities - single consolidated file

import AsyncStorage from '@react-native-async-storage/async-storage';
import { router } from 'expo-router';
import { Alert } from 'react-native';
import { AccountType } from '../services/accountMockService';
import { persistor, resetStore, store } from '../store/index';
import { addAccount, removeAccount } from '../store/slices/accountsSlice';
import { setFirstLaunch } from '../store/slices/uiSlice';

// Debug mode flag
export const DEBUG_MODE = __DEV__ && true;

// Sample account data for testing
const SAMPLE_ACCOUNTS = [
  { name: 'Main Checking', type: 'checking' as AccountType, emoji: '🏦', bank: 'chase' },
  { name: 'Emergency Savings', type: 'savings' as AccountType, emoji: '💰', bank: 'hsbc' },
  { name: 'Credit Card', type: 'credit' as AccountType, emoji: '💳', bank: 'capital_one' },
  { name: 'Investment Account', type: 'investment' as AccountType, emoji: '📈', bank: 'schwab' },
  { name: 'Home Mortgage', type: 'mortgage' as AccountType, emoji: '🏠', bank: 'wells_fargo' },
];

export class DebugCommands {
  /**
   * Complete app reset - clears all data and returns to fresh state
   */
  static async resetApp(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      console.log('🧹 Starting complete app reset...');
      
      // 1. Purge Redux persist store
      await persistor.purge();
      await persistor.flush();
      
      // 2. Clear AsyncStorage completely for our app
      try {
        const keys = await AsyncStorage.getAllKeys();
        const netWorthKeys = keys.filter(key => 
          key.startsWith('persist:networth') || 
          key.includes('networth') ||
          key.startsWith('redux-persist')
        );
        
        if (netWorthKeys.length > 0) {
          await AsyncStorage.multiRemove(netWorthKeys);
          console.log(`🗑️ Cleared ${netWorthKeys.length} AsyncStorage keys`);
        }
      } catch (asyncStorageError) {
        console.warn('⚠️ AsyncStorage clear failed:', asyncStorageError);
      }
      
      // 3. Reset Redux store
      resetStore();
      
      // 4. Set first launch flag to true so welcome screen shows
      store.dispatch(setFirstLaunch(true));
      
      // 5. For web, also clear localStorage if available
      if (typeof window !== 'undefined' && window.localStorage) {
        const keys = Object.keys(window.localStorage);
        keys.forEach(key => {
          if (key.startsWith('persist:networth') || 
              key.includes('networth') || 
              key.startsWith('redux-persist')) {
            window.localStorage.removeItem(key);
          }
        });
        console.log('🌐 Cleared web localStorage');
      }
      
      console.log('✅ App reset completed successfully');
      
      // 6. Navigate to fresh start
      try {
        router.replace('/');
        
        // For web, reload the page
        if (typeof window !== 'undefined') {
          setTimeout(() => {
            window.location.reload();
          }, 100);
        }
      } catch (navigationError) {
        console.warn('⚠️ Navigation reset failed:', navigationError);
      }
      
    } catch (error) {
      console.error('❌ Failed to reset app:', error);
      throw error;
    }
  }

  /**
   * Add a sample checking account
   */
  static async addSampleAccount(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      await store.dispatch(addAccount({
        name: `Test Account ${Date.now()}`,
        type: 'checking',
        emoji: '🏦',
        bank: 'chase'
      })).unwrap();
      
      console.log('✅ Added sample account');
    } catch (error) {
      console.error('❌ Failed to add sample account:', error);
    }
  }

  /**
   * Add multiple sample accounts for testing
   */
  static async addSampleAccounts(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      for (const account of SAMPLE_ACCOUNTS) {
        await store.dispatch(addAccount(account)).unwrap();
      }
      console.log(`✅ Added ${SAMPLE_ACCOUNTS.length} sample accounts`);
    } catch (error) {
      console.error('❌ Failed to add sample accounts:', error);
    }
  }

  /**
   * Remove all accounts
   */
  static async removeAllAccounts(): Promise<void> {
    if (!DEBUG_MODE) return;
    
    try {
      const state = store.getState();
      const accounts = state.accounts.accounts;
      
      for (const account of accounts) {
        await store.dispatch(removeAccount(account.id)).unwrap();
      }
      
      console.log(`✅ Removed ${accounts.length} accounts`);
    } catch (error) {
      console.error('❌ Failed to remove accounts:', error);
    }
  }

  /**
   * Show current app state info
   */
  static showAppInfo(): void {
    if (!DEBUG_MODE) return;
    
    const state = store.getState();
    const accounts = state.accounts.accounts;
    const isInitialized = state.accounts.isInitialized;
    
    const info = [
      `=== APP DEBUG INFO ===`,
      `Accounts: ${accounts.length}`,
      `Initialized: ${isInitialized}`,
      `Account Types: ${accounts.map(a => a.type).join(', ') || 'None'}`,
      `Debug Mode: ${DEBUG_MODE}`,
      `Should show onboarding: ${accounts.length === 0}`,
    ].join('\n');
    
    console.log(info);
  }
}

// Register global debug helpers in development
if (DEBUG_MODE && typeof global !== 'undefined') {
  (global as any).resetApp = DebugCommands.resetApp;
  (global as any).addSampleAccount = DebugCommands.addSampleAccount;
  (global as any).addSampleAccounts = DebugCommands.addSampleAccounts;
  (global as any).removeAllAccounts = DebugCommands.removeAllAccounts;
  (global as any).showAppInfo = DebugCommands.showAppInfo;
  
  console.log('🔧 Debug helpers available:');
  console.log('   - resetApp() - Reset app to fresh state');
  console.log('   - addSampleAccount() - Add one test account');
  console.log('   - addSampleAccounts() - Add multiple test accounts');
  console.log('   - removeAllAccounts() - Remove all accounts');
  console.log('   - showAppInfo() - Show current app state');
}
