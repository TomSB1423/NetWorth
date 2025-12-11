import { render, screen } from "@testing-library/react";
import { vi, describe, it, expect, type Mock } from "vitest";
import { TopAccounts } from "./TopAccounts";
import { useAccounts } from "../../contexts/AccountContext";
import { BrowserRouter } from "react-router-dom";

vi.mock("../../contexts/AccountContext", () => ({
    useAccounts: vi.fn(),
}));

describe("TopAccounts", () => {
    it("shows loading state when isSyncing is true", () => {
        (useAccounts as Mock).mockReturnValue({ accounts: [], balances: [] });
        render(
            <BrowserRouter>
                <TopAccounts isSyncing={true} />
            </BrowserRouter>
        );
        expect(screen.getByText("Loading data...")).toBeInTheDocument();
    });

    it("renders accounts when not syncing", () => {
        (useAccounts as Mock).mockReturnValue({
            accounts: [{ id: "1", name: "Test Account" }],
            balances: [
                {
                    accountId: "1",
                    balances: [
                        {
                            balanceType: "interimAvailable",
                            amount: "100",
                            currency: "GBP",
                        },
                    ],
                },
            ],
        });

        render(
            <BrowserRouter>
                <TopAccounts isSyncing={false} />
            </BrowserRouter>
        );
        expect(screen.getByText("Test Account")).toBeInTheDocument();
    });
});
