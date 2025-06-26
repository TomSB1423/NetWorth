// examples/StateUsageExample.tsx
// Example component showing simple state management usage

import React from 'react';
import { StyleSheet, Text, TouchableOpacity, View } from 'react-native';
import {
    useAccounts,
    useAppDispatch,
    useAppError,
    useAppLoading,
    useTheme,
    useTotalNetWorth
} from '../hooks';
import { addAccount } from '../store/slices/accountsSlice';
import { setTheme } from '../store/slices/uiSlice';

/**
 * Example component demonstrating the simple state management API
 * Shows how to read state and dispatch actions with minimal boilerplate
 */
export default function StateUsageExample() {
  // Reading state is simple - just use the hooks
  const accounts = useAccounts();
  const theme = useTheme();
  const totalNetWorth = useTotalNetWorth();
  const isLoading = useAppLoading();
  const error = useAppError();
  
  // Dispatching actions is also simple
  const dispatch = useAppDispatch();
  
  const handleAddAccount = () => {
    dispatch(addAccount({
      name: 'Example Account',
      type: 'checking'
    }));
  };
  
  const handleToggleTheme = () => {
    const newTheme = theme === 'light' ? 'dark' : 'light';
    dispatch(setTheme(newTheme));
  };
  
  // Loading and error handling
  if (isLoading) {
    return <Text>Loading...</Text>;
  }
  
  if (error) {
    return <Text>Error: {error}</Text>;
  }
  
  return (
    <View style={styles.container}>
      <Text style={styles.title}>State Management Example</Text>
      
      <Text>Theme: {theme}</Text>
      <Text>Accounts: {accounts.length}</Text>
      <Text>Net Worth: ${totalNetWorth.toFixed(2)}</Text>
      
      <TouchableOpacity style={styles.button} onPress={handleAddAccount}>
        <Text>Add Account</Text>
      </TouchableOpacity>
      
      <TouchableOpacity style={styles.button} onPress={handleToggleTheme}>
        <Text>Toggle Theme</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    padding: 20,
  },
  title: {
    fontSize: 20,
    fontWeight: 'bold',
    marginBottom: 20,
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 12,
    borderRadius: 8,
    marginVertical: 8,
    alignItems: 'center',
  },
});
