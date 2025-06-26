// accountService.ts
// Mock implementation of accounts and transactions with realistic data

import { TransactionCategory } from './metricsService';

export type AccountType = 'checking' | 'savings' | 'credit' | 'mortgage' | 'investment';

export type Transaction = {
  id: string;
  date: string; // ISO date string
  amount: number; // positive for income/credits, negative for expenses/debits
  description: string;
  category: TransactionCategory;
};

export type Account = {
  id: string;
  name: string;
  type: AccountType;
  transactions: Transaction[];
  creditLimit?: number; // for credit accounts
  interestRate?: number; // for savings/credit/mortgage
  emoji?: string; // optional emoji for the account
  bank?: string; // bank identifier
};

interface TransactionPattern {
  category: TransactionCategory;
  descriptions: string[];
  amountRange: [number, number];
  frequency: 'daily' | 'weekly' | 'monthly' | 'quarterly' | 'random';
  probability: number; // 0-1, for random frequency
}

export class AccountMockService {
  private accounts: Account[];

  constructor() {
    // Start with no accounts - users will add their own
    this.accounts = [];
  }

  private generateAccounts(months: number): Account[] {
    const accounts: Account[] = [];

    // 1. Primary Checking Account
    accounts.push(this.generateCheckingAccount('1', 'Primary Checking', months));
    
    // 2. High-Yield Savings Account
    accounts.push(this.generateSavingsAccount('2', 'High-Yield Savings', months));
    
    // 3. Credit Card Account
    accounts.push(this.generateCreditAccount('3', 'Visa Credit Card', months));
    
    // 4. Mortgage Account
    accounts.push(this.generateMortgageAccount('4', 'Home Mortgage', months));
    
    // 5. Investment Account
    accounts.push(this.generateInvestmentAccount('5', 'Investment Portfolio', months));

    return accounts;
  }

  private generateCheckingAccount(id: string, name: string, months: number): Account {
    const transactions: Transaction[] = [];
    const patterns: TransactionPattern[] = [
      // Income
      { category: TransactionCategory.SALARY, descriptions: ['Monthly Salary', 'Payroll Deposit'], amountRange: [3800, 4200], frequency: 'monthly', probability: 1 },
      { category: TransactionCategory.FREELANCE, descriptions: ['Freelance Payment', 'Consulting Fee'], amountRange: [500, 1500], frequency: 'random', probability: 0.3 },
      
      // Regular Expenses
      { category: TransactionCategory.RENT, descriptions: ['Monthly Rent'], amountRange: [-1200, -1200], frequency: 'monthly', probability: 1 },
      { category: TransactionCategory.UTILITIES, descriptions: ['Electric Bill', 'Gas Bill', 'Water Bill', 'Internet Bill'], amountRange: [-80, -200], frequency: 'monthly', probability: 0.8 },
      { category: TransactionCategory.GROCERIES, descriptions: ['Whole Foods', 'Trader Joes', 'Safeway', 'Local Market'], amountRange: [-50, -150], frequency: 'weekly', probability: 0.9 },
      { category: TransactionCategory.GAS, descriptions: ['Shell Gas Station', 'Chevron', 'BP Gas'], amountRange: [-40, -80], frequency: 'weekly', probability: 0.7 },
      { category: TransactionCategory.RESTAURANTS, descriptions: ['Starbucks', 'Chipotle', 'Local Restaurant', 'Pizza Place'], amountRange: [-15, -60], frequency: 'random', probability: 0.4 },
      
      // Lifestyle
      { category: TransactionCategory.SUBSCRIPTIONS, descriptions: ['Netflix', 'Spotify', 'Amazon Prime', 'Gym Membership'], amountRange: [-10, -50], frequency: 'monthly', probability: 0.6 },
      { category: TransactionCategory.SHOPPING, descriptions: ['Amazon Purchase', 'Target', 'Best Buy', 'Clothing Store'], amountRange: [-25, -200], frequency: 'random', probability: 0.3 },
      { category: TransactionCategory.ENTERTAINMENT, descriptions: ['Movie Theater', 'Concert Ticket', 'Uber Ride'], amountRange: [-20, -100], frequency: 'random', probability: 0.2 },
      
      // Special Events & Bulk Purchases
      { category: TransactionCategory.WEDDING, descriptions: ['Wedding Gift', 'Wedding Attire', 'Wedding Travel', 'Bachelor/Bachelorette Party'], amountRange: [-150, -800], frequency: 'random', probability: 0.05 },
      { category: TransactionCategory.BIRTHDAY, descriptions: ['Birthday Gift', 'Birthday Party Supplies', 'Birthday Dinner', 'Birthday Celebration'], amountRange: [-50, -300], frequency: 'random', probability: 0.15 },
      { category: TransactionCategory.FUNERAL, descriptions: ['Funeral Flowers', 'Memorial Donation', 'Funeral Travel', 'Sympathy Gift'], amountRange: [-75, -400], frequency: 'random', probability: 0.03 },
      { category: TransactionCategory.ANNIVERSARY, descriptions: ['Anniversary Gift', 'Anniversary Dinner', 'Anniversary Trip'], amountRange: [-100, -500], frequency: 'random', probability: 0.08 },
      { category: TransactionCategory.HOLIDAY, descriptions: ['Christmas Gifts', 'Holiday Decorations', 'Thanksgiving Dinner', 'Holiday Travel'], amountRange: [-200, -1200], frequency: 'random', probability: 0.12 },
      { category: TransactionCategory.GRADUATION, descriptions: ['Graduation Gift', 'Graduation Party', 'Graduation Ceremony'], amountRange: [-100, -600], frequency: 'random', probability: 0.04 },
    ];

    return {
      id,
      name,
      type: 'checking',
      transactions: this.generateTransactionsFromPatterns(patterns, months),
    };
  }

