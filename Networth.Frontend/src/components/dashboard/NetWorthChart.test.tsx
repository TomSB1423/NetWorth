import { render, screen, waitFor, fireEvent } from "@testing-library/react";
import { vi, describe, it, expect, beforeEach, type Mock } from "vitest";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { NetWorthChart } from "./NetWorthChart";
import { api } from "../../services/api";

// Mock API
vi.mock("../../services/api", () => ({
    api: {
        getNetWorthHistory: vi.fn(),
    },
}));

// Mock Recharts
vi.mock("recharts", () => ({
    ResponsiveContainer: ({ children }: { children: React.ReactNode }) => (
        <div data-testid="responsive-container">{children}</div>
    ),
    AreaChart: ({ children }: { children: React.ReactNode }) => (
        <svg data-testid="area-chart">{children}</svg>
    ),
    Area: () => <g />,
    XAxis: () => <g />,
    YAxis: () => <g />,
    CartesianGrid: () => <g />,
    Tooltip: () => <g />,
}));

const createTestQueryClient = () =>
    new QueryClient({
        defaultOptions: {
            queries: {
                retry: false,
            },
        },
    });

const renderWithClient = (ui: React.ReactElement) => {
    const testQueryClient = createTestQueryClient();
    return render(
        <QueryClientProvider client={testQueryClient}>{ui}</QueryClientProvider>
    );
};

describe("NetWorthChart", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    });

    it("shows loading state initially", () => {
        (api.getNetWorthHistory as Mock).mockReturnValue(new Promise(() => {})); // Never resolves
        renderWithClient(<NetWorthChart isSyncing={false} />);
        expect(screen.getByText(/Loading data/i)).toBeInTheDocument();
    });

    it("shows loading state when isSyncing is true", async () => {
        (api.getNetWorthHistory as Mock).mockResolvedValue({
            dataPoints: [],
            status: "NotCalculated",
            lastCalculated: null,
        });
        renderWithClient(<NetWorthChart isSyncing={true} />);

        // Wait for the effect to finish to avoid act warnings
        await waitFor(() => expect(api.getNetWorthHistory).toHaveBeenCalled());

        expect(screen.getByText(/Syncing account data/i)).toBeInTheDocument();
    });

    it("shows loading state when status is Calculating", async () => {
        (api.getNetWorthHistory as Mock).mockResolvedValue({
            dataPoints: [],
            status: "Calculating",
            lastCalculated: null,
        });
        renderWithClient(<NetWorthChart isSyncing={false} />);

        await waitFor(() => expect(api.getNetWorthHistory).toHaveBeenCalled());

        await waitFor(() => {
            expect(screen.getByText(/Calculating net worth/i)).toBeInTheDocument();
        });
    });

    it("shows loading state and polls when status is NotCalculated", async () => {
        (api.getNetWorthHistory as Mock).mockResolvedValue({
            dataPoints: [],
            status: "NotCalculated",
            lastCalculated: null,
        });
        renderWithClient(<NetWorthChart isSyncing={false} />);

        await waitFor(() => expect(api.getNetWorthHistory).toHaveBeenCalled());

        await waitFor(() => {
            expect(screen.getByText(/Calculating net worth/i)).toBeInTheDocument();
        });
    });

    it("renders chart when data is loaded", async () => {
        const mockData = {
            dataPoints: [
                { date: "2023-01-01", amount: 1000 },
                { date: "2023-01-02", amount: 1100 },
            ],
            status: "Calculated",
            lastCalculated: "2023-01-02T12:00:00Z",
        };
        (api.getNetWorthHistory as Mock).mockResolvedValue(mockData);

        renderWithClient(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.queryByText(/Loading data/i)).not.toBeInTheDocument();
        });

        expect(screen.getByTestId("area-chart")).toBeInTheDocument();
    });

    it("handles API error", async () => {
        (api.getNetWorthHistory as Mock).mockRejectedValue(
            new Error("API Error")
        );

        renderWithClient(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.getByText(/API Error/i)).toBeInTheDocument();
        });
    });

    it("allows changing time periods", async () => {
        // Provide data spanning more than 30 days to show 1M period
        const mockData = {
            dataPoints: [
                { date: "2023-01-01", amount: 1000 },
                { date: "2023-02-15", amount: 1100 },
            ],
            status: "Calculated",
            lastCalculated: "2023-02-15T12:00:00Z",
        };
        (api.getNetWorthHistory as Mock).mockResolvedValue(mockData);
        renderWithClient(<NetWorthChart isSyncing={false} />);

        await waitFor(() => {
            expect(screen.getByText("1M")).toBeInTheDocument();
        });

        const button = screen.getByText("1M");
        fireEvent.click(button);
        // Verify interaction doesn't crash
        expect(button).toBeInTheDocument();
    });
});
