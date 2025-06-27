import { Platform, StyleSheet, View } from "react-native";
import AccountsList from "../../components/AccountsList";
import LoadingSpinner from "../../components/LoadingSpinner";
import { useColorSchemeToggle } from "../../hooks/useColorSchemeToggle";
import { usePalette } from "../../hooks/usePalette";
import { Account } from "../../services/accountMockService";
import { useAccounts, useAppDispatch, useAppLoading } from "../../store/hooks";
import { removeAccount } from "../../store/slices/accountsSlice";

export default function AccountsScreen() {
  const [colorScheme] = useColorSchemeToggle();
  const accounts = useAccounts();
  const isLoading = useAppLoading();
  const dispatch = useAppDispatch();
  const colors = usePalette();

  if (isLoading) {
    return <LoadingSpinner message="Loading accounts..." />;
  }

  const handleAccountSelect = (account: Account) => {
    // You can implement account detail navigation here
    console.log('Selected account:', account.name);
  };

  const handleAccountDelete = (accountId: string) => {
    dispatch(removeAccount(accountId));
  };

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <AccountsList 
        accounts={accounts}
        onAccountSelect={handleAccountSelect}
        onAccountDelete={handleAccountDelete}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: Platform.OS === 'android' ? 60 : Platform.OS === 'ios' ? 80 : 20,
    paddingHorizontal: 20,
  },
});
