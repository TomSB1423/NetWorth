import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { usePalette } from "../../hooks/usePalette";

interface MonthlyFlowData {
  month: string;
  income: number;
  expenses: number;
}

interface MonthlyFlowChartProps {
  data: MonthlyFlowData[];
}

export default function MonthlyFlowChart({ data }: MonthlyFlowChartProps) {
  const colors = usePalette();

  if (!data || data.length === 0) {
    return (
      <View style={[styles.container, { backgroundColor: colors.card }]}>
        <Text style={[styles.title, { color: colors.text }]}>Monthly Cash Flow</Text>
        <Text style={[styles.emptyText, { color: colors.secondaryText }]}>
          No data available
        </Text>
      </View>
    );
  }

  // Find the maximum value to scale the bars
  const maxValue = Math.max(
    ...data.flatMap(d => [d.income, Math.abs(d.expenses)])
  );

  return (
    <View style={[styles.container, { backgroundColor: colors.card }]}>
      <Text style={[styles.title, { color: colors.text }]}>Monthly Cash Flow</Text>
      
      <View style={styles.chartContainer}>
        {data.slice(-6).map((item, index) => {
          const incomeHeight = (item.income / maxValue) * 100;
          const expenseHeight = (Math.abs(item.expenses) / maxValue) * 100;
          
          return (
            <View key={index} style={styles.barGroup}>
              <View style={styles.barsContainer}>
                {/* Income bar */}
                <View style={styles.barWrapper}>
                  <View
                    style={[
                      styles.bar,
                      styles.incomeBar,
                      { 
                        height: `${incomeHeight}%`,
                        backgroundColor: colors.success,
                      }
                    ]}
                  />
                </View>
                
                {/* Expense bar */}
                <View style={styles.barWrapper}>
                  <View
                    style={[
                      styles.bar,
                      styles.expenseBar,
                      { 
                        height: `${expenseHeight}%`,
                        backgroundColor: colors.error,
                      }
                    ]}
                  />
                </View>
              </View>
              
              <Text style={[styles.monthLabel, { color: colors.secondaryText }]}>
                {item.month}
              </Text>
            </View>
          );
        })}
      </View>
      
      <View style={styles.legend}>
        <View style={styles.legendItem}>
          <View style={[styles.legendColor, { backgroundColor: colors.success }]} />
          <Text style={[styles.legendText, { color: colors.secondaryText }]}>Income</Text>
        </View>
        <View style={styles.legendItem}>
          <View style={[styles.legendColor, { backgroundColor: colors.error }]} />
          <Text style={[styles.legendText, { color: colors.secondaryText }]}>Expenses</Text>
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
  },
  title: {
    fontSize: 16,
    fontWeight: 'bold',
    marginBottom: 16,
  },
  emptyText: {
    fontSize: 14,
    textAlign: 'center',
    marginTop: 20,
  },
  chartContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-end',
    height: 120,
    marginBottom: 16,
  },
  barGroup: {
    flex: 1,
    alignItems: 'center',
    marginHorizontal: 2,
  },
  barsContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'flex-end',
    height: 100,
    width: '100%',
  },
  barWrapper: {
    flex: 1,
    alignItems: 'center',
    height: '100%',
    justifyContent: 'flex-end',
    marginHorizontal: 1,
  },
  bar: {
    width: 12,
    borderRadius: 2,
    minHeight: 2,
  },
  incomeBar: {
    marginRight: 1,
  },
  expenseBar: {
    marginLeft: 1,
  },
  monthLabel: {
    fontSize: 10,
    marginTop: 8,
    textAlign: 'center',
  },
  legend: {
    flexDirection: 'row',
    justifyContent: 'center',
    gap: 16,
  },
  legendItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
  },
  legendColor: {
    width: 12,
    height: 12,
    borderRadius: 2,
  },
  legendText: {
    fontSize: 12,
  },
});
