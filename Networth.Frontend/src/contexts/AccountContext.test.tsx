import { render, screen, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, type Mock } from 'vitest';
import { AccountProvider, useAccounts } from './AccountContext';
import { api } from '../services/api';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Mock API
vi.mock('../services/api', () => ({
    api: {
        getAccounts: vi.fn(),
        getAccountBalances: vi.fn(),
    },
}));

const TestComponent = () => {
    const { accounts, balances, isLoading, hasAccounts } = useAccounts();
    if (isLoading) return <div>Loading...</div>;
    return (
        <div>
            <div data-testid="has-accounts">{hasAccounts.toString()}</div>
            <div data-testid="accounts-count">{accounts.length}</div>
            <div data-testid="balances-count">{balances.length}</div>
        </div>
    );
};

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: {
                retry: false,
            },
        },
    });
    return ({ children }: { children: React.ReactNode }) => (
        <QueryClientProvider client={queryClient}>
            <AccountProvider>{children}</AccountProvider>
        </QueryClientProvider>
    );
};

describe('AccountContext', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('provides initial state', async () => {
        (api.getAccounts as Mock).mockResolvedValue([]);
        (api.getAccountBalances as Mock).mockResolvedValue([]);

        render(<TestComponent />, { wrapper: createWrapper() });

        expect(screen.getByText('Loading...')).toBeInTheDocument();
        
        await waitFor(() => {
            expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
        });

        expect(screen.getByTestId('has-accounts')).toHaveTextContent('false');
        expect(screen.getByTestId('accounts-count')).toHaveTextContent('0');
    });

    it('fetches accounts and balances', async () => {
        const mockAccounts = [{ id: '1', name: 'Acc 1' }];
        const mockBalances = [{ amount: 100 }];
        
        (api.getAccounts as Mock).mockResolvedValue(mockAccounts);
        (api.getAccountBalances as Mock).mockResolvedValue(mockBalances);

        render(<TestComponent />, { wrapper: createWrapper() });

        await waitFor(() => {
            expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
        });

        expect(screen.getByTestId('has-accounts')).toHaveTextContent('true');
        expect(screen.getByTestId('accounts-count')).toHaveTextContent('1');
        expect(screen.getByTestId('balances-count')).toHaveTextContent('1');
    });
});
