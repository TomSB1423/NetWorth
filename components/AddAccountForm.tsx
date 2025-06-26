import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Modal,
  ScrollView,
  ActivityIndicator,
} from 'react-native';
import { usePalette } from '../hooks/usePalette';
import { Account, AccountType } from '../services/accountMockService';
import { sanitizeString, isValidAccountName } from '../utils/security';

interface AddAccountFormProps {
  visible: boolean;
  onClose: () => void;
  onAddAccount: (account: Omit<Account, 'id' | 'transactions'>) => Promise<void>;
}

const accountTypeOptions: { value: AccountType; label: string; description: string }[] = [
  { value: 'checking', label: 'Checking Account', description: 'For daily transactions and bill payments' },
  { value: 'savings', label: 'Savings Account', description: 'For saving money and earning interest' },
  { value: 'credit', label: 'Credit Card', description: 'For credit purchases and building credit' },
  { value: 'mortgage', label: 'Mortgage', description: 'For home loan payments' },
  { value: 'investment', label: 'Investment Account', description: 'For stocks, bonds, and other investments' },
];

export default function AddAccountForm({ visible, onClose, onAddAccount }: AddAccountFormProps) {
  const colors = usePalette();
  const [name, setName] = useState('');
  const [selectedType, setSelectedType] = useState<AccountType>('checking');
  const [creditLimit, setCreditLimit] = useState('');
  const [interestRate, setInterestRate] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};
    
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

  const resetForm = () => {
    setName('');
    setSelectedType('checking');
    setCreditLimit('');
    setInterestRate('');
    setErrors({});
    setIsSubmitting(false);
  };

  const handleClose = () => {
    resetForm();
    onClose();
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
        ...(selectedType === 'credit' && creditLimit && { creditLimit: parseFloat(creditLimit) }),
        ...(interestRate && { interestRate: parseFloat(interestRate) / 100 }), // Convert percentage to decimal
      };

      await onAddAccount(newAccount);
      handleClose();
    } catch (error) {
      // Error is handled by the global error state
      console.error('Failed to add account:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const shouldShowCreditLimit = selectedType === 'credit';
  const shouldShowInterestRate = ['savings', 'credit', 'mortgage'].includes(selectedType);

  return (
    <Modal visible={visible} animationType="slide" presentationStyle="pageSheet">
      <View style={[styles.container, { backgroundColor: colors.background }]}>
        <View style={[styles.header, { borderBottomColor: colors.border }]}>
          <TouchableOpacity onPress={handleClose}>
            <Text style={[styles.cancelButton, { color: colors.primary }]}>Cancel</Text>
          </TouchableOpacity>
          <Text style={[styles.title, { color: colors.text }]}>Add Account</Text>
          <TouchableOpacity onPress={handleSubmit} disabled={isSubmitting}>
            <View style={styles.saveButtonContent}>
              {isSubmitting && (
                <ActivityIndicator 
                  size="small" 
                  color={colors.primary} 
                  style={styles.saveButtonSpinner}
                />
              )}
              <Text style={[styles.saveButton, { color: colors.primary }]}>
                {isSubmitting ? 'Saving...' : 'Save'}
              </Text>
            </View>
          </TouchableOpacity>
        </View>

        <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
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
            <Text style={[styles.label, { color: colors.text }]}>Account Type *</Text>
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
      </View>
    </Modal>
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
    borderBottomWidth: 1,
  },
  cancelButton: {
    fontSize: 16,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  saveButton: {
    fontSize: 16,
    fontWeight: '600',
  },
  content: {
    flex: 1,
    padding: 20,
  },
  section: {
    marginBottom: 24,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 8,
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
  saveButtonContent: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  saveButtonSpinner: {
    marginRight: 8,
  },
});
