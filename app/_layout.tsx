import { Stack } from 'expo-router';
import AppProvider from '../providers/AppProvider';
// Import for development helper
import '../utils/clearStorage';

export default function RootLayout() {
  // Only use Stack here, Tabs are handled in (tabs)/_layout.tsx
  return (
    <AppProvider>
      <Stack screenOptions={{ headerShown: false }}>
        <Stack.Screen name="index" />
        <Stack.Screen name="onboarding" />
        <Stack.Screen name="(tabs)" />
        <Stack.Screen 
          name="select-bank" 
          options={{ 
            presentation: 'modal',
            headerShown: false 
          }} 
        />
        <Stack.Screen 
          name="add-account" 
          options={{ 
            headerShown: false 
          }} 
        />
      </Stack>
    </AppProvider>
  );
}
