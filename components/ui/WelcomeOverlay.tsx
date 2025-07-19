import { router } from 'expo-router';
import React from 'react';
import {
    Modal,
    StyleSheet,
    Text,
    TouchableOpacity,
    View,
} from 'react-native';
import { usePalette } from '../../hooks/usePalette';
import { useAccounts } from '../../store/hooks';

export default function WelcomeOverlay() {
  const colors = usePalette();
  const accounts = useAccounts();

  // Only show overlay if there are no accounts
  if (accounts.length > 0) {
    return null;
  }

  const handleAddAccount = () => {
    router.push('/select-bank');
  };

  return (
    <>
      <Modal visible={true} animationType="fade" presentationStyle="overFullScreen">
        <View style={[styles.overlay, { backgroundColor: colors.background }]}>
          <View style={styles.welcomeContainer}>
            <Text style={[styles.welcomeEmoji, { color: colors.text }]}>🏦</Text>
            <Text style={[styles.welcomeTitle, { color: colors.text }]}>
              Welcome to NetWorth!
            </Text>
            <Text style={[styles.welcomeDescription, { color: colors.secondaryText }]}>
              Get started by adding your first account to begin tracking your finances
            </Text>
            <TouchableOpacity
              style={[styles.welcomeButton, { backgroundColor: colors.primary }]}
              onPress={handleAddAccount}
            >
              <Text style={[styles.welcomeButtonText, { color: colors.background }]}>
                Add Your First Account
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>
    </>
  );
}

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  welcomeContainer: {
    alignItems: 'center',
    paddingHorizontal: 40,
    paddingVertical: 60,
  },
  welcomeEmoji: {
    fontSize: 80,
    marginBottom: 32,
  },
  welcomeTitle: {
    fontSize: 32,
    fontWeight: 'bold',
    marginBottom: 20,
    textAlign: 'center',
  },
  welcomeDescription: {
    fontSize: 18,
    textAlign: 'center',
    marginBottom: 40,
    lineHeight: 26,
    maxWidth: 300,
  },
  welcomeButton: {
    paddingHorizontal: 40,
    paddingVertical: 18,
    borderRadius: 16,
    elevation: 4,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.15,
    shadowRadius: 8,
  },
  welcomeButtonText: {
    fontSize: 20,
    fontWeight: '600',
    textAlign: 'center',
  },
});
