import { Stack } from 'expo-router';
import { AuthProvider } from '../contexts/AuthContext';
import AppProvider from '../providers/AppProvider';
// Import for development helpers
import '../utils/debug';

export default function RootLayout() {
  return (
    <AppProvider>
      <AuthProvider>
        <Stack screenOptions={{ headerShown: false }}>
          <Stack.Screen name="index" />
          <Stack.Screen name="(tabs)" />
          <Stack.Screen name="onboarding" />
          <Stack.Screen name="add-account" />
          <Stack.Screen name="select-bank" />
        </Stack>
      </AuthProvider>
    </AppProvider>
  );
}
