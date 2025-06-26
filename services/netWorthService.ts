// netWorthService.ts
// Interface for net worth data services


export type NetWorthEntry = {
  date: string; // ISO date string
  value: number;
};

export interface NetWorthService {
  getNetWorthHistory(days: number): NetWorthEntry[];
}
