---
sidebar_position: 4
---

# Frontend Architecture

The frontend is a modern **React SPA** built with Vite and Tailwind CSS, providing a responsive and intuitive user interface.

## Technology Stack

- **Build Tool**: Vite
- **Framework**: React 19
- **Styling**: Tailwind CSS
- **Routing**: React Router DOM v7
- **Data Fetching**: TanStack Query (React Query)
- **Icons**: Lucide React
- **Charts**: Recharts

## Project Structure

```
Networth.Frontend/
├── src/
│   ├── components/      # Reusable UI components
│   ├── hooks/          # Custom React hooks
│   ├── pages/          # Page components
│   ├── services/       # API service layer
│   ├── constants/      # Constants and mock data
│   ├── utils/          # Utility functions
│   ├── App.jsx         # Root component
│   └── main.jsx        # Entry point
├── public/             # Static assets
└── vite.config.js      # Vite configuration
```

## Key Features

### 1. Custom Hooks Pattern

The application uses custom hooks for state management and data fetching:

```javascript
// useInstitutions hook
export const useInstitutions = (country) => {
  return useQuery({
    queryKey: ['institutions', country],
    queryFn: () => institutionService.getInstitutions(country),
    enabled: !!country,
  });
};

// useAccounts hook
export const useAccounts = (userId) => {
  return useQuery({
    queryKey: ['accounts', userId],
    queryFn: () => accountService.getAccounts(userId),
    enabled: !!userId,
  });
};
```

### 2. Service Layer

API calls are abstracted into service modules:

```javascript
// src/services/accountService.js
const API_URL = import.meta.env.VITE_API_URL;

export const accountService = {
  async getAccounts(userId) {
    const response = await fetch(`${API_URL}/api/accounts?userId=${userId}`);
    if (!response.ok) throw new Error('Failed to fetch accounts');
    return response.json();
  },

  async getAccountBalances(accountId) {
    const response = await fetch(`${API_URL}/api/accounts/${accountId}/balances`);
    if (!response.ok) throw new Error('Failed to fetch balances');
    return response.json();
  },
};
```

### 3. TanStack Query Integration

React Query manages server state:

```javascript
// In App.jsx
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  );
}
```

### 4. Tailwind CSS Styling

Utility-first CSS with custom configuration:

```javascript
// tailwind.config.js
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        primary: '#8b5cf6',
        secondary: '#ec4899',
      },
    },
  },
  plugins: [],
};
```

## Component Structure

### Page Components

```javascript
// src/pages/Dashboard.jsx
import { useAccounts } from '../hooks/useAccounts';
import { AccountCard } from '../components/AccountCard';

export const Dashboard = () => {
  const userId = getCurrentUserId(); // Utility function
  const { data: accounts, isLoading } = useAccounts(userId);

  if (isLoading) return <LoadingSpinner />;

  return (
    <div className="container mx-auto px-4">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {accounts?.map(account => (
          <AccountCard key={account.id} account={account} />
        ))}
      </div>
    </div>
  );
};
```

### Reusable Components

```javascript
// src/components/AccountCard.jsx
export const AccountCard = ({ account }) => {
  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h3 className="text-xl font-semibold mb-2">{account.name}</h3>
      <p className="text-gray-600">{account.institutionId}</p>
      <div className="mt-4">
        <span className="text-2xl font-bold">
          {formatCurrency(account.balance)}
        </span>
      </div>
    </div>
  );
};
```

## Routing

React Router DOM v7 for navigation:

```javascript
// src/App.jsx
const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      { index: true, element: <Dashboard /> },
      { path: 'accounts', element: <Accounts /> },
      { path: 'institutions', element: <Institutions /> },
      { path: 'link-account', element: <LinkAccount /> },
    ],
  },
]);
```

## Environment Configuration

Environment variables are managed through Vite:

```javascript
// .env
VITE_API_URL=http://localhost:7071

// Accessed via
import.meta.env.VITE_API_URL
```

In production, Aspire sets this dynamically:

```csharp
// In AppHost
var frontend = builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend", "dev")
    .WithEnvironment("VITE_API_URL", functions.GetEndpoint("http"));
```

## State Management

The application uses a combination of:
- **React Query**: Server state (API data)
- **React Context**: Global client state (auth, theme)
- **Component State**: Local UI state

## Development Workflow

### Local Development

```bash
cd Networth.Frontend
npm install
npm run dev
```

The dev server runs on port 3000 with hot module replacement.

### Building for Production

```bash
npm run build
```

Outputs optimized bundle to `dist/` folder.

### Linting

```bash
npm run lint
```

ESLint configuration with React-specific rules.

## Mock Data

Development can use mock data for faster iteration:

```javascript
// src/constants/mockData.js
export const mockAccounts = [
  {
    id: '123',
    name: 'Checking Account',
    institutionId: 'HSBC_HBUKGB4B',
    balance: 5000.00,
  },
  // ...more mock data
];
```

## Performance Optimization

### Code Splitting

Vite automatically code-splits routes:

```javascript
const Dashboard = lazy(() => import('./pages/Dashboard'));
```

### Asset Optimization

- Images optimized during build
- CSS purged of unused classes
- JavaScript minified and tree-shaken

### Caching Strategy

React Query caches API responses:

```javascript
{
  staleTime: 5 * 60 * 1000, // 5 minutes
  cacheTime: 10 * 60 * 1000, // 10 minutes
}
```

## Testing

Frontend testing uses:
- **Vitest**: Unit testing
- **React Testing Library**: Component testing
- **Playwright**: E2E testing (via Aspire)

```javascript
import { render, screen } from '@testing-library/react';
import { AccountCard } from './AccountCard';

test('renders account name', () => {
  const account = { name: 'Test Account', balance: 1000 };
  render(<AccountCard account={account} />);
  expect(screen.getByText('Test Account')).toBeInTheDocument();
});
```
