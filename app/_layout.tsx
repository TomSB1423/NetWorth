import { Stack } from 'expo-router';
import AppProvider from '../providers/AppProvider';
// Import for development helper
import '../utils/clearStorage';

export default function RootLayout() {
  // Only use Stack here, Tabs are handled in (tabs)/_layout.tsx
  return (
    <AppProvider>
      <Stack>
        <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
      </Stack>
    </AppProvider>
  );
}
