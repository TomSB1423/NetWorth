import React from "react";
import { Pressable, StyleSheet, Text, View } from "react-native";
import DebugMenu from "../../components/DebugMenu";
import { useColorSchemeToggle } from "../../hooks/useColorSchemeToggle";
import { usePalette } from "../../hooks/usePalette";

export default function SettingsScreen() {
  const [colorScheme, mode, setMode] = useColorSchemeToggle();
  const colors = usePalette();
  console.log("[SettingsScreen] colorScheme:", colorScheme, "mode:", mode);

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <Text style={[styles.title, { color: colors.primary }]}>Settings</Text>
      <Text style={[styles.text, { color: colors.text }]}>Current color scheme: {mode}</Text>
      <View style={styles.buttonRow}>
        <Pressable
          style={[styles.button, { backgroundColor: colors.card, borderColor: colors.primary }, mode === "auto" && { backgroundColor: colors.highlight }]}
          onPress={() => setMode("auto")}
          accessibilityRole="button"
        >
          <Text style={[styles.buttonText, { color: colors.text }]}>Auto</Text>
        </Pressable>
        <Pressable
          style={[styles.button, { backgroundColor: colors.card, borderColor: colors.primary }, mode === "light" && { backgroundColor: colors.highlight }]}
          onPress={() => setMode("light")}
          accessibilityRole="button"
        >
          <Text style={[styles.buttonText, { color: colors.text }]}>Light</Text>
        </Pressable>
        <Pressable
          style={[styles.button, { backgroundColor: colors.card, borderColor: colors.primary }, mode === "dark" && { backgroundColor: colors.highlight }]}
          onPress={() => setMode("dark")}
          accessibilityRole="button"
        >
          <Text style={[styles.buttonText, { color: colors.text }]}>Dark</Text>
        </Pressable>
      </View>
      
      {/* Debug Menu - Only visible in development */}
      <DebugMenu colors={colors} style={styles.debugMenuPosition} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
  },
  title: {
    fontSize: 24,
    fontWeight: "bold",
    marginBottom: 16,
  },
  text: {
    fontSize: 16,
    marginBottom: 16,
  },
  buttonRow: {
    flexDirection: "row",
    gap: 12,
  },
  button: {
    paddingVertical: 10,
    paddingHorizontal: 18,
    borderRadius: 8,
    borderWidth: 1,
    marginHorizontal: 4,
  },
  buttonText: {
    fontWeight: "bold",
  },
  debugMenuPosition: {
    top: 80, // Position below the status bar on settings screen
    right: 20,
  },
});
