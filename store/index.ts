// store/index.ts
// Redux store configuration with RTK and persistence

import { configureStore, combineReducers } from '@reduxjs/toolkit';
import { persistStore, persistReducer, FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER } from 'redux-persist';
import AsyncStorage from '@react-native-async-storage/async-storage';

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
  // Persist accounts and UI state (for hasEverHadAccounts flag), metrics can be recalculated
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
export const resetStore = () => {
  persistor.purge();
};
