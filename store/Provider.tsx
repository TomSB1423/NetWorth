// store/Provider.tsx
// Redux Provider wrapper with persistence

import { ReactNode } from 'react';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import { persistor, store } from './index';

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
