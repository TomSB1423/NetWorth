import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { usePalette } from "../../hooks/usePalette";

interface CategoryData {
  category: string;
  amount: number;
  percentage: number;
}

interface CategoryBreakdownProps {
  title: string;
  data: CategoryData[];
  type: 'income' | 'expense';
}

export default function CategoryBreakdown({ title, data, type }: CategoryBreakdownProps) {
  const colors = usePalette();

  if (!data || data.length === 0) {
    return (
      <View style={[styles.container, { backgroundColor: colors.card }]}>
        <Text style={[styles.title, { color: colors.text }]}>{title}</Text>
        <Text style={[styles.emptyText, { color: colors.secondaryText }]}>
          No data available
        </Text>
      </View>
    );
  }

  const getCategoryColor = (index: number) => {
    const colorOptions = [
      colors.primary,
      colors.success,
      colors.warning,
      colors.error,
      colors.info,
      colors.secondaryText,
    ];
    return colorOptions[index % colorOptions.length];
  };

  // Sort by amount and take top 5 - create a copy first to avoid mutating Redux state
  const topCategories = [...data]
    .sort((a, b) => Math.abs(b.amount) - Math.abs(a.amount))
    .slice(0, 5);

  return (
    <View style={[styles.container, { backgroundColor: colors.card }]}>
      <Text style={[styles.title, { color: colors.text }]}>{title}</Text>
      
      <View style={styles.categoriesContainer}>
        {topCategories.map((category, index) => (
          <View key={category.category} style={styles.categoryRow}>
            <View style={styles.categoryInfo}>
              <View 
                style={[
                  styles.categoryDot, 
                  { backgroundColor: getCategoryColor(index) }
                ]} 
              />
              <Text style={[styles.categoryName, { color: colors.text }]}>
                {category.category}
              </Text>
            </View>
            
            <View style={styles.categoryValues}>
              <Text style={[styles.categoryAmount, { color: colors.text }]}>
                ${Math.abs(category.amount).toLocaleString('en-US', { 
                  minimumFractionDigits: 0, 
                  maximumFractionDigits: 0 
                })}
              </Text>
              <Text style={[styles.categoryPercentage, { color: colors.secondaryText }]}>
                {category.percentage.toFixed(1)}%
              </Text>
            </View>
          </View>
        ))}
      </View>
      
      {/* Progress bars */}
      <View style={styles.progressContainer}>
        {topCategories.map((category, index) => (
          <View key={`${category.category}-bar`} style={styles.progressRow}>
            <View 
              style={[
                styles.progressBar,
                { backgroundColor: colors.border }
              ]}
            >
              <View
                style={[
                  styles.progressFill,
                  {
                    width: `${category.percentage}%`,
                    backgroundColor: getCategoryColor(index),
                  }
                ]}
              />
            </View>
          </View>
        ))}
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
  categoriesContainer: {
    marginBottom: 12,
  },
  categoryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  categoryInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },
  categoryDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    marginRight: 8,
  },
  categoryName: {
    fontSize: 14,
    flex: 1,
  },
  categoryValues: {
    alignItems: 'flex-end',
  },
  categoryAmount: {
    fontSize: 14,
    fontWeight: '600',
  },
  categoryPercentage: {
    fontSize: 12,
  },
  progressContainer: {
    gap: 8,
  },
  progressRow: {
    height: 4,
  },
  progressBar: {
    height: 4,
    borderRadius: 2,
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    borderRadius: 2,
  },
});
