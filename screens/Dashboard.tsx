import React from "react";
import { Platform, ScrollView, StyleSheet, View } from "react-native";
import CategoryBreakdown from "../components/CategoryBreakdown";
import MetricsCard from "../components/MetricsCard";
import MonthlyFlowChart from "../components/MonthlyFlowChart";
import NetWorthChart from "../components/NetWorthChart";
import {
    useAccounts,
    useAppDispatch,
    useAppLoading,
    useMetrics,
    useNetWorthHistory,
    usePalette,
    useSelectedTimeRange
} from "../hooks";
import { setSelectedTimeRange } from "../store/slices/uiSlice";

const TIME_RANGE_DAYS: Record<string, number> = {
  all: 365,
  "1y": 365,
  "6m": 182,
  "3m": 90,
  "1w": 7,
};

export default function Dashboard() {
  // ALL HOOKS MUST BE CALLED FIRST - before any conditional logic
  const colors = usePalette();
  const dispatch = useAppDispatch();
  
  // Use Redux state
  const accounts = useAccounts();
  const metrics = useMetrics();
  const netWorthHistory = useNetWorthHistory();
  const isLoading = useAppLoading();
  const selectedRange = useSelectedTimeRange();

  // Filter net worth history based on selected range
  const filteredHistory = React.useMemo(() => {
    const days = TIME_RANGE_DAYS[selectedRange];
    return netWorthHistory.slice(-days);
  }, [netWorthHistory, selectedRange]);

  const handleRangeChange = (range: string) => {
    dispatch(setSelectedTimeRange(range));
  };

  // Return empty view if no metrics available
  if (!metrics) {
    return <View style={[styles.container, { backgroundColor: colors.background }]} />;
  }

  return (
    <ScrollView style={[styles.container, { backgroundColor: colors.background }]}> 
        <NetWorthChart
          data={filteredHistory}
          selectedRange={selectedRange}
          onRangeChange={handleRangeChange}
        />
        
        {/* Key Metrics Row */}
        <View style={styles.metricsRow}>
          <MetricsCard
            title="Monthly Income"
            value={`$${metrics.monthlyIncome.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            subtitle="Last 30 days"
            icon="💰"
          />
          <MetricsCard
            title="Monthly Expenses"
            value={`$${metrics.monthlyExpenses.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            subtitle="Last 30 days"
            icon="💸"
          />
        </View>

        <View style={styles.metricsRow}>
          <MetricsCard
            title="Savings Rate"
            value={`${metrics.savingsRate.toFixed(1)}%`}
            subtitle="Income - Expenses"
            trend={{
              direction: metrics.savingsRate > 20 ? 'up' : metrics.savingsRate > 10 ? 'neutral' : 'down',
              percentage: Math.abs(metrics.savingsRate - 20)
            }}
            icon="📈"
          />
          <MetricsCard
            title="Avg Transaction"
            value={`$${metrics.averageTransactionSize.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            subtitle="All time"
            icon="🧾"
          />
        </View>

        <View style={styles.metricsRow}>
          <MetricsCard
            title="Total Balance"
            value={`$${metrics.totalBalance.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            subtitle="Assets - Liabilities"
            trend={{
              direction: metrics.totalBalance > 0 ? 'up' : 'down',
              percentage: Math.abs(metrics.totalBalance / 1000)
            }}
            icon="💳"
          />
          <MetricsCard
            title="Largest Expense"
            value={`$${metrics.largestExpense.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`}
            subtitle="Last 30 days"
            icon="💲"
          />
        </View>

        {/* Monthly Flow Chart */}
        <MonthlyFlowChart data={metrics.monthlyFlow} />

        {/* Category Breakdowns */}
        <View style={styles.categoryRow}>
          <View style={styles.categoryContainer}>
            <CategoryBreakdown
              title="Top Expenses"
              data={metrics.expenseCategories}
              type="expense"
            />
          </View>
          <View style={styles.categoryContainer}>
            <CategoryBreakdown
              title="Income Sources"
              data={metrics.incomeCategories}
              type="income"
            />
          </View>
        </View>
      </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: Platform.OS === 'android' ? 60 : Platform.OS === 'ios' ? 80 : 20,
    paddingHorizontal: 20,
    backgroundColor: 'transparent',
  },
  metricsRow: {
    flexDirection: 'row',
    gap: 12,
    marginBottom: 16,
  },
  categoryRow: {
    flexDirection: 'row',
    gap: 12,
    marginBottom: 16,
  },
  categoryContainer: {
    flex: 1,
  },
});
