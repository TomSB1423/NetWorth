import React from "react";
import { Alert, ScrollView, StyleSheet, Text, TouchableOpacity, View } from "react-native";
import { usePalette } from "../../hooks/usePalette";
import { Account } from "../../services/accountMockService";

interface AccountsListProps {
  accounts: Account[];
  onAccountSelect?: (account: Account) => void;
  onAccountDelete?: (accountId: string) => void;
}

export default function AccountsList({ accounts, onAccountSelect, onAccountDelete }: AccountsListProps) {
  const colors = usePalette();

  const handleDeleteAccount = (account: Account) => {
    Alert.alert(
      "Delete Account",
      `Are you sure you want to delete "${account.name}"?\n\nThis action cannot be undone and will remove all transaction history for this account.`,
      [
        {
          text: "Cancel",
          style: "cancel",
        },
        {
          text: "Delete",
          style: "destructive",
          onPress: () => onAccountDelete?.(account.id),
        },
      ]
    );
  };

  // Helper function to format bank name for display
  const formatBankName = (bank: string) => {
    // Handle special cases
    const bankMap: Record<string, string> = {
      'bank_of_america': 'Bank of America',
      'wells_fargo': 'Wells Fargo',
      'td_bank': 'TD Bank',
      'capital_one': 'Capital One',
      'pnc': 'PNC Bank',
    };
    
    return bankMap[bank] || bank.charAt(0).toUpperCase() + bank.slice(1);
  };

  // Helper function to format account type for display
  const formatAccountType = (type: string) => {
    switch (type) {
      case 'checking': return 'Checking';
      case 'savings': return 'Savings';
      case 'credit': return 'Credit Card';
      case 'mortgage': return 'Mortgage';
      case 'investment': return 'Investment';
      default: return type;
    }
  };

  // Helper function to get account type color
  const getAccountTypeColor = (type: string) => {
    switch (type) {
      case 'checking': return colors.primary;
      case 'savings': return colors.success;
      case 'credit': return colors.warning;
      case 'mortgage': return colors.error;
      case 'investment': return colors.info;
      default: return colors.secondaryText;
    }
  };

  // Helper function to determine balance color - only red for negative
  const getBalanceColor = (balance: number) => {
    return balance < 0 ? colors.error : colors.text;
  };

  return (
    <View style={styles.listContainer}>
      <ScrollView 
        style={styles.scrollContainer}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        {accounts
          .filter(account => account.type !== 'mortgage') // Filter out mortgages
          .map(account => {
            // Calculate total balance from transactions
            let balance = account.transactions.reduce((sum, t) => sum + t.amount, 0);
            
            // For debt accounts (credit), show balance as negative to represent debt
            if (account.type === 'credit') {
              balance = -Math.abs(balance);
            }
            
            return (
              <View key={account.id} style={[styles.accountCard, { backgroundColor: colors.card }]}>
                <TouchableOpacity
                  onPress={() => onAccountSelect?.(account)}
                  activeOpacity={0.7}
                  style={styles.accountMainContent}
                >
                  <View style={styles.accountHeader}>
                    <View style={styles.accountInfo}>
                      <View style={styles.accountNameRow}>
                        {account.emoji && (
                          <Text style={styles.accountEmoji}>{account.emoji}</Text>
                        )}
                        <Text style={[styles.accountName, { color: colors.text }]}>{account.name}</Text>
                      </View>
                      <Text style={[
                        styles.accountType, 
                        { color: getAccountTypeColor(account.type) }
                      ]}>
                        {formatAccountType(account.type)}
                        {account.bank && ` • ${formatBankName(account.bank)}`}
                      </Text>
                    </View>
                    <Text 
                      style={[
                        styles.accountBalance, 
                        { color: getBalanceColor(balance) }
                      ]}
                    >
                      {balance < 0 ? '-$' : '$'}{Math.abs(balance).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                    </Text>
                  </View>
                  <View style={styles.accountFooter}>
                    <Text style={[styles.transactionCount, { color: colors.secondaryText }]}>
                      {account.transactions.length} transactions
                    </Text>
                    {account.creditLimit && (
                      <Text style={[styles.creditLimit, { color: colors.secondaryText }]}>
                        Limit: ${account.creditLimit.toLocaleString()}
                      </Text>
                    )}
                    {account.interestRate && (
                      <Text style={[styles.interestRate, { color: colors.secondaryText }]}>
                        {(account.interestRate * 100).toFixed(2)}% APR
                      </Text>
                    )}
                  </View>
                </TouchableOpacity>
                
                {/* Delete Button */}
                <TouchableOpacity
                  style={[styles.deleteButton, { backgroundColor: colors.error + '15', borderColor: colors.error + '30' }]}
                  onPress={() => handleDeleteAccount(account)}
                  activeOpacity={0.7}
                >
                  <Text style={[styles.deleteButtonText, { color: colors.error }]}>🗑️</Text>
                </TouchableOpacity>
              </View>
            );
          })}
      </ScrollView>

      {/* Liabilities Section */}
      <Text style={[styles.heading, { color: colors.text, marginTop: 24 }]}>Liabilities</Text>
      <ScrollView 
        style={styles.scrollContainer}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        {accounts
          .filter(account => account.type === 'mortgage') // Only mortgages
          .map(account => {
            // Calculate total balance from transactions
            let balance = account.transactions.reduce((sum, t) => sum + t.amount, 0);
            
            // For liabilities, show as negative to represent debt
            balance = -Math.abs(balance);
            
            return (
              <View key={account.id} style={[styles.accountCard, { backgroundColor: colors.card }]}>
                <TouchableOpacity
                  onPress={() => onAccountSelect?.(account)}
                  activeOpacity={0.7}
                  style={styles.accountMainContent}
                >
                  <View style={styles.accountHeader}>
                    <View style={styles.accountInfo}>
                      <View style={styles.accountNameRow}>
                        {account.emoji && (
                          <Text style={styles.accountEmoji}>{account.emoji}</Text>
                        )}
                        <Text style={[styles.accountName, { color: colors.text }]}>{account.name}</Text>
                      </View>
                      <Text style={[
                        styles.accountType, 
                        { color: getAccountTypeColor(account.type) }
                      ]}>
                        {formatAccountType(account.type)}
                        {account.bank && ` • ${formatBankName(account.bank)}`}
                      </Text>
                    </View>
                    <Text 
                      style={[
                        styles.accountBalance, 
                        { color: getBalanceColor(balance) }
                      ]}
                    >
                      {balance < 0 ? '-$' : '$'}{Math.abs(balance).toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                    </Text>
                  </View>
                  <View style={styles.accountFooter}>
                    <Text style={[styles.transactionCount, { color: colors.secondaryText }]}>
                      {account.transactions.length} payments
                    </Text>
                    {account.interestRate && (
                      <Text style={[styles.interestRate, { color: colors.secondaryText }]}>
                        {(account.interestRate * 100).toFixed(2)}% APR
                      </Text>
                    )}
                  </View>
                </TouchableOpacity>
                
                {/* Delete Button */}
                <TouchableOpacity
                  style={[styles.deleteButton, { backgroundColor: colors.error + '15', borderColor: colors.error + '30' }]}
                  onPress={() => handleDeleteAccount(account)}
                  activeOpacity={0.7}
                >
                  <Text style={[styles.deleteButtonText, { color: colors.error }]}>🗑️</Text>
                </TouchableOpacity>
              </View>
            );
          })}
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  listContainer: {
    width: "100%",
    marginTop: 24,
    flex: 1,
  },
  heading: {
    fontSize: 18,
    fontWeight: "bold",
    marginBottom: 12,
  },
  scrollContainer: {
    flex: 1,
    maxHeight: 400, // Limit height to ensure it's scrollable
  },
  scrollContent: {
    paddingBottom: 16,
  },
  accountCard: {
    borderRadius: 12,
    padding: 16,
    marginBottom: 12,
    flexDirection: 'row',
    alignItems: 'stretch',
  },
  accountMainContent: {
    flex: 1,
  },
  accountHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "flex-start",
    marginBottom: 8,
  },
  accountInfo: {
    flex: 1,
  },
  accountNameRow: {
    flexDirection: "row",
    alignItems: "center",
  },
  accountName: {
    fontSize: 16,
    fontWeight: "bold",
    marginBottom: 2,
  },
  accountEmoji: {
    fontSize: 16,
    marginRight: 8,
  },
  accountType: {
    fontSize: 12,
    fontWeight: "600",
    textTransform: "uppercase",
    letterSpacing: 0.5,
  },
  accountBalance: {
    fontSize: 16,
    fontWeight: "bold",
    textAlign: "right",
  },
  accountFooter: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    flexWrap: "wrap",
    gap: 8,
  },
  transactionCount: {
    fontSize: 12,
  },
  creditLimit: {
    fontSize: 12,
  },
  interestRate: {
    fontSize: 12,
  },
  deleteButton: {
    width: 36,
    height: 36,
    borderRadius: 8,
    justifyContent: 'center',
    alignItems: 'center',
    marginLeft: 12,
    borderWidth: 1,
  },
  deleteButtonText: {
    fontSize: 16,
  },
});
