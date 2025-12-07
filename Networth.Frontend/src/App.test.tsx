import { render, screen, waitFor } from "@testing-library/react";
import { vi, describe, it, expect, beforeEach, Mock } from "vitest";
import App from "./App";
import { api } from "./services/api";

// Mock the API module
vi.mock("./services/api", () => ({
    api: {
        getAccounts: vi.fn(),
        getAccountBalances: vi.fn(),
        syncInstitution: vi.fn(),
        getNetWorthHistory: vi.fn(),
    },
}));

// Mock the page components to simplify testing
vi.mock("./pages/Index", () => ({
    default: () => <div data-testid="dashboard-page">Dashboard Page</div>,
}));
vi.mock("./pages/Onboarding", () => ({
    default: () => <div data-testid="onboarding-page">Onboarding Page</div>,
}));
vi.mock("./pages/SelectBank", () => ({
    default: () => <div>Select Bank Page</div>,
}));
vi.mock("./pages/NameAccount", () => ({
    default: () => <div>Name Account Page</div>,
}));
vi.mock("./pages/Transactions", () => ({
    default: () => <div>Transactions Page</div>,
}));
vi.mock("./pages/NotFound", () => ({
    default: () => <div>Not Found Page</div>,
}));

describe("App", () => {
    beforeEach(() => {
        vi.clearAllMocks();
        // Reset URL
        window.history.pushState({}, "Home", "/");

        // Default API mocks
        (api.getAccounts as Mock).mockResolvedValue([]);
        (api.getAccountBalances as Mock).mockResolvedValue([]);
        (api.syncInstitution as Mock).mockResolvedValue({});
    });

    it("renders without crashing", async () => {
        render(<App />);
        // Should eventually show onboarding if no accounts
        await waitFor(() => {
            expect(screen.getByTestId("onboarding-page")).toBeInTheDocument();
        });
    });

    it("triggers sync when institutionId is present in URL", async () => {
        // Set URL with query param
        window.history.pushState(
            {},
            "Callback",
            "/?institutionId=test-inst-id"
        );

        render(<App />);

        // Should show loading indicator
        expect(screen.getByText(/Loading.../i)).toBeInTheDocument();

        // Should call syncInstitution
        await waitFor(() => {
            expect(api.syncInstitution).toHaveBeenCalledWith("test-inst-id");
        });

        // Should redirect to dashboard (remove query param)
        await waitFor(() => {
            expect(window.location.search).toBe("");
        });
    });

    it("redirects to dashboard if accounts exist", async () => {
        (api.getAccounts as Mock).mockResolvedValue([{ id: "1", name: "Test Account" }]);
        
        render(<App />);
    });
});