  private generateSavingsAccount(id: string, name: string, months: number): Account {
    const transactions: Transaction[] = [];
    const patterns: TransactionPattern[] = [
      { category: TransactionCategory.TRANSFER, descriptions: ['Transfer from Checking', 'Monthly Savings'], amountRange: [300, 800], frequency: 'monthly', probability: 0.8 },
      { category: TransactionCategory.TRANSFER, descriptions: ['Transfer to Checking', 'Emergency Withdrawal'], amountRange: [-200, -1000], frequency: 'random', probability: 0.1 },
      { category: TransactionCategory.INVESTMENT_RETURN, descriptions: ['Interest Payment'], amountRange: [15, 45], frequency: 'monthly', probability: 1 },
    ];

    return {
      id,
      name,
      type: 'savings',
      transactions: this.generateTransactionsFromPatterns(patterns, months),
      interestRate: 0.045, // 4.5% APY
    };
  }

  private generateCreditAccount(id: string, name: string, months: number): Account {
    const transactions: Transaction[] = [];
    const patterns: TransactionPattern[] = [
      // Payments (positive - paying down debt)
      { category: TransactionCategory.TRANSFER, descriptions: ['Payment from Checking', 'Auto Payment'], amountRange: [200, 800], frequency: 'monthly', probability: 0.9 },
      
      // Charges (negative - increasing debt)
      { category: TransactionCategory.GROCERIES, descriptions: ['Whole Foods', 'Costco'], amountRange: [-80, -200], frequency: 'weekly', probability: 0.3 },
      { category: TransactionCategory.GAS, descriptions: ['Shell', 'Chevron'], amountRange: [-45, -85], frequency: 'weekly', probability: 0.4 },
      { category: TransactionCategory.RESTAURANTS, descriptions: ['Restaurant Charge', 'Food Delivery'], amountRange: [-25, -80], frequency: 'random', probability: 0.4 },
      { category: TransactionCategory.SHOPPING, descriptions: ['Amazon', 'Online Purchase', 'Department Store'], amountRange: [-50, -300], frequency: 'random', probability: 0.3 },
      { category: TransactionCategory.TRAVEL, descriptions: ['Hotel Booking', 'Flight Ticket', 'Car Rental'], amountRange: [-200, -800], frequency: 'random', probability: 0.1 },
      { category: TransactionCategory.FEES, descriptions: ['Late Fee', 'Interest Charge'], amountRange: [-25, -35], frequency: 'random', probability: 0.1 },
      
      // Special Events & Bulk Purchases (often charged to credit)
      { category: TransactionCategory.WEDDING, descriptions: ['Wedding Venue Deposit', 'Wedding Dress/Tux', 'Wedding Catering', 'Wedding Photography'], amountRange: [-500, -2000], frequency: 'random', probability: 0.03 },
      { category: TransactionCategory.HOLIDAY, descriptions: ['Holiday Shopping Spree', 'Christmas Decorations', 'Holiday Electronics'], amountRange: [-300, -1500], frequency: 'random', probability: 0.08 },
      { category: TransactionCategory.BIRTHDAY, descriptions: ['Birthday Party Venue', 'Birthday Electronics Gift', 'Birthday Jewelry'], amountRange: [-100, -600], frequency: 'random', probability: 0.1 },
      { category: TransactionCategory.GRADUATION, descriptions: ['Graduation Laptop', 'Graduation Watch', 'Graduation Party Catering'], amountRange: [-200, -1200], frequency: 'random', probability: 0.02 },
    ];

    return {
      id,
      name,
      type: 'credit',
      transactions: this.generateTransactionsFromPatterns(patterns, months),
      creditLimit: 5000,
      interestRate: 0.1899, // 18.99% APR
    };
  }

