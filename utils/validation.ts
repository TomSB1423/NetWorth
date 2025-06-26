// utils/validation.ts
// Shared validation functions for accounts and transactions

import { Account, Transaction } from '../services/accountMockService';
import { isValidAccountName, isValidAmount, isValidTransactionDate, sanitizeString } from './security';

export const validateAccount = (account: Partial<Account>): string | null => {
  if (!account.name) {
    return 'Account name is required';
  }
  
  const sanitizedName = sanitizeString(account.name);
  if (!isValidAccountName(sanitizedName)) {
    return 'Account name contains invalid characters or is too long';
  }
  
  if (!account.type) {
    return 'Account type is required';
  }
  
  const validTypes = ['checking', 'savings', 'credit', 'mortgage', 'investment'];
  if (!validTypes.includes(account.type)) {
    return 'Invalid account type';
  }
  
  if (account.creditLimit !== undefined) {
    if (!isValidAmount(account.creditLimit)) {
      return 'Credit limit must be a valid number';
    }
    if (account.creditLimit < 0 || account.creditLimit > 1000000) {
      return 'Credit limit must be between $0 and $1,000,000';
    }
  }
  
  if (account.interestRate !== undefined) {
    if (!isValidAmount(account.interestRate)) {
      return 'Interest rate must be a valid number';
    }
    if (account.interestRate < 0 || account.interestRate > 1) {
      return 'Interest rate must be between 0% and 100%';
    }
  }
  
  return null;
};

export const validateTransaction = (transaction: Partial<Transaction>): string | null => {
  if (!transaction.description) {
    return 'Transaction description is required';
  }
  
  const sanitizedDescription = sanitizeString(transaction.description);
  if (sanitizedDescription.length === 0) {
    return 'Transaction description is required';
  }
  if (sanitizedDescription.length > 200) {
    return 'Transaction description must be less than 200 characters';
  }
  
  if (transaction.amount === undefined || transaction.amount === null) {
    return 'Transaction amount is required';
  }
  if (!isValidAmount(transaction.amount)) {
    return 'Transaction amount must be a valid number';
  }
  if (Math.abs(transaction.amount) > 1000000) {
    return 'Transaction amount must be less than $1,000,000';
  }
  
  if (!transaction.date) {
    return 'Transaction date is required';
  }
  if (!isValidTransactionDate(transaction.date)) {
    return 'Transaction date is invalid or in the future';
  }
  
  return null;
};
