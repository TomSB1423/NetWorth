import { Ionicons } from '@expo/vector-icons';
import { router, useLocalSearchParams } from 'expo-router';
import React, { useState } from 'react';
import {
  ActivityIndicator,
  Alert,
  ScrollView,
  StyleSheet,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from 'react-native';
import { usePalette } from '../hooks/usePalette';
import { Account, AccountType } from '../services/accountMockService';
import { useAppDispatch } from '../store/hooks';
import { addAccount } from '../store/slices/accountsSlice';
import { isValidAccountName, sanitizeString } from '../utils/security';

const accountTypeOptions: { value: AccountType; label: string; description: string }[] = [
  { value: 'checking', label: 'Current Account', description: 'For daily transactions and bill payments' },
  { value: 'savings', label: 'Savings Account', description: 'For saving money and earning interest' },
  { value: 'credit', label: 'Credit Card', description: 'For credit purchases and building credit' },
  { value: 'mortgage', label: 'Mortgage', description: 'For home loan payments' },
  { value: 'investment', label: 'Investment Account', description: 'For stocks, bonds, and other investments' },
];

export default function AddAccountPage() {
  const colors = usePalette();
  const dispatch = useAppDispatch();
  const params = useLocalSearchParams();
  
  // Get bank info from navigation params
  const selectedBank = params.selectedBank as string || '';
  const bankLabel = params.bankLabel as string || '';
  const bankEmoji = params.bankEmoji as string || '';
  
  // Initialize name with default value to avoid useEffect infinite loops
  // Using function initializer ensures this only runs once during component mount
  const [name, setName] = useState(() => bankLabel ? `${bankLabel} Account` : '');
  const [selectedType, setSelectedType] = useState<AccountType>('checking');
  const [creditLimit, setCreditLimit] = useState('');
  const [interestRate, setInterestRate] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};
    
    // Validate bank selection
    if (!selectedBank) {
      newErrors.bank = 'Please select a bank';
    }
    
    // Validate account name
    const sanitizedName = sanitizeString(name);
    if (!sanitizedName) {
      newErrors.name = 'Account name is required';
    } else if (!isValidAccountName(sanitizedName)) {
      newErrors.name = 'Account name contains invalid characters';
    }
    
    // Validate credit limit for credit cards
    if (selectedType === 'credit' && creditLimit) {
      const limit = parseFloat(creditLimit);
      if (isNaN(limit) || limit < 0) {
        newErrors.creditLimit = 'Please enter a valid credit limit';
      } else if (limit > 1000000) {
        newErrors.creditLimit = 'Credit limit cannot exceed $1,000,000';
      }
    }
    
    // Validate interest rate
    if (interestRate) {
      const rate = parseFloat(interestRate);
      if (isNaN(rate) || rate < 0 || rate > 100) {
        newErrors.interestRate = 'Please enter a valid interest rate (0-100%)';
      }
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    if (isSubmitting) return;
    
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    
    try {
      const sanitizedName = sanitizeString(name);
      const newAccount: Omit<Account, 'id' | 'transactions'> = {
        name: sanitizedName,
        type: selectedType,
        emoji: bankEmoji,
        bank: selectedBank,
        ...(selectedType === 'credit' && creditLimit && { creditLimit: parseFloat(creditLimit) }),
        ...(interestRate && { interestRate: parseFloat(interestRate) / 100 }), // Convert percentage to decimal
      };

      await dispatch(addAccount(newAccount)).unwrap();
      
      // Navigate directly to dashboard after successful account creation
      router.replace('/(tabs)/dashboard');
    } catch (error) {
      console.error('Failed to add account:', error);
      Alert.alert(
        'Error',
        'Failed to create account. Please try again.',
        [{ text: 'OK' }]
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const shouldShowCreditLimit = selectedType === 'credit';
  const shouldShowInterestRate = ['savings', 'credit', 'mortgage'].includes(selectedType);

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      {/* Header */}
      <View style={[styles.header, { backgroundColor: colors.background, borderBottomColor: colors.border }]}>
        <TouchableOpacity 
          style={styles.backButton}
          onPress={() => router.back()}
        >
          <Ionicons name="arrow-back" size={24} color={colors.text} />
        </TouchableOpacity>
        <Text style={[styles.title, { color: colors.text }]}>Add Account</Text>
        <View style={styles.headerRight} />
      </View>

      <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
        {/* Selected Bank Display */}
        <View style={styles.section}>
          <Text style={[styles.label, { color: colors.text }]}>Bank *</Text>
          {selectedBank ? (
            <TouchableOpacity
              style={[styles.selectedBankContainer, { backgroundColor: colors.card, borderColor: colors.border }]}
              onPress={() => router.push('/select-bank')}
            >
              <View style={styles.selectedBankContent}>
                <Text style={styles.selectedBankEmoji}>{bankEmoji}</Text>
                <Text style={[styles.selectedBankLabel, { color: colors.text }]}>{bankLabel}</Text>
              </View>
              <Text style={[styles.changeText, { color: colors.primary }]}>Change</Text>
            </TouchableOpacity>
          ) : (
            <TouchableOpacity
              style={[styles.selectBankButton, { backgroundColor: colors.card, borderColor: colors.border }]}
              onPress={() => router.push('/select-bank')}
            >
              <Text style={[styles.selectBankText, { color: colors.secondaryText }]}>Select Bank</Text>
              <Text style={[styles.chevron, { color: colors.secondaryText }]}>›</Text>
            </TouchableOpacity>
          )}
          {errors.bank && (
            <Text style={[styles.errorText, { color: '#dc3545' }]}>
              {errors.bank}
            </Text>
          )}
        </View>

        {/* Account Name */}
        <View style={styles.section}>
          <Text style={[styles.label, { color: colors.text }]}>Account Name *</Text>
          <TextInput
            style={[
              styles.input,
              {
                backgroundColor: colors.card,
                borderColor: errors.name ? '#dc3545' : colors.border,
                color: colors.text,
              },
            ]}
            value={name}
            onChangeText={(text) => {
              setName(text);
              if (errors.name) {
                setErrors(prev => ({ ...prev, name: '' }));
              }
            }}
            placeholder="Enter account name"
            placeholderTextColor={colors.secondaryText}
            autoCapitalize="words"
            maxLength={100}
            editable={!isSubmitting}
          />
          {errors.name && (
            <Text style={[styles.errorText, { color: '#dc3545' }]}>
              {errors.name}
            </Text>
          )}
        </View>

        {/* Account Type */}
        <View style={styles.section}>
          <Text style={[styles.label, { color: colors.text }]}>Account Category *</Text>
          {accountTypeOptions.map((option) => (
            <TouchableOpacity
              key={option.value}
              style={[
                styles.typeOption,
                {
                  backgroundColor: selectedType === option.value ? colors.primary + '20' : colors.card,
                  borderColor: selectedType === option.value ? colors.primary : colors.border,
                },
              ]}
              onPress={() => setSelectedType(option.value)}
            >
              <View style={styles.typeContent}>
                <Text style={[styles.typeLabel, { color: colors.text }]}>{option.label}</Text>
                <Text style={[styles.typeDescription, { color: colors.secondaryText }]}>
                  {option.description}
                </Text>
              </View>
              <View
                style={[
                  styles.radioButton,
                  {
                    borderColor: selectedType === option.value ? colors.primary : colors.border,
                    backgroundColor: selectedType === option.value ? colors.primary : 'transparent',
                  },
                ]}
              >
                {selectedType === option.value && (
                  <View style={[styles.radioButtonInner, { backgroundColor: colors.background }]} />
                )}
              </View>
            </TouchableOpacity>
          ))}
        </View>

        {/* Credit Limit (for credit cards) */}
        {shouldShowCreditLimit && (
          <View style={styles.section}>
            <Text style={[styles.label, { color: colors.text }]}>Credit Limit</Text>
            <TextInput
              style={[
                styles.input,
                {
                  backgroundColor: colors.card,
                  borderColor: errors.creditLimit ? '#dc3545' : colors.border,
                  color: colors.text,
                },
              ]}
              value={creditLimit}
              onChangeText={(text) => {
                setCreditLimit(text);
                if (errors.creditLimit) {
                  setErrors(prev => ({ ...prev, creditLimit: '' }));
                }
              }}
              placeholder="Enter credit limit"
              placeholderTextColor={colors.secondaryText}
              keyboardType="numeric"
              editable={!isSubmitting}
            />
            {errors.creditLimit && (
              <Text style={[styles.errorText, { color: '#dc3545' }]}>
                {errors.creditLimit}
              </Text>
            )}
          </View>
        )}

        {/* Interest Rate */}
        {shouldShowInterestRate && (
          <View style={styles.section}>
            <Text style={[styles.label, { color: colors.text }]}>
              Interest Rate (
              {selectedType === 'savings' || selectedType === 'investment' ? 'APY' : 'APR'}
              )
            </Text>
            <TextInput
              style={[
                styles.input,
                {
                  backgroundColor: colors.card,
                  borderColor: errors.interestRate ? '#dc3545' : colors.border,
                  color: colors.text,
                },
              ]}
              value={interestRate}
              onChangeText={(text) => {
                setInterestRate(text);
                if (errors.interestRate) {
                  setErrors(prev => ({ ...prev, interestRate: '' }));
                }
              }}
              placeholder="Enter interest rate (%)"
              placeholderTextColor={colors.secondaryText}
              keyboardType="numeric"
              editable={!isSubmitting}
            />
            {errors.interestRate && (
              <Text style={[styles.errorText, { color: '#dc3545' }]}>
                {errors.interestRate}
              </Text>
            )}
          </View>
        )}
      </ScrollView>

      {/* Submit Button */}
      <View style={[styles.submitContainer, { backgroundColor: colors.background, borderTopColor: colors.border }]}>
        <TouchableOpacity
          style={[
            styles.submitButton,
            {
              backgroundColor: (!selectedBank || !name) ? colors.border : colors.primary,
              opacity: (!selectedBank || !name) ? 0.6 : 1,
            },
          ]}
          onPress={handleSubmit}
          disabled={isSubmitting || !selectedBank || !name}
        >
          <View style={styles.submitButtonContent}>
            {isSubmitting && (
              <ActivityIndicator 
                size="small" 
                color={colors.background} 
                style={styles.submitButtonSpinner}
              />
            )}
            <Text style={[
              styles.submitButtonText, 
              { color: colors.background }
            ]}>
              {isSubmitting ? 'Creating Account...' : 'Create Account'}
            </Text>
          </View>
        </TouchableOpacity>
      </View>
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
  backButton: {
    padding: 4,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  headerRight: {
    width: 32, // Same width as back button for centering
  },
  content: {
    flex: 1,
    padding: 20,
  },
  section: {
    marginBottom: 32,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 12,
  },
  selectedBankContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    justifyContent: 'space-between',
  },
  selectedBankContent: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  selectedBankEmoji: {
    fontSize: 20,
    marginRight: 12,
  },
  selectedBankLabel: {
    fontSize: 16,
    fontWeight: '500',
  },
  changeText: {
    fontSize: 14,
    fontWeight: '600',
  },
  selectBankButton: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    justifyContent: 'space-between',
  },
  selectBankText: {
    fontSize: 16,
  },
  chevron: {
    fontSize: 20,
    fontWeight: 'bold',
  },
  input: {
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
  },
  typeOption: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderRadius: 8,
    padding: 16,
    marginBottom: 8,
  },
  typeContent: {
    flex: 1,
  },
  typeLabel: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 4,
  },
  typeDescription: {
    fontSize: 14,
  },
  radioButton: {
    width: 20,
    height: 20,
    borderRadius: 10,
    borderWidth: 2,
    alignItems: 'center',
    justifyContent: 'center',
  },
  radioButtonInner: {
    width: 8,
    height: 8,
    borderRadius: 4,
  },
  errorText: {
    fontSize: 14,
    marginTop: 4,
  },
  submitContainer: {
    padding: 20,
    paddingBottom: 40, // Extra padding for bottom safe area
    borderTopWidth: 1,
  },
  submitButton: {
    borderRadius: 12,
    paddingVertical: 16,
    alignItems: 'center',
    justifyContent: 'center',
  },
  submitButtonContent: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  submitButtonSpinner: {
    marginRight: 8,
  },
  submitButtonText: {
    fontSize: 16,
    fontWeight: '600',
  },
});
