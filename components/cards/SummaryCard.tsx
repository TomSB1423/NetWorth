import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { usePalette } from "../../hooks/usePalette";
import { FinancialSummary } from "../../services/types";

interface SummaryCardProps {
  summary: FinancialSummary;
  cardColor?: string;
  borderless?: boolean;
}

const SummaryCard: React.FC<SummaryCardProps> = ({ summary, cardColor, borderless }) => {
  const colors = usePalette();

  const formatCurrency = (value: number) => {
    return `$${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
  };

  return (
    <View style={[
      styles.container,
      {
        backgroundColor: cardColor || colors.backgroundAlt,
        borderColor: borderless ? 'transparent' : colors.border,
        shadowColor: borderless ? 'transparent' : colors.shadow,
        borderWidth: borderless ? 0 : 1,
        elevation: borderless ? 0 : 6,
        shadowOpacity: borderless ? 0 : 0.10,
      },
    ]}>
      <Text style={[styles.title, { color: colors.primary }]}>Financial Summary</Text>
      <View style={styles.row}>
        <Text style={[styles.label, { color: colors.secondaryText }]}>Total Assets:</Text>
        <Text style={[styles.value, { color: colors.success }]}>{formatCurrency(summary.totalAssets)}</Text>
      </View>
      <View style={styles.row}>
        <Text style={[styles.label, { color: colors.secondaryText }]}>Total Liabilities:</Text>
        <Text style={[styles.value, { color: colors.error }]}>{formatCurrency(summary.totalLiabilities)}</Text>
      </View>
      <View style={[styles.separator, { backgroundColor: colors.border }]} />
      <View style={styles.row}>
        <Text style={[styles.netWorthLabel, { color: colors.secondaryText }]}>Net Worth:</Text>
        <Text style={[
          styles.netWorthValue,
          { color: summary.netWorth >= 0 ? colors.primary : colors.error },
        ]}>
          {formatCurrency(summary.netWorth)}
        </Text>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 18,
    borderRadius: 16,
    borderWidth: 1,
    marginVertical: 12,
    width: '100%',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.10,
    shadowRadius: 12,
    elevation: 6,
  },
  title: {
    fontSize: 20,
    fontWeight: 'bold',
    marginBottom: 14,
    letterSpacing: 0.2,
    textAlign: 'center',
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginVertical: 7,
  },
  label: {
    fontSize: 16,
    fontWeight: '500',
  },
  value: {
    fontSize: 16,
    fontWeight: '600',
  },
  separator: {
    height: 1,
    marginVertical: 14,
    borderRadius: 1,
  },
  netWorthLabel: {
    fontSize: 18,
    fontWeight: '600',
  },
  netWorthValue: {
    fontSize: 18,
    fontWeight: 'bold',
  },
});

export default SummaryCard;
