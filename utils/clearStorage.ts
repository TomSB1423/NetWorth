// utils/clearStorage.ts
// Development utility to clear persisted Redux state

import { resetStore } from '../store/index';

export const clearPersistedData = async () => {
  try {
    // Clear Redux persist storage
    resetStore();
    
    // For web, also clear localStorage if available
    if (typeof window !== 'undefined' && window.localStorage) {
      // Clear all items that start with our persist key
      const keys = Object.keys(window.localStorage);
      keys.forEach(key => {
        if (key.startsWith('persist:networth') || key.includes('networth')) {
          window.localStorage.removeItem(key);
        }
      });
    }
    
    console.log('✅ Persisted data cleared successfully');
    
    // Reload the page to start fresh
    if (typeof window !== 'undefined') {
      window.location.reload();
    }
  } catch (error) {
    console.error('❌ Failed to clear persisted data:', error);
  }
};

// Make it available globally in development
if (__DEV__ && typeof window !== 'undefined') {
  (window as any).clearNetWorthData = clearPersistedData;
  console.log('🔧 Development helper available: run clearNetWorthData() in console to reset app');
}