  private generateMortgageAccount(id: string, name: string, months: number): Account {
    const transactions: Transaction[] = [];
    const patterns: TransactionPattern[] = [
      { category: TransactionCategory.MORTGAGE, descriptions: ['Monthly Mortgage Payment'], amountRange: [-1850, -1850], frequency: 'monthly', probability: 1 },
      { category: TransactionCategory.FEES, descriptions: ['Escrow Adjustment', 'Property Tax'], amountRange: [-150, -300], frequency: 'random', probability: 0.1 },
    ];

    return {
      id,
      name,
      type: 'mortgage',
      transactions: this.generateTransactionsFromPatterns(patterns, months),
      interestRate: 0.065, // 6.5% APR
    };
  }

  private generateInvestmentAccount(id: string, name: string, months: number): Account {
    const transactions: Transaction[] = [];
    const patterns: TransactionPattern[] = [
      { category: TransactionCategory.TRANSFER, descriptions: ['Monthly Investment', '401k Contribution'], amountRange: [400, 1000], frequency: 'monthly', probability: 0.8 },
      { category: TransactionCategory.INVESTMENT_RETURN, descriptions: ['Dividend Payment', 'Capital Gains'], amountRange: [50, 300], frequency: 'quarterly', probability: 0.7 },
      { category: TransactionCategory.INVESTMENT_RETURN, descriptions: ['Market Loss', 'Portfolio Adjustment'], amountRange: [-100, -500], frequency: 'random', probability: 0.2 },
      { category: TransactionCategory.FEES, descriptions: ['Management Fee', 'Trading Fee'], amountRange: [-10, -25], frequency: 'monthly', probability: 0.3 },
    ];

    return {
      id,
      name,
      type: 'investment',
      transactions: this.generateTransactionsFromPatterns(patterns, months),
    };
  }

  private generateTransactionsFromPatterns(patterns: TransactionPattern[], months: number): Transaction[] {
    const transactions: Transaction[] = [];
    const now = new Date();
    const days = months * 30;
    let idCounter = 1;

    for (let i = days - 1; i >= 0; i--) {
      const date = new Date(now);
      date.setDate(now.getDate() - i);
      
      for (const pattern of patterns) {
        let shouldGenerate = false;
        
        switch (pattern.frequency) {
          case 'daily':
            shouldGenerate = Math.random() < pattern.probability;
            break;
          case 'weekly':
            shouldGenerate = i % 7 === 0 && Math.random() < pattern.probability;
            break;
          case 'monthly':
            shouldGenerate = date.getDate() === 15 && Math.random() < pattern.probability;
            break;
          case 'quarterly':
            shouldGenerate = date.getDate() === 15 && date.getMonth() % 3 === 0 && Math.random() < pattern.probability;
            break;
          case 'random':
            shouldGenerate = Math.random() < (pattern.probability / 30); // Adjust for daily probability
            break;
        }

        if (shouldGenerate) {
          const description = pattern.descriptions[Math.floor(Math.random() * pattern.descriptions.length)];
          const amount = Math.round((
            pattern.amountRange[0] + 
            Math.random() * (pattern.amountRange[1] - pattern.amountRange[0])
          ) * 100) / 100;

          transactions.push({
            id: `${idCounter++}`,
            date: date.toISOString().slice(0, 10),
            amount,
            description,
            category: pattern.category,
          });
        }
      }
    }

    return transactions.sort((a, b) => a.date.localeCompare(b.date));
  }

  getAccounts(): Account[] {
    return this.accounts;
  }

