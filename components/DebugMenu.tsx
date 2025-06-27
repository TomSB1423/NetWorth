// components/DebugMenu.tsx
// Debug menu component for development testing

import React, { useState } from 'react';
import {
  Modal,
  SafeAreaView,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';
import { DEBUG_MODE, DebugCommands } from '../utils/debug';

export interface DebugMenuProps {
  colors: any;
  style?: any;
}

export default function DebugMenu({ colors, style }: DebugMenuProps) {
  const [isVisible, setIsVisible] = useState(false);

  // Don't render anything if not in debug mode
  if (!DEBUG_MODE) {
    return null;
  }

  const handleCommand = async (command: () => Promise<void> | void) => {
    try {
      await command();
    } catch (error) {
      console.error('Debug command error:', error);
    }
  };

  const debugCommands = [
    {
      title: 'Add Sample Account',
      description: 'Adds a single test account for testing',
      action: () => handleCommand(DebugCommands.addSampleAccount),
      color: colors.primary,
    },
    {
      title: 'Add Multiple Sample Accounts',
      description: 'Adds checking, savings, credit, investment, and mortgage accounts',
      action: () => handleCommand(DebugCommands.addSampleAccounts),
      color: colors.success,
    },
    {
      title: 'Remove All Accounts',
      description: 'Removes all accounts from the app',
      action: () => handleCommand(DebugCommands.removeAllAccounts),
      color: colors.error,
    },
    {
      title: 'Reset App',
      description: 'Completely resets app to fresh state',
      action: () => handleCommand(DebugCommands.resetApp),
      color: colors.error,
    },
    {
      title: 'Show App Info',
      description: 'Shows current app state information',
      action: () => DebugCommands.showAppInfo(),
      color: colors.primary,
    },
  ];

  return (
    <>
      {/* Debug Trigger Button */}
      <TouchableOpacity
        style={[styles.debugTrigger, { backgroundColor: colors.error }, style]}
        onPress={() => setIsVisible(true)}
      >
        <Text style={styles.debugTriggerText}>🐛 DEBUG</Text>
      </TouchableOpacity>

      {/* Debug Menu Modal */}
      <Modal
        visible={isVisible}
        animationType="slide"
        presentationStyle="pageSheet"
        onRequestClose={() => setIsVisible(false)}
      >
        <SafeAreaView style={[styles.modalContainer, { backgroundColor: colors.background }]}>
          {/* Header */}
          <View style={[styles.modalHeader, { borderBottomColor: colors.border }]}>
            <Text style={[styles.modalTitle, { color: colors.text }]}>
              🐛 Debug Commands
            </Text>
            <TouchableOpacity
              style={[styles.closeButton, { backgroundColor: colors.card }]}
              onPress={() => setIsVisible(false)}
            >
              <Text style={[styles.closeButtonText, { color: colors.text }]}>✕</Text>
            </TouchableOpacity>
          </View>

          {/* Warning Banner */}
          <View style={[styles.warningBanner, { backgroundColor: colors.error + '20', borderColor: colors.error }]}>
            <Text style={[styles.warningText, { color: colors.error }]}>
              ⚠️ Development Only - These commands modify app data
            </Text>
          </View>

          {/* Commands List */}
          <ScrollView style={styles.commandsList} showsVerticalScrollIndicator={false}>
            {debugCommands.map((command, index) => (
              <TouchableOpacity
                key={index}
                style={[styles.commandItem, { backgroundColor: colors.card, borderColor: colors.border }]}
                onPress={() => {
                  command.action();
                  setIsVisible(false);
                }}
              >
                <View style={styles.commandContent}>
                  <Text style={[styles.commandTitle, { color: command.color }]}>
                    {command.title}
                  </Text>
                  <Text style={[styles.commandDescription, { color: colors.secondaryText }]}>
                    {command.description}
                  </Text>
                </View>
                <View style={[styles.commandIndicator, { backgroundColor: command.color }]} />
              </TouchableOpacity>
            ))}
          </ScrollView>

          {/* Footer */}
          <View style={[styles.footer, { borderTopColor: colors.border }]}>
            <Text style={[styles.footerText, { color: colors.secondaryText }]}>
              Debug mode is enabled in development builds
            </Text>
          </View>
        </SafeAreaView>
      </Modal>
    </>
  );
}

const styles = StyleSheet.create({
  debugTrigger: {
    position: 'absolute',
    top: 60,
    right: 20,
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 16,
    zIndex: 1000,
  },
  debugTriggerText: {
    color: 'white',
    fontSize: 12,
    fontWeight: 'bold',
  },
  modalContainer: {
    flex: 1,
  },
  modalHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 20,
    borderBottomWidth: 1,
  },
  modalTitle: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  closeButton: {
    width: 32,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center',
  },
  closeButtonText: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  warningBanner: {
    margin: 20,
    padding: 16,
    borderRadius: 12,
    borderWidth: 1,
  },
  warningText: {
    fontSize: 14,
    fontWeight: '600',
    textAlign: 'center',
  },
  commandsList: {
    flex: 1,
    paddingHorizontal: 20,
  },
  commandItem: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 16,
    marginBottom: 12,
    borderRadius: 12,
    borderWidth: 1,
  },
  commandContent: {
    flex: 1,
  },
  commandTitle: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 4,
  },
  commandDescription: {
    fontSize: 14,
    lineHeight: 18,
  },
  commandIndicator: {
    width: 4,
    height: 40,
    borderRadius: 2,
    marginLeft: 12,
  },
  footer: {
    padding: 20,
    borderTopWidth: 1,
  },
  footerText: {
    fontSize: 12,
    textAlign: 'center',
    fontStyle: 'italic',
  },
});
