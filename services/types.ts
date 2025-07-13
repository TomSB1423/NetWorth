// This file is a type definition or utility and not a route.
// You can safely ignore this warning or delete this file if not needed.

// Define types for financial data
export type AssetCategory = 'cash' | 'investments' | 'property' | 'other';
export type LiabilityCategory = 'mortgage' | 'loans' | 'credit' | 'other';

export interface Asset {
  id: string;
  name: string;
  value: number;
  category: AssetCategory;
}

export interface Liability {
  id: string;
  name: string;
  value: number;
  category: LiabilityCategory;
}

export interface FinancialSummary {
  totalAssets: number;
  totalLiabilities: number;
  netWorth: number;
  assetsByCategory: Record<AssetCategory, number>;
  liabilitiesByCategory: Record<LiabilityCategory, number>;
}
