// utils/clearStorage.ts
// Development utility to clear persisted Redux state

import AsyncStorage from '@react-native-async-storage/async-storage';
import { router } from 'expo-router';
import { persistor, resetStore, store } from '../store/index';
import { setFirstLaunch } from '../store/slices/uiSlice';

export const clearPersistedData = async () => {
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
    
    console.log('✅ Persisted data cleared successfully');
    
    // 6. Navigate to fresh start and reload
    try {
      // Reset navigation stack to initial state
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
    console.error('❌ Failed to clear persisted data:', error);
    throw error;
  }
};

// Make it available globally in development
if (__DEV__ && typeof global !== 'undefined') {
  (global as any).clearNetWorthData = clearPersistedData;
  (global as any).resetApp = clearPersistedData;
  console.log('🔧 Development helpers available:');
  console.log('   - clearNetWorthData() - Reset app to fresh state');  
  console.log('   - resetApp() - Alias for clearNetWorthData()');
}
