import React, { useState, useEffect } from "react";
import { StyleSheet, Text, TouchableOpacity, View, Alert, Platform } from "react-native";
import AccountsList from "../components/AccountsList";
import AddAccountForm from "../components/AddAccountForm";
import BankTransactions from "../components/BankTransactions";
import LoadingSpinner from "../components/LoadingSpinner";
import { useColorSchemeToggle } from "../hooks/useColorSchemeToggle";
import { usePalette } from "../hooks/usePalette";
import { Account } from "../services/accountMockService";
import { 
  useAppDispatch, 
  useAccounts, 
  useAppLoading, 
  useAccountsError,
  useAddAccountFormVisible,
  useHasEverHadAccounts
} from "../store/hooks";
import { addAccount, clearError, removeAccount } from "../store/slices/accountsSlice";
import { setAddAccountFormVisible } from "../store/slices/uiSlice";

export default function AccountsScreen() {
  const colors = usePalette();
  const [colorScheme] = useColorSchemeToggle();
  const [selectedAccount, setSelectedAccount] = useState<Account | null>(null);
  const dispatch = useAppDispatch();
  
  // Use Redux state
  const accounts = useAccounts();
  const isLoading = useAppLoading();
  const error = useAccountsError();
  const showAddForm = useAddAccountFormVisible();
  const hasEverHadAccounts = useHasEverHadAccounts();

  // Show error if any
  useEffect(() => {
    if (error) {
      Alert.alert(
        'Error',
        error,
        [
          {
            text: 'OK',
            onPress: () => dispatch(clearError()),
          },
        ]
      );
    }
  }, [error, dispatch]);

  // Automatically open add account form only if no accounts exist on initial load
  useEffect(() => {
    // Only auto-open if user has never had accounts before (true onboarding)
    if (!isLoading && accounts.length === 0 && !showAddForm && !hasEverHadAccounts) {
      dispatch(setAddAccountFormVisible(true));
    }
  }, [accounts.length, isLoading, showAddForm, hasEverHadAccounts, dispatch]);

  const handleAccountSelect = (account: Account) => {
    setSelectedAccount(account);
  };

  const handleBackToAccounts = () => {
    setSelectedAccount(null);
  };

  const handleAddAccount = async (accountData: Omit<Account, 'id' | 'transactions'>) => {
    try {
      await dispatch(addAccount(accountData)).unwrap();
      dispatch(setAddAccountFormVisible(false));
    } catch (error) {
      // Error is handled by the global error state
      console.error('Failed to add account:', error);
    }
  };

  const handleDeleteAccount = async (accountId: string) => {
    try {
      await dispatch(removeAccount(accountId)).unwrap();
      // If the currently selected account was deleted, go back to accounts list
      if (selectedAccount && selectedAccount.id === accountId) {
        setSelectedAccount(null);
      }
    } catch (error) {
      // Error is handled by the global error state
      console.error('Failed to delete account:', error);
    }
  };

  // Show loading spinner while data is being fetched
  if (isLoading) {
    return <LoadingSpinner message="Loading accounts..." />;
  }

  if (selectedAccount) {
    // Show transactions for the selected account
    return (
      <View style={[styles.container, { backgroundColor: colors.background }]}>
        <View style={styles.header}>
          <TouchableOpacity 
            onPress={handleBackToAccounts}
            style={[styles.backButton, { backgroundColor: colors.card }]}
          >
            <Text style={[styles.backButtonText, { color: colors.primary }]}>← Back</Text>
          </TouchableOpacity>
          <View style={styles.accountHeader}>
            <Text style={[styles.accountTitle, { color: colors.text }]}>{selectedAccount.name}</Text>
            <Text style={[styles.accountSubtitle, { color: colors.secondaryText }]}>
              {selectedAccount.type === 'checking' ? 'Checking Account' :
               selectedAccount.type === 'savings' ? 'Savings Account' :
               selectedAccount.type === 'credit' ? 'Credit Card' :
               selectedAccount.type === 'mortgage' ? 'Mortgage' :
               selectedAccount.type === 'investment' ? 'Investment Account' :
               selectedAccount.type}
            </Text>
          </View>
        </View>
        <BankTransactions 
          transactions={selectedAccount.transactions.slice().reverse()} 
          colorScheme={colorScheme} 
        />
      </View>
    );
  }

  // Show accounts list
  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <View style={styles.accountsHeader}>
        <Text style={[styles.headerTitle, { color: colors.text }]}>Accounts</Text>
        <TouchableOpacity
          style={[styles.addButton, { backgroundColor: colors.primary }]}
          onPress={() => dispatch(setAddAccountFormVisible(true))}
        >
          <Text style={[styles.addButtonText, { color: colors.background }]}>+ Add</Text>
        </TouchableOpacity>
      </View>
      
      <AccountsList 
        accounts={accounts}
        onAccountSelect={handleAccountSelect}
        onAccountDelete={handleDeleteAccount}
      />
      
      <AddAccountForm
        visible={showAddForm}
        onClose={() => dispatch(setAddAccountFormVisible(false))}
        onAddAccount={handleAddAccount}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: Platform.OS === 'android' ? 32 : Platform.OS === 'ios' ? 44 : 0,
    paddingHorizontal: 20,
    backgroundColor: 'transparent',
  },
  header: {
    marginBottom: 20,
  },
  backButton: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
    alignSelf: 'flex-start',
    marginBottom: 16,
  },
  backButtonText: {
    fontSize: 16,
    fontWeight: '600',
  },
  accountHeader: {
    alignItems: 'center',
  },
  accountTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  accountSubtitle: {
    fontSize: 16,
    textTransform: 'capitalize',
  },
  accountsHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  headerTitle: {
    fontSize: 28,
    fontWeight: 'bold',
  },
  addButton: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
  },
  addButtonText: {
    fontSize: 16,
    fontWeight: '600',
  },
});