  getAccountBalanceAtMonth(accountId: string, year: number, month: number): number {
    const account = this.accounts.find(a => a.id === accountId);
    if (!account) return 0;
    // Sum all transactions up to the end of the given month
    const endDate = new Date(year, month + 1, 0).toISOString().slice(0, 10);
    return account.transactions
      .filter(t => t.date <= endDate)
      .reduce((sum, t) => sum + t.amount, 0);
  }

  // Get current balance for an account
  getCurrentBalance(accountId: string): number {
    const account = this.accounts.find(a => a.id === accountId);
    if (!account) return 0;
    return account.transactions.reduce((sum, t) => sum + t.amount, 0);
  }

  // Get account by type
  getAccountsByType(type: AccountType): Account[] {
    return this.accounts.filter(a => a.type === type);
  }

  // Get total net worth across all accounts
  getTotalNetWorth(): number {
    return this.accounts.reduce((total, account) => {
      const balance = this.getCurrentBalance(account.id);
      // For debt accounts (credit, mortgage), negative balance means we owe money
      if (account.type === 'credit' || account.type === 'mortgage') {
        return total - Math.abs(balance); // Subtract debt from net worth
      }
      return total + balance;
    }, 0);
  }

  // Get transactions by category across all accounts
  getTransactionsByCategory(category: TransactionCategory): Transaction[] {
    const allTransactions: Transaction[] = [];
    this.accounts.forEach(account => {
      allTransactions.push(...account.transactions.filter(t => t.category === category));
    });
    return allTransactions.sort((a, b) => b.date.localeCompare(a.date));
  }

  // Add a new account
  addAccount(accountData: Omit<Account, 'id' | 'transactions'>): Account {
    const newId = (this.accounts.length > 0 ? Math.max(...this.accounts.map(a => parseInt(a.id))) + 1 : 1).toString();
    
    // Generate realistic transaction data for the new account
    const transactions = this.generateTransactionsForNewAccount(accountData.type, 12);
    
    const newAccount: Account = {
      ...accountData,
      id: newId,
      transactions: transactions,
    };
    
    this.accounts.push(newAccount);
    return newAccount;
  }

  // Remove an account
  removeAccount(accountId: string): boolean {
    const index = this.accounts.findIndex(a => a.id === accountId);
    if (index !== -1) {
      this.accounts.splice(index, 1);
      return true;
    }
    return false;
  }

  // Update an account
  updateAccount(accountId: string, updates: Partial<Omit<Account, 'id' | 'transactions'>>): Account | null {
    const account = this.accounts.find(a => a.id === accountId);
    if (account) {
      Object.assign(account, updates);
      return account;
    }
    return null;
  }

