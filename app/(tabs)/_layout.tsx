import { Tabs } from 'expo-router';
import { useColorSchemeToggle } from '../../hooks/useColorSchemeToggle';
import { usePalette } from '../../hooks/usePalette';

export default function TabLayout() {
  const colors = usePalette();
  const [colorScheme] = useColorSchemeToggle();
  return (
    <Tabs
      key={colorScheme}
      screenOptions={{
        tabBarActiveTintColor: colors.primary,
        tabBarInactiveTintColor: colors.secondaryText,
        tabBarStyle: {
          backgroundColor: colors.card,
          borderTopColor: colors.border,
        },
        headerShown: false, // Hide the header at the top of each tab screen
      }}
    >
      <Tabs.Screen name="index" options={{}} />
      <Tabs.Screen name="transactions" options={{ title: 'Accounts', tabBarLabel: 'Accounts' }} />
      <Tabs.Screen name="settings" options={{ title: 'Settings', tabBarLabel: 'Settings' }} />
    </Tabs>
  );
}
