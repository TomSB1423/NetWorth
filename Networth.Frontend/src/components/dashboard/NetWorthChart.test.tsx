import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach, type Mock } from 'vitest';
import { NetWorthChart } from './NetWorthChart';
import { api } from '../../services/api';

// Mock API
vi.mock('../../services/api', () => ({
    api: {
        getNetWorthHistory: vi.fn(),
    },
}));

// Mock Recharts
vi.mock('recharts', () => ({
    ResponsiveContainer: ({ children }: { children: React.ReactNode }) => <div data-testid="responsive-container">{children}</div>,
    AreaChart: ({ children }: { children: React.ReactNode }) => <svg data-testid="area-chart">{children}</svg>,
    Area: () => <g />,
    XAxis: () => <g />,
    YAxis: () => <g />,
    CartesianGrid: () => <g />,
    Tooltip: () => <g />,
}));

describe('NetWorthChart', () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it('shows loading state initially', () => {
        (api.getNetWorthHistory as Mock).mockReturnValue(new Promise(() => {})); // Never resolves
        render(<NetWorthChart isSyncing={false} />);
        expect(screen.getByText(/Loading data/i)).toBeInTheDocument();
    });

    it('shows loading state when isSyncing is true', async () => {
        (api.getNetWorthHistory as Mock).mockResolvedValue([]);
        render(<NetWorthChart isSyncing={true} />);
        
        // Wait for the effect to finish to avoid act warnings
        await waitFor(() => expect(api.getNetWorthHistory).toHaveBeenCalled());

        expect(screen.getByText(/Loading data/i)).toBeInTheDocument();
    });

    it('renders chart when data is loaded', async () => {
        const mockData = [
            { date: '2023-01-01', value: 1000 },
            { date: '2023-01-02', value: 1100 },
        ];
        (api.getNetWorthHistory as Mock).mockResolvedValue(mockData);

        render(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.queryByText(/Loading data/i)).not.toBeInTheDocument();
        });

        expect(screen.getByTestId('area-chart')).toBeInTheDocument();
    });

    it('handles API error', async () => {
        (api.getNetWorthHistory as Mock).mockRejectedValue(new Error('API Error'));

        render(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.getByText(/Failed to load data/i)).toBeInTheDocument();
        });
    });

    it('allows changing time periods', async () => {
        (api.getNetWorthHistory as Mock).mockResolvedValue([]);
        render(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.getByText('1M')).toBeInTheDocument();
        });

        const button = screen.getByText('1M');
        fireEvent.click(button);
        // Verify interaction doesn't crash
        expect(button).toBeInTheDocument();
    });
});
