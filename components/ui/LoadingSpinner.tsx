// components/LoadingSpinner.tsx
// Reusable loading component with accessibility

import React from 'react';
import { ActivityIndicator, StyleSheet, Text, View } from 'react-native';
import { usePalette } from '../../hooks/usePalette';

interface LoadingSpinnerProps {
  message?: string;
  size?: 'small' | 'large';
  overlay?: boolean;
}

export default function LoadingSpinner({ 
  message = 'Loading...', 
  size = 'large',
  overlay = false 
}: LoadingSpinnerProps) {
  const colors = usePalette();

  const containerStyle = overlay 
    ? [styles.overlayContainer, { backgroundColor: colors.background + 'CC' }]
    : [styles.container, { backgroundColor: colors.background }];

  return (
    <View style={containerStyle}>
      <View style={[styles.content, { backgroundColor: colors.card }]}>
        <ActivityIndicator 
          size={size} 
          color={colors.primary}
          accessibilityLabel="Loading"
          accessibilityHint="Please wait while content loads"
        />
        {message && (
          <Text 
            style={[styles.message, { color: colors.text }]}
            accessibilityLabel={message}
          >
            {message}
          </Text>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
  },
  overlayContainer: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000,
  },
  content: {
    padding: 24,
    borderRadius: 12,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  message: {
    marginTop: 16,
    fontSize: 16,
    textAlign: 'center',
  },
});
