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
        getUser: vi.fn(),
    },
}));

// Mock MSAL browser with proper class
vi.mock("@azure/msal-browser", async (importOriginal) => {
    const actual = await importOriginal<typeof import("@azure/msal-browser")>();
    return {
        ...actual,
        PublicClientApplication: class MockPublicClientApplication {
            initialize = vi.fn().mockResolvedValue(undefined);
            getAllAccounts = vi.fn().mockReturnValue([]);
            getActiveAccount = vi.fn().mockReturnValue(null);
            setActiveAccount = vi.fn();
            acquireTokenSilent = vi
                .fn()
                .mockResolvedValue({ accessToken: "test-token" });
            loginPopup = vi.fn().mockResolvedValue({});
            logoutPopup = vi.fn().mockResolvedValue(undefined);
            addEventCallback = vi.fn();
            removeEventCallback = vi.fn();
        },
    };
});

vi.mock("@azure/msal-react", async (importOriginal) => {
    const actual = await importOriginal<typeof import("@azure/msal-react")>();
    return {
        ...actual,
        MsalProvider: ({ children }: { children: React.ReactNode }) => (
            <>{children}</>
        ),
        useMsal: () => ({
            instance: {
                getAllAccounts: vi.fn().mockReturnValue([]),
                getActiveAccount: vi.fn().mockReturnValue(null),
                setActiveAccount: vi.fn(),
                acquireTokenSilent: vi
                    .fn()
                    .mockResolvedValue({ accessToken: "test-token" }),
                loginPopup: vi.fn().mockResolvedValue({}),
                logoutPopup: vi.fn().mockResolvedValue(undefined),
                addEventCallback: vi.fn(),
                removeEventCallback: vi.fn(),
            },
            accounts: [],
            inProgress: "none",
        }),
        useIsAuthenticated: () => false,
        useAccount: () => null,
    };
});

// Mock the page components to simplify testing
vi.mock("./pages/Landing", () => ({
    default: ({ onGetStarted }: { onGetStarted: () => void }) => (
        <div data-testid="landing-page">
            <button onClick={onGetStarted}>Get Started</button>
        </div>
    ),
}));
vi.mock("./pages/onboarding/OnboardingTutorial", () => ({
    default: () => <div data-testid="onboarding-page">Onboarding Page</div>,
}));
vi.mock("./pages/onboarding/SelectBank", () => ({
    default: () => <div>Select Bank Page</div>,
}));
vi.mock("./pages/onboarding/NameAccount", () => ({
    default: () => <div>Name Account Page</div>,
}));
vi.mock("./pages/Transactions", () => ({
    default: () => <div>Transactions Page</div>,
}));
vi.mock("./pages/NotFound", () => ({
    default: () => <div>Not Found Page</div>,
}));
vi.mock("./pages/dashboard/Overview", () => ({
    default: () => <div data-testid="dashboard-page">Dashboard Page</div>,
}));
vi.mock("./pages/dashboard/Accounts", () => ({
    default: () => <div>Accounts Page</div>,
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
        (api.getUser as Mock).mockResolvedValue({
            id: "test-user",
            isNewUser: false,
            hasCompletedOnboarding: true,
        });
    });

    it("renders landing page when not authenticated", async () => {
        render(<App />);

        // Should show landing page for unauthenticated users
        await waitFor(() => {
            expect(screen.getByTestId("landing-page")).toBeInTheDocument();
        });
    });

    it("renders without crashing", () => {
        // Simple smoke test
        expect(() => render(<App />)).not.toThrow();
    });
});
