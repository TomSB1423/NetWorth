import { Account, Transaction } from './accountMockService';

// Transaction categories enum
export enum TransactionCategory {
  // Income categories
  SALARY = 'Salary',
  FREELANCE = 'Freelance',
  BONUS = 'Bonus',
  INVESTMENT_INCOME = 'Investment Income',
  GIFT_RECEIVED = 'Gift Received',
  
  // Expense categories
  FOOD_DINING = 'Food & Dining',
  TRANSPORTATION = 'Transportation',
  HOUSING = 'Housing',
  SHOPPING = 'Shopping',
  ENTERTAINMENT = 'Entertainment',
  HEALTHCARE = 'Healthcare',
  
  // Additional categories from accountMockService
  GROCERIES = 'Groceries',
  RESTAURANTS = 'Restaurants',
  GAS = 'Gas',
  UTILITIES = 'Utilities',
  RENT = 'Rent',
  MORTGAGE = 'Mortgage',
  INSURANCE = 'Insurance',
  SUBSCRIPTIONS = 'Subscriptions',
  GYM = 'Gym',
  EDUCATION = 'Education',
  GIFTS_GIVEN = 'Gifts Given',
  CHARITY = 'Charity',
  TRAVEL = 'Travel',
  
  // Special events and bulk purchases
  WEDDING = 'Wedding',
  FUNERAL = 'Funeral',
  BIRTHDAY = 'Birthday',
  ANNIVERSARY = 'Anniversary',
  HOLIDAY = 'Holiday',
  GRADUATION = 'Graduation',
  
  // System categories
  TRANSFER = 'Transfer',
  FEES = 'Fees',
  INVESTMENT_RETURN = 'Investment Return',
  OTHER = 'Other',
}

interface MonthlyFlowData {
  month: string;
  income: number;
  expenses: number;
  net: number;
}

interface CategoryData {
  category: string;
  amount: number;
  percentage: number;
}

export interface MetricsData {
  totalBalance: number;
  monthlyIncome: number;
  monthlyExpenses: number;
  savingsRate: number;
  averageTransactionSize: number;
  largestExpense: number;
  monthlyFlow: MonthlyFlowData[];
  expenseCategories: CategoryData[];
  incomeCategories: CategoryData[];
}

export class MetricsService {
  private accounts: Account[];

  constructor(accounts: Account[]) {
    this.accounts = accounts;
  }

  private getAllTransactions(): Transaction[] {
    return this.accounts.flatMap(account => account.transactions);
  }

  private getTransactionsInDateRange(days: number): Transaction[] {
    const cutoffDate = new Date();
    cutoffDate.setDate(cutoffDate.getDate() - days);
    
    return this.getAllTransactions().filter(
      transaction => new Date(transaction.date) >= cutoffDate
    );
  }

  private categorizeTransaction(transaction: Transaction): string {
    const { description, amount, category } = transaction;
    
    // If the transaction already has a category from accountMockService, use it
    if (category) {
      return category;
    }
    
    // Fallback categorization for legacy data
    const desc = description.toLowerCase();

    // Income categories
    if (amount > 0) {
      if (desc.includes('salary') || desc.includes('payroll') || desc.includes('wage')) {
        return TransactionCategory.SALARY;
      }
      if (desc.includes('dividend') || desc.includes('interest')) {
        return TransactionCategory.INVESTMENT_INCOME;
      }
      if (desc.includes('freelance') || desc.includes('consulting')) {
        return TransactionCategory.FREELANCE;
      }
      return TransactionCategory.OTHER;
    }

    // Expense categories
    if (desc.includes('grocery') || desc.includes('food')) {
      return TransactionCategory.GROCERIES;
    }
    if (desc.includes('restaurant') || desc.includes('dining')) {
      return TransactionCategory.RESTAURANTS;
    }
    if (desc.includes('gas') || desc.includes('fuel')) {
      return TransactionCategory.GAS;
    }
    if (desc.includes('transport') || desc.includes('uber')) {
      return TransactionCategory.TRANSPORTATION;
    }
    if (desc.includes('rent')) {
      return TransactionCategory.RENT;
    }
    if (desc.includes('mortgage')) {
      return TransactionCategory.MORTGAGE;
    }
    if (desc.includes('utilities') || desc.includes('electric') || desc.includes('water') || desc.includes('internet')) {
      return TransactionCategory.UTILITIES;
    }
    if (desc.includes('shopping') || desc.includes('amazon') || desc.includes('store')) {
      return TransactionCategory.SHOPPING;
    }
    if (desc.includes('entertainment') || desc.includes('movie') || desc.includes('netflix')) {
      return TransactionCategory.ENTERTAINMENT;
    }
    if (desc.includes('health') || desc.includes('medical') || desc.includes('pharmacy')) {
      return TransactionCategory.HEALTHCARE;
    }
    if (desc.includes('subscription') || desc.includes('spotify') || desc.includes('gym')) {
      return TransactionCategory.SUBSCRIPTIONS;
    }
    if (desc.includes('travel') || desc.includes('hotel') || desc.includes('flight')) {
      return TransactionCategory.TRAVEL;
    }
    if (desc.includes('wedding')) {
      return TransactionCategory.WEDDING;
    }
    if (desc.includes('funeral') || desc.includes('memorial')) {
      return TransactionCategory.FUNERAL;
    }
    if (desc.includes('birthday') || desc.includes('party')) {
      return TransactionCategory.BIRTHDAY;
    }
    if (desc.includes('anniversary')) {
      return TransactionCategory.ANNIVERSARY;
    }
    if (desc.includes('holiday') || desc.includes('christmas') || desc.includes('thanksgiving')) {
      return TransactionCategory.HOLIDAY;
    }
    if (desc.includes('graduation') || desc.includes('ceremony')) {
      return TransactionCategory.GRADUATION;
    }
    if (desc.includes('transfer') || desc.includes('payment')) {
      return TransactionCategory.TRANSFER;
    }
    if (desc.includes('fee') || desc.includes('charge')) {
      return TransactionCategory.FEES;
    }
    return TransactionCategory.OTHER;
  }

