import React, { useState } from "react";
import { Dimensions, Pressable, StyleSheet, Text, View } from "react-native";
import Svg, { Circle, Line, Path, Text as SvgText } from "react-native-svg";
import { usePalette } from "../hooks/usePalette";
import { NetWorthEntry } from "../services/netWorthService";

const screenWidth = Dimensions.get("window").width;
const chartHeight = 140;
const chartWidth = screenWidth - 32;
const padding = 32;

interface NetWorthChartProps {
  data: NetWorthEntry[];
  setScrollEnabled?: (enabled: boolean) => void;
  selectedRange?: string;
  onRangeChange?: (range: string) => void;
}

const TIME_RANGES = [
  { label: "ALL", value: "all" },
  { label: "1y", value: "1y" },
  { label: "6m", value: "6m" },
  { label: "3m", value: "3m" },
  { label: "1w", value: "1w" },
];

export default function NetWorthChart({ data, setScrollEnabled, selectedRange, onRangeChange }: NetWorthChartProps) {
  const colors = usePalette();
  const [hoverIndex, setHoverIndex] = useState<number | null>(null);
  const [internalRange, setInternalRange] = useState<string>(selectedRange ?? "all");

  // Compute the displayed value (current net worth or hovered value)
  const displayValue = hoverIndex !== null && data[hoverIndex]
    ? data[hoverIndex].value
    : data.length > 0
      ? data[data.length - 1].value
      : 0;

  // Prepare data points for chart (filtering is already done by parent)
  const values = data.map(d => d.value);
  
  if (values.length === 0) {
    // Handle empty data case
    return (
      <View style={styles.container}>
        <Text style={[styles.total, { color: colors.primary }]}>
          $0.00
        </Text>
        <Text style={[{ textAlign: 'center', fontSize: 14, marginTop: 8 }, { color: colors.secondaryText }]}>
          No data available
        </Text>
      </View>
    );
  }

  // Calculate min/max values from the filtered data for proper Y-axis scaling
  const minValue = Math.min(...values);
  const maxValue = Math.max(...values);
  const valueRange = maxValue - minValue || 1;
  
  // Add padding to the Y-axis range for better visualization
  const paddingPercent = 0.1; // 10% padding
  const rangePadding = valueRange * paddingPercent;
  const adjustedMinValue = minValue - rangePadding;
  const adjustedMaxValue = maxValue + rangePadding;
  const adjustedRange = adjustedMaxValue - adjustedMinValue;
  
  // Calculate chart points using adjusted range
  const points = values.map((v, i) => {
    const x = (i / (values.length - 1)) * (chartWidth - padding) + padding / 2;
    const y = chartHeight - ((v - adjustedMinValue) / adjustedRange) * (chartHeight - padding) - padding / 2;
    return { x, y };
  });

  // Find indices for min and max values for axis marks
  const minValueIndex = values.indexOf(minValue);
  const maxValueIndex = values.indexOf(maxValue);
  const minPoint = points[minValueIndex];
  const maxPoint = points[maxValueIndex];

  // Create simple path from points
  const pathPoints = points.map((p, i) => 
    i === 0 ? `M${p.x},${p.y}` : `L${p.x},${p.y}`
  ).join(" ");

  // Chart interaction handlers
  const handleChartPress = (event: any) => {
    const x = event.nativeEvent.locationX;
    
    if (points.length < 2) return;
    
    // Find closest point to touch/click position
    let closestIndex = 0;
    let minDistance = Math.abs(points[0].x - x);
    
    for (let i = 1; i < points.length; i++) {
      const distance = Math.abs(points[i].x - x);
      if (distance < minDistance) {
        minDistance = distance;
        closestIndex = i;
      }
    }
    
    setHoverIndex(closestIndex);
  };

  const handlePressEnd = () => {
    setTimeout(() => setHoverIndex(null), 2000); // Auto-hide indicator after 2 seconds
    setScrollEnabled?.(true);
  };

  const formatCurrency = (value: number) =>
    `$${value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

  const range = selectedRange ?? internalRange;
  const handleRangeChange = (rangeValue: string) => {
    if (onRangeChange) {
      onRangeChange(rangeValue);
    } else {
      setInternalRange(rangeValue);
    }
  };

  return (
    <View style={styles.container}>
      {/* Centered total amount above the chart */}
      <Text style={[styles.total, { color: colors.primary }]} accessibilityRole="header">
        {formatCurrency(displayValue)}
      </Text>
      
      {/* Chart area */}
      <Pressable 
        style={styles.chartArea}
        onPressIn={() => setScrollEnabled?.(false)}
        onTouchMove={handleChartPress}
        onPressOut={handlePressEnd}
      >
        <Svg width={chartWidth} height={chartHeight}>
          {/* Chart line */}
          {points.length > 1 && (
            <Path
              d={pathPoints}
              fill="none"
              stroke={colors.primary}
              strokeWidth={2.5}
              strokeLinejoin="round"
            />
          )}
          
          {/* Y-axis marks for highest and lowest points */}
          {points.length > 1 && minValue !== maxValue && (
            <>
              {/* Max value point and label */}
              <Circle
                cx={maxPoint.x}
                cy={maxPoint.y}
                r="4"
                fill={colors.success}
                stroke={colors.background}
                strokeWidth="2"
              />
              <SvgText
                x={maxPoint.x}
                y={maxPoint.y - 12}
                fontSize="11"
                fill={colors.success}
                textAnchor="middle"
                fontWeight="600"
              >
                {formatCurrency(maxValue)}
              </SvgText>
              
              {/* Min value point and label */}
              <Circle
                cx={minPoint.x}
                cy={minPoint.y}
                r="4"
                fill={colors.error}
                stroke={colors.background}
                strokeWidth="2"
              />
              <SvgText
                x={minPoint.x}
                y={minPoint.y + 18}
                fontSize="11"
                fill={colors.error}
                textAnchor="middle"
                fontWeight="600"
              >
                {formatCurrency(minValue)}
              </SvgText>
            </>
          )}
          
          {/* Vertical indicator line */}
          {hoverIndex !== null && points[hoverIndex] && (
            <Line
              x1={points[hoverIndex].x}
              y1={0}
              x2={points[hoverIndex].x}
              y2={chartHeight}
              stroke={colors.primary}
              strokeWidth={1.5}
              opacity={0.7}
            />
          )}
        </Svg>
      </Pressable>
      
      {/* Time range selector */}
      <View style={styles.rangeSelector}>
        {TIME_RANGES.map(rangeObj => (
          <Text
            key={rangeObj.value}
            accessibilityRole="button"
            onPress={() => handleRangeChange(rangeObj.value)}
            style={[
              styles.rangeButton,
              {
                color: range === rangeObj.value ? colors.primary : colors.secondaryText,
                fontWeight: range === rangeObj.value ? "bold" : "normal",
                backgroundColor: range === rangeObj.value ? colors.card : 'transparent',
                borderColor: range === rangeObj.value ? colors.primary : 'transparent',
              },
            ]}
          >
            {rangeObj.label}
          </Text>
        ))}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    position: 'relative',
    width: '100%',
    alignItems: 'center',
    paddingTop: 0,
  },
  chartArea: {
    width: '100%',
    alignItems: 'center',
  },
  total: {
    fontSize: 28,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 4,
    marginTop: 8,
    letterSpacing: 0.5,
  },
  rangeSelector: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 12,
    marginBottom: 4,
    gap: 8,
  },
  rangeButton: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
    borderWidth: 1,
    overflow: 'hidden',
    fontSize: 14,
    marginHorizontal: 2,
  },
});
