// serviceProvider.ts
// Singleton pattern to ensure consistent data across screens

import { NetWorthMockService } from './netWorthMockService';

let netWorthServiceInstance: NetWorthMockService | null = null;

export function getNetWorthService(): NetWorthMockService {
  if (!netWorthServiceInstance) {
    netWorthServiceInstance = new NetWorthMockService();
  }
  return netWorthServiceInstance;
}

// For testing or reset purposes
export function resetNetWorthService(): void {
  netWorthServiceInstance = null;
}
