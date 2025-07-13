import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { usePalette } from "../hooks/usePalette";

interface MetricsCardProps {
  title: string;
  value: string;
  subtitle?: string;
  trend?: {
    direction: 'up' | 'down' | 'neutral';
    percentage: number;
  };
  icon?: string;
}

export default function MetricsCard({ title, value, subtitle, trend, icon }: MetricsCardProps) {
  const colors = usePalette();

  const getTrendColor = () => {
    if (!trend) return colors.secondaryText;
    switch (trend.direction) {
      case 'up': return colors.success;
      case 'down': return colors.error;
      default: return colors.secondaryText;
    }
  };

  const getTrendIcon = () => {
    if (!trend) return '';
    switch (trend.direction) {
      case 'up': return '↗';
      case 'down': return '↘';
      default: return '→';
    }
  };

  return (
    <View style={[styles.card, { backgroundColor: colors.card }]}>
      <View style={styles.header}>
        <Text style={[styles.title, { color: colors.secondaryText }]}>{title}</Text>
        {icon && <Text style={styles.icon}>{icon}</Text>}
      </View>
      
      <Text style={[styles.value, { color: colors.text }]}>{value}</Text>
      
      {subtitle && (
        <Text style={[styles.subtitle, { color: colors.secondaryText }]}>{subtitle}</Text>
      )}
      
      {trend && (
        <View style={styles.trendContainer}>
          <Text style={[styles.trend, { color: getTrendColor() }]}>
            {getTrendIcon()} {Math.abs(trend.percentage).toFixed(1)}%
          </Text>
        </View>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    borderRadius: 12,
    padding: 16,
    flex: 1,
    minHeight: 100,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  title: {
    fontSize: 12,
    fontWeight: '600',
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  icon: {
    fontSize: 18,
  },
  value: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 4,
  },
  subtitle: {
    fontSize: 12,
    marginBottom: 8,
  },
  trendContainer: {
    alignSelf: 'flex-start',
  },
  trend: {
    fontSize: 12,
    fontWeight: '600',
  },
});