  // Generate realistic transaction data for a new account
  generateTransactionsForNewAccount(accountType: AccountType, months: number = 12): Transaction[] {
    const patterns: TransactionPattern[] = [];
    
    switch (accountType) {
      case 'checking':
        patterns.push(
          // Income
          { category: TransactionCategory.SALARY, descriptions: ['Monthly Salary', 'Payroll Deposit'], amountRange: [3800, 4200], frequency: 'monthly', probability: 1 },
          { category: TransactionCategory.FREELANCE, descriptions: ['Freelance Payment', 'Consulting Fee'], amountRange: [500, 1500], frequency: 'random', probability: 0.3 },
          
          // Regular Expenses
          { category: TransactionCategory.RENT, descriptions: ['Monthly Rent'], amountRange: [-1200, -1400], frequency: 'monthly', probability: 1 },
          { category: TransactionCategory.UTILITIES, descriptions: ['Electric Bill', 'Gas Bill', 'Water Bill', 'Internet Bill'], amountRange: [-80, -200], frequency: 'monthly', probability: 0.8 },
          { category: TransactionCategory.GROCERIES, descriptions: ['Whole Foods', 'Trader Joes', 'Safeway', 'Local Market'], amountRange: [-50, -150], frequency: 'weekly', probability: 0.9 },
          { category: TransactionCategory.GAS, descriptions: ['Shell Gas Station', 'Chevron', 'BP Gas'], amountRange: [-40, -80], frequency: 'weekly', probability: 0.7 },
          { category: TransactionCategory.RESTAURANTS, descriptions: ['Starbucks', 'Chipotle', 'Local Restaurant', 'Pizza Place'], amountRange: [-15, -60], frequency: 'random', probability: 0.4 },
          
          // Lifestyle
          { category: TransactionCategory.SUBSCRIPTIONS, descriptions: ['Netflix', 'Spotify', 'Amazon Prime', 'Gym Membership'], amountRange: [-10, -50], frequency: 'monthly', probability: 0.6 },
          { category: TransactionCategory.SHOPPING, descriptions: ['Amazon Purchase', 'Target', 'Best Buy', 'Clothing Store'], amountRange: [-25, -200], frequency: 'random', probability: 0.3 },
          { category: TransactionCategory.ENTERTAINMENT, descriptions: ['Movie Theater', 'Concert Ticket', 'Uber Ride'], amountRange: [-20, -100], frequency: 'random', probability: 0.2 },
          
          // Special Events
          { category: TransactionCategory.HOLIDAY, descriptions: ['Christmas Gifts', 'Holiday Decorations', 'Thanksgiving Dinner', 'Holiday Travel'], amountRange: [-200, -1200], frequency: 'random', probability: 0.12 },
          { category: TransactionCategory.BIRTHDAY, descriptions: ['Birthday Gift', 'Birthday Party Supplies', 'Birthday Dinner'], amountRange: [-50, -300], frequency: 'random', probability: 0.15 }
        );
        break;
        
      case 'savings':
        patterns.push(
          { category: TransactionCategory.TRANSFER, descriptions: ['Transfer from Checking', 'Monthly Savings'], amountRange: [300, 800], frequency: 'monthly', probability: 0.8 },
          { category: TransactionCategory.INVESTMENT_INCOME, descriptions: ['Interest Payment'], amountRange: [15, 45], frequency: 'monthly', probability: 1 },
          { category: TransactionCategory.TRANSFER, descriptions: ['Emergency Withdrawal', 'Transfer to Checking'], amountRange: [-200, -500], frequency: 'random', probability: 0.1 }
        );
        break;
        
      case 'credit':
        patterns.push(
          // Credit card expenses
          { category: TransactionCategory.GROCERIES, descriptions: ['Whole Foods', 'Trader Joes', 'Safeway'], amountRange: [-100, -300], frequency: 'weekly', probability: 0.8 },
          { category: TransactionCategory.GAS, descriptions: ['Shell Gas Station', 'Chevron', 'BP Gas'], amountRange: [-40, -80], frequency: 'weekly', probability: 0.7 },
          { category: TransactionCategory.RESTAURANTS, descriptions: ['Restaurant Charge', 'Food Delivery', 'Coffee Shop'], amountRange: [-25, -120], frequency: 'random', probability: 0.5 },
          { category: TransactionCategory.SHOPPING, descriptions: ['Amazon Purchase', 'Online Shopping', 'Department Store'], amountRange: [-50, -400], frequency: 'random', probability: 0.4 },
          { category: TransactionCategory.SUBSCRIPTIONS, descriptions: ['Netflix', 'Spotify', 'Software Subscription'], amountRange: [-10, -50], frequency: 'monthly', probability: 0.7 },
          
          // Payments
          { category: TransactionCategory.TRANSFER, descriptions: ['Credit Card Payment'], amountRange: [400, 1200], frequency: 'monthly', probability: 0.9 }
        );
        break;
        
      case 'mortgage':
        patterns.push(
          { category: TransactionCategory.RENT, descriptions: ['Mortgage Payment'], amountRange: [-2200, -2200], frequency: 'monthly', probability: 1 },
          { category: TransactionCategory.UTILITIES, descriptions: ['Property Tax', 'Home Insurance'], amountRange: [-300, -500], frequency: 'quarterly', probability: 1 }
        );
        break;
        
      case 'investment':
        patterns.push(
          { category: TransactionCategory.TRANSFER, descriptions: ['Monthly Investment', 'Portfolio Contribution'], amountRange: [500, 1000], frequency: 'monthly', probability: 0.7 },
          { category: TransactionCategory.INVESTMENT_RETURN, descriptions: ['Dividend Payment', 'Capital Gains'], amountRange: [50, 300], frequency: 'quarterly', probability: 0.8 },
          { category: TransactionCategory.INVESTMENT_RETURN, descriptions: ['Market Loss', 'Portfolio Rebalancing'], amountRange: [-100, -500], frequency: 'random', probability: 0.2 }
        );
        break;
    }
    
    return this.generateTransactionsFromPatterns(patterns, months);
  }
}
