// store/Provider.tsx
// Redux Provider wrapper with persistence

import React, { ReactNode } from 'react';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from './index';
import LoadingSpinner from '../components/LoadingSpinner';

interface ReduxProviderProps {
  children: ReactNode;
}

export default function ReduxProvider({ children }: ReduxProviderProps) {
  return (
    <Provider store={store}>
      <PersistGate 
        loading={<LoadingSpinner message="Loading your data..." />} 
        persistor={persistor}
      >
        {children}
      </PersistGate>
    </Provider>
  );
}
