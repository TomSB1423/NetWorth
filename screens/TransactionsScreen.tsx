import { StyleSheet, View } from "react-native";
import BankTransactions from "../components/BankTransactions";
import LoadingSpinner from "../components/LoadingSpinner";
import { useColorSchemeToggle } from "../hooks/useColorSchemeToggle";
import { useAccounts, useAppLoading } from "../store/hooks";

export default function TransactionsScreen() {
  const [colorScheme] = useColorSchemeToggle();
  const accounts = useAccounts();
  const isLoading = useAppLoading();

  if (isLoading) {
    return <LoadingSpinner message="Loading transactions..." />;
  }

  const checkingAccount = accounts.find(account => account.type === 'checking') || accounts[0];
  const transactions = checkingAccount?.transactions || [];

  return (
    <View style={styles.container}>
      <BankTransactions transactions={transactions.slice().reverse()} colorScheme={colorScheme} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: 32,
    paddingHorizontal: 20,
    backgroundColor: 'transparent',
  },
});
