# Application State Management

This app uses a **single unified provider** architecture for simple and maintainable state management.

## Quick Start

```tsx
import { useAccounts, useAppDispatch } from '../hooks';
import { addAccount } from '../store/slices/accountsSlice';

function MyComponent() {
  const accounts = useAccounts();
  const dispatch = useAppDispatch();

  const handleAddAccount = (accountData) => {
    dispatch(addAccount(accountData));
  };

  return (
    // Your component JSX
  );
}
```

## Architecture Overview

### Single Provider

- **Location**: `providers/AppProvider.tsx`
- **What it provides**: Redux state, persistence, color scheme, error boundaries
- **Usage**: Wraps the entire app in `app/_layout.tsx`

### State Structure

```
AppState
├── accounts (persisted)
│   ├── accounts[]
│   ├── isLoading
│   ├── error
│   └── isInitialized
├── metrics (calculated)
│   ├── metrics
│   ├── netWorthHistory[]
│   ├── isLoading
│   └── error
└── ui (persisted)
    ├── selectedTimeRange
    ├── theme
    └── isFirstLaunch
```

### Available Hooks

All hooks are exported from `hooks/index.ts`:

**Account Hooks:**

- `useAccounts()` - Get all accounts
- `useAccount(id)` - Get specific account
- `useAccountsLoading()` - Loading state
- `useAccountsError()` - Error state

**Metrics Hooks:**

- `useMetrics()` - Get calculated metrics
- `useNetWorthHistory()` - Get historical data
- `useTotalNetWorth()` - Get total net worth

**UI Hooks:**

- `useTheme()` - Current theme
- `useSelectedTimeRange()` - Selected time range
- `useIsFirstLaunch()` - Whether this is the first app launch

**Utility Hooks:**

- `useAppDispatch()` - Dispatch actions
- `useAppSelector()` - Select state
- `useAppLoading()` - Combined loading state
- `useAppError()` - Combined error state

### State Persistence

- ✅ **Accounts**: Persisted (user data)
- ✅ **UI**: Persisted (user preferences)
- ❌ **Metrics**: Not persisted (recalculated)

### Best Practices

1. **Import from single source**: `import { useAccounts } from '../hooks'`
2. **Use typed hooks**: All hooks are TypeScript typed
3. **Dispatch actions**: Use `dispatch(actionCreator(payload))`
4. **Error handling**: Check `useAppError()` for global errors
5. **Loading states**: Use `useAppLoading()` for global loading state

## Common Patterns

### Adding Data

```tsx
const dispatch = useAppDispatch();
dispatch(addAccount(accountData));
```

### Reading Data

```tsx
const accounts = useAccounts();
const isLoading = useAccountsLoading();
const error = useAccountsError();
```

### UI State

```tsx
const theme = useTheme();
const dispatch = useAppDispatch();
dispatch(setTheme("dark"));
```
