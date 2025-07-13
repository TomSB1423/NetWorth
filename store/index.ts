// store/index.ts
// Redux store configuration with RTK and persistence

import AsyncStorage from '@react-native-async-storage/async-storage';
import { combineReducers, configureStore } from '@reduxjs/toolkit';
import { FLUSH, PAUSE, PERSIST, persistReducer, persistStore, PURGE, REGISTER, REHYDRATE } from 'redux-persist';

// Import slices
import accountsReducer from './slices/accountsSlice';
import metricsReducer from './slices/metricsSlice';
import uiReducer from './slices/uiSlice';

// Combine reducers
const rootReducer = combineReducers({
  accounts: accountsReducer,
  metrics: metricsReducer,
  ui: uiReducer,
});

// Persistence configuration
const persistConfig = {
  key: 'networth-root',
  version: 1,
  storage: AsyncStorage,
  // Persist accounts and UI state, metrics can be recalculated
  whitelist: ['accounts', 'ui'],
};

const persistedReducer = persistReducer(persistConfig, rootReducer);

// Configure store
export const store = configureStore({
  reducer: persistedReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
      },
    }),
  devTools: __DEV__, // Enable Redux DevTools in development
});

export const persistor = persistStore(store);

// Types for TypeScript
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

// Helper to reset store (useful for logout or data reset)
export const resetStore = async () => {
  try {
    // Purge the persistor to clear all persisted data
    await persistor.purge();
    await persistor.flush();
    
    // Dispatch reset actions to all slices if needed
    // (Note: purge should handle this, but we can be explicit)
    
    console.log('🔄 Redux store reset complete');
  } catch (error) {
    console.error('❌ Store reset failed:', error);
    throw error;
  }
};