  getMetrics(): MetricsData {
    const allTransactions = this.getAllTransactions();
    const last30Days = this.getTransactionsInDateRange(30);
    const last90Days = this.getTransactionsInDateRange(90);

    // Calculate total balance (assets - liabilities)
    const totalBalance = this.accounts.reduce((sum, account) => {
      const balance = account.transactions.reduce((acc, t) => acc + t.amount, 0);
      // For liabilities (mortgage, credit), treat as negative
      if (account.type === 'mortgage' || account.type === 'credit') {
        return sum - Math.abs(balance);
      }
      return sum + balance;
    }, 0);

    // Monthly income and expenses (last 30 days)
    const monthlyIncome = last30Days
      .filter(t => t.amount > 0)
      .reduce((sum, t) => sum + t.amount, 0);

    const monthlyExpenses = Math.abs(last30Days
      .filter(t => t.amount < 0)
      .reduce((sum, t) => sum + t.amount, 0));

    // Savings rate
    const savingsRate = monthlyIncome > 0 ? ((monthlyIncome - monthlyExpenses) / monthlyIncome) * 100 : 0;

    // Average transaction size
    const averageTransactionSize = allTransactions.length > 0 
      ? Math.abs(allTransactions.reduce((sum, t) => sum + Math.abs(t.amount), 0) / allTransactions.length)
      : 0;

    // Largest expense in the last 30 days
    const largestExpense = Math.max(
      0,
      ...last30Days.filter(t => t.amount < 0).map(t => Math.abs(t.amount))
    );

    // Monthly flow data (last 6 months)
    const monthlyFlow = this.getMonthlyFlowData();

    // Category breakdowns
    const expenseCategories = this.getCategoryBreakdown(last90Days, 'expense');
    const incomeCategories = this.getCategoryBreakdown(last90Days, 'income');

    return {
      totalBalance,
      monthlyIncome,
      monthlyExpenses,
      savingsRate,
      averageTransactionSize,
      largestExpense,
      monthlyFlow,
      expenseCategories,
      incomeCategories,
    };
  }

  private getMonthlyFlowData(): MonthlyFlowData[] {
    const monthlyData: { [key: string]: MonthlyFlowData } = {};
    const allTransactions = this.getAllTransactions();

    // Group transactions by month
    allTransactions.forEach(transaction => {
      const date = new Date(transaction.date);
      const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
      const monthName = date.toLocaleDateString('en-US', { month: 'short' });

      if (!monthlyData[monthKey]) {
        monthlyData[monthKey] = {
          month: monthName,
          income: 0,
          expenses: 0,
          net: 0,
        };
      }

      if (transaction.amount > 0) {
        monthlyData[monthKey].income += transaction.amount;
      } else {
        monthlyData[monthKey].expenses += transaction.amount;
      }
    });

    // Calculate net and sort by date
    const sortedData = Object.values(monthlyData)
      .map(data => ({
        ...data,
        net: data.income + data.expenses, // expenses are negative
      }))
      .sort((a, b) => {
        // Sort by month name (this is simplified, in real app you'd sort by actual date)
        const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        return months.indexOf(a.month) - months.indexOf(b.month);
      });

    return sortedData.slice(-6); // Last 6 months
  }

  private getCategoryBreakdown(transactions: Transaction[], type: 'income' | 'expense'): CategoryData[] {
    const filteredTransactions = transactions.filter(t => 
      type === 'income' ? t.amount > 0 : t.amount < 0
    );

    const categoryTotals: { [key: string]: number } = {};
    let totalAmount = 0;

    filteredTransactions.forEach(transaction => {
      const category = this.categorizeTransaction(transaction);
      const amount = Math.abs(transaction.amount);
      categoryTotals[category] = (categoryTotals[category] || 0) + amount;
      totalAmount += amount;
    });

    return Object.entries(categoryTotals)
      .map(([category, amount]) => ({
        category,
        amount,
        percentage: totalAmount > 0 ? (amount / totalAmount) * 100 : 0,
      }))
      .sort((a, b) => b.amount - a.amount);
  }
}
