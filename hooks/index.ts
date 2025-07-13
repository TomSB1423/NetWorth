// hooks/index.ts
// Single export point for all application hooks

// Redux hooks
export {
    useAccount, useAccounts, useAccountsError,
    useAccountsInitialized, useAccountsLoading, useAppDispatch, useAppError, useAppLoading, useAppSelector, useIsFirstLaunch, useMetrics, useMetricsError, useMetricsLoading, useNetWorthHistory, useSelectedTimeRange, useTheme, useTotalNetWorth
} from '../store/hooks';

// Color scheme hooks
export { useColorSchemeToggle, type ColorSchemeMode, type EffectiveColorScheme } from './useColorSchemeToggle';

// UI hooks
export { usePalette } from './usePalette';
