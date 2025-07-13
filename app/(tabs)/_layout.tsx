import { Ionicons } from '@expo/vector-icons';
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
        headerShown: false,
      }}
    >
      <Tabs.Screen 
        name="dashboard" 
        options={{ 
          tabBarLabel: 'Dashboard',
          title: 'Dashboard',
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="stats-chart" size={size} color={color} />
          ),
        }} 
      />
      <Tabs.Screen 
        name="transactions" 
        options={{ 
          tabBarLabel: 'Accounts',
          title: 'Accounts',
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="wallet" size={size} color={color} />
          ),
        }} 
      />
      <Tabs.Screen 
        name="settings" 
        options={{ 
          tabBarLabel: 'Settings',
          title: 'Settings',
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="settings" size={size} color={color} />
          ),
        }} 
      />
    </Tabs>
  );
}
