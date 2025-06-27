// app/index.tsx
// Entry point - AuthProvider handles the routing logic

import { Redirect } from 'expo-router';

export default function Index() {
  // AuthProvider will handle the proper navigation based on auth state
  // Start with dashboard tab, AuthProvider will redirect if needed
  return <Redirect href="/(tabs)/dashboard" />;
}
