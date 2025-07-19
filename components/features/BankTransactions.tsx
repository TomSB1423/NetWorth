import React from "react";
import { ScrollView, StyleSheet, Text, View } from "react-native";
import { usePalette } from "../../hooks/usePalette";
import { Transaction } from "../../services/accountMockService";

interface BankTransactionsProps {
  transactions: Transaction[];
  colorScheme: "light" | "dark";
}

function formatMonth(dateStr: string) {
  const d = new Date(dateStr);
  return d.toLocaleString("default", { month: "short", year: "2-digit" });
}

function getMonthlySections(transactions: Transaction[]) {
  const sections: { month: string; items: Transaction[]; total: number }[] = [];
  let currentMonth = "";
  let currentItems: Transaction[] = [];
  for (const tx of transactions) {
    const month = tx.date.slice(0, 7);
    if (month !== currentMonth) {
      if (currentItems.length > 0) {
        const total = currentItems.reduce((sum, t) => sum + t.amount, 0);
        sections.push({ month: currentMonth, items: currentItems, total });
      }
      currentMonth = month;
      currentItems = [];
    }
    currentItems.push(tx);
  }
  if (currentItems.length > 0) {
    const total = currentItems.reduce((sum, t) => sum + t.amount, 0);
    sections.push({ month: currentMonth, items: currentItems, total });
  }
  return sections;
}

export default function BankTransactions({ transactions }: BankTransactionsProps) {
  const colors = usePalette();
  const sections = getMonthlySections(transactions);
  const total = transactions.reduce((sum, t) => sum + t.amount, 0);
  const totalColor = colors.warning;
  return (
    <ScrollView 
      style={styles.container}
      showsVerticalScrollIndicator={false}
      contentContainerStyle={styles.scrollContent}
    >
      <View style={styles.headerRow}>
        <Text style={[styles.header, { color: colors.text }]}>Bank Transactions</Text>
        <Text style={[styles.total, { color: totalColor }]}>
          ${total.toLocaleString(undefined, { minimumFractionDigits: 2 })}
        </Text>
      </View>
      {sections.map(section => (
        <View key={section.month}>
          <View style={styles.monthHeaderRow}>
            <Text style={[styles.monthHeader, { color: colors.secondaryText }]}>{formatMonth(section.items[0].date)}</Text>
            <Text style={[styles.monthTotal, { color: colors.text }]}>
              ${section.total.toLocaleString(undefined, { minimumFractionDigits: 2 })}
            </Text>
          </View>
          {section.items.map(tx => (
            <View style={styles.row} key={tx.id}>
              <View style={{ flex: 1 }}>
                <Text style={[styles.desc, { color: colors.text }]}>{tx.description}</Text>
                <Text style={[styles.date, { color: colors.secondaryText }]}>{tx.date}</Text>
              </View>
              <Text style={{ color: tx.amount > 0 ? colors.success : colors.error, fontWeight: "bold" }}>
                {tx.amount >= 0 ? "+" : "-"}${Math.abs(tx.amount).toFixed(2)}
              </Text>
            </View>
          ))}
          <View style={[styles.separator, { borderBottomColor: colors.border }]} />
        </View>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    marginTop: 24,
  },
  scrollContent: {
    paddingBottom: 32,
  },
  headerRow: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginBottom: 12,
  },
  header: {
    fontSize: 18,
    fontWeight: "bold",
  },
  total: {
    fontSize: 18,
    fontWeight: "bold",
  },
  monthHeaderRow: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
    marginTop: 16,
    marginBottom: 4,
  },
  monthHeader: {
    fontSize: 15,
    fontWeight: "600",
  },
  monthTotal: {
    fontSize: 15,
    fontWeight: "600",
  },
  row: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: 8,
  },
  desc: {
    fontSize: 15,
  },
  date: {
    fontSize: 12,
  },
  separator: {
    borderBottomWidth: StyleSheet.hairlineWidth,
    marginVertical: 2,
  },
});
