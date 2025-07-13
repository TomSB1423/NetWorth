// netWorthMockService.ts
// Mock implementation of NetWorthService

import { AccountMockService } from "./accountMockService";
import { NetWorthEntry, NetWorthService } from "./netWorthService";

export class NetWorthMockService implements NetWorthService {
  public accountService = new AccountMockService();

  getNetWorthHistory(days: number): NetWorthEntry[] {
    // Calculate total net worth across all accounts for each day
    const now = new Date();
    const accounts = this.accountService.getAccounts();
    const data: NetWorthEntry[] = [];
    
    for (let i = days - 1; i >= 0; i--) {
      const date = new Date(now);
      date.setDate(now.getDate() - i);
      const dateString = date.toISOString().slice(0, 10);
      
      let totalNetWorth = 0;
      
      // Calculate net worth for this specific date
      for (const account of accounts) {
        // Get balance up to this date
        const accountBalance = account.transactions
          .filter(t => t.date <= dateString)
          .reduce((sum, t) => sum + t.amount, 0);
        
        // Handle different account types
        if (account.type === 'credit' || account.type === 'mortgage') {
          // For debt accounts, negative balance means we owe money
          // So we subtract the absolute value of the balance from net worth
          totalNetWorth -= Math.abs(accountBalance);
        } else {
          // For asset accounts (checking, savings, investment), add to net worth
          totalNetWorth += accountBalance;
        }
      }
      
      data.push({
        date: dateString,
        value: Math.round(totalNetWorth),
      });
    }
    
    return data;
  }
}
