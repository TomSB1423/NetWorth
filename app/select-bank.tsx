import { router } from 'expo-router';
import React, { useState } from 'react';
import {
  FlatList,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View
} from 'react-native';
import { usePalette } from '../hooks/usePalette';

interface BankOption {
  value: string;
  label: string;
  emoji: string;
}

const bankOptions: BankOption[] = [
  { value: 'chase', label: 'Chase', emoji: '🏦' },
  { value: 'hsbc', label: 'HSBC', emoji: '🌐' },
  { value: 'barclays', label: 'Barclays', emoji: '🔷' },
  { value: 'bank_of_america', label: 'Bank of America', emoji: '🇺🇸' },
  { value: 'wells_fargo', label: 'Wells Fargo', emoji: '🏛️' },
  { value: 'citibank', label: 'Citibank', emoji: '🏙️' },
  { value: 'santander', label: 'Santander', emoji: '🟥' },
  { value: 'capital_one', label: 'Capital One', emoji: '💳' },
  { value: 'td_bank', label: 'TD Bank', emoji: '🟢' },
  { value: 'pnc', label: 'PNC Bank', emoji: '🟡' },
  { value: 'american_express', label: 'American Express', emoji: '💳' },
  { value: 'discover', label: 'Discover', emoji: '🔍' },
  { value: 'usaa', label: 'USAA', emoji: '🇺🇸' },
  { value: 'navy_federal', label: 'Navy Federal Credit Union', emoji: '⚓' },
  { value: 'ally', label: 'Ally Bank', emoji: '🤝' },
  { value: 'schwab', label: 'Charles Schwab', emoji: '📈' },
  { value: 'fidelity', label: 'Fidelity', emoji: '📊' },
  { value: 'vanguard', label: 'Vanguard', emoji: '📈' },
  { value: 'other', label: 'Other', emoji: '🏛️' },
];

export default function BankSelectionPage() {
  const colors = usePalette();
  const [searchQuery, setSearchQuery] = useState('');
  const [filteredBanks, setFilteredBanks] = useState(bankOptions);

  const handleSearch = (query: string) => {
    setSearchQuery(query);
    if (query.trim() === '') {
      setFilteredBanks(bankOptions);
    } else {
      const filtered = bankOptions.filter(bank =>
        bank.label.toLowerCase().includes(query.toLowerCase())
      );
      setFilteredBanks(filtered);
    }
  };

  const handleBankSelect = (bank: BankOption) => {
    // Navigate to add account with the selected bank
    router.push({
      pathname: '/add-account',
      params: { 
        selectedBank: bank.value,
        bankLabel: bank.label,
        bankEmoji: bank.emoji
      }
    });
  };

  const renderBankItem = ({ item }: { item: BankOption }) => (
    <TouchableOpacity
      style={[styles.bankItem, { backgroundColor: colors.card, borderColor: colors.border }]}
      onPress={() => handleBankSelect(item)}
      activeOpacity={0.7}
    >
      <View style={styles.bankContent}>
        <Text style={styles.bankEmoji}>{item.emoji}</Text>
        <Text style={[styles.bankLabel, { color: colors.text }]}>{item.label}</Text>
      </View>
      <Text style={[styles.chevron, { color: colors.secondaryText }]}>›</Text>
    </TouchableOpacity>
  );

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      {/* Header */}
      <View style={[styles.header, { backgroundColor: colors.card, borderBottomColor: colors.border }]}>
        <TouchableOpacity onPress={() => router.back()}>
          <Text style={[styles.cancelButton, { color: colors.primary }]}>Cancel</Text>
        </TouchableOpacity>
        <Text style={[styles.title, { color: colors.text }]}>Select Bank</Text>
        <View style={styles.placeholder} />
      </View>

      {/* Search Bar */}
      <View style={styles.searchContainer}>
        <TextInput
          style={[
            styles.searchInput,
            {
              backgroundColor: colors.card,
              borderColor: colors.border,
              color: colors.text,
            },
          ]}
          value={searchQuery}
          onChangeText={handleSearch}
          placeholder="Search banks..."
          placeholderTextColor={colors.secondaryText}
          autoCapitalize="none"
          autoCorrect={false}
        />
      </View>

      {/* Bank List */}
      <FlatList
        data={filteredBanks}
        renderItem={renderBankItem}
        keyExtractor={(item) => item.value}
        style={styles.list}
        contentContainerStyle={styles.listContent}
        showsVerticalScrollIndicator={false}
        ItemSeparatorComponent={() => <View style={[styles.separator, { backgroundColor: colors.border }]} />}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: 20,
    paddingVertical: 16,
    paddingTop: 60, // Account for status bar
    borderBottomWidth: 1,
  },
  cancelButton: {
    fontSize: 16,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  placeholder: {
    width: 60, // Same width as cancel button for centering
  },
  searchContainer: {
    padding: 20,
  },
  searchInput: {
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
  },
  list: {
    flex: 1,
  },
  listContent: {
    paddingHorizontal: 20,
  },
  bankItem: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 16,
    paddingHorizontal: 16,
    borderRadius: 8,
    marginVertical: 4,
    borderWidth: 1,
  },
  bankContent: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
  },
  bankEmoji: {
    fontSize: 24,
    marginRight: 12,
  },
  bankLabel: {
    fontSize: 16,
    fontWeight: '500',
  },
  chevron: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  separator: {
    height: 1,
    marginHorizontal: 16,
  },
});
