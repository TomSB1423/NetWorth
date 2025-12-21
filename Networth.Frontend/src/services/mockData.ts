/**
 * Mock data generator for the Networth application.
 * Generates realistic financial data spanning 14+ months with:
 * - Monthly salary deposits
 * - Rent payments
 * - Utility bills (electricity, gas, water, internet)
 * - Transport costs (TfL, fuel, occasional Uber)
 * - Groceries (weekly Tesco, Sainsbury's, occasional M&S)
 * - Subscriptions (Netflix, Spotify, gym)
 * - Transfers between accounts
 * - Occasional gifts and dining out
 */

import type {
    Account,
    Balance,
    Institution,
    Transaction,
    User,
    NetWorthHistoryResponse,
    NetWorthDataPoint,
    PagedResponse,
    LinkInstitutionResponse,
    AccountCategory,
} from "../types";

// ============================================================================
// Constants
// ============================================================================

const MOCK_USER_ID = "mock-user-001";
const CURRENCY = "GBP";
const START_DATE = new Date("2023-10-01");
const END_DATE = new Date();

// Account IDs
const CURRENT_ACCOUNT_ID = "acc-current-001";
const SAVINGS_ACCOUNT_ID = "acc-savings-001";
const ISA_ACCOUNT_ID = "acc-isa-001";

// Institution IDs
const MONZO_ID = "MONZO_MONZGB2L";
const STARLING_ID = "STARLING_SRLGGB2L";
const NATIONWIDE_ID = "NATIONWIDE_NAIAGB21";

// ============================================================================
// Mock Institutions
// ============================================================================

export const mockInstitutions: Institution[] = [
    {
        id: MONZO_ID,
        name: "Monzo Bank",
        logoUrl: "https://cdn.nordigen.com/ais/MONZO_MONZGB2L.png",
    },
    {
        id: STARLING_ID,
        name: "Starling Bank",
        logoUrl: "https://cdn.nordigen.com/ais/STARLING_SRLGGB2L.png",
    },
    {
        id: NATIONWIDE_ID,
        name: "Nationwide Building Society",
        logoUrl: "https://cdn.nordigen.com/ais/NATIONWIDE_NAIAGB21.png",
    },
    {
        id: "HSBC_HBUKGB4B",
        name: "HSBC UK",
        logoUrl: "https://cdn.nordigen.com/ais/HSBC_HBUKGB4B.png",
    },
    {
        id: "BARCLAYS_BARCGB22",
        name: "Barclays",
        logoUrl: "https://cdn.nordigen.com/ais/BARCLAYS_BARCGB22.png",
    },
    {
        id: "LLOYDS_LOYDGB2L",
        name: "Lloyds Bank",
        logoUrl: "https://cdn.nordigen.com/ais/LLOYDS_LOYDGB2L.png",
    },
    {
        id: "SANTANDER_ABBYGB2L",
        name: "Santander UK",
        logoUrl: "https://cdn.nordigen.com/ais/SANTANDER_ABBYGB2L.png",
    },
    {
        id: "NATWEST_NWBKGB2L",
        name: "NatWest",
        logoUrl: "https://cdn.nordigen.com/ais/NATWEST_NWBKGB2L.png",
    },
];

// ============================================================================
// Mock User
// ============================================================================

export const mockUser: User = {
    userId: MOCK_USER_ID,
    name: "Demo User",
    isNewUser: false,
    hasCompletedOnboarding: true,
};

// ============================================================================
// Mock Accounts
// ============================================================================

export const mockAccounts: Account[] = [
    {
        id: CURRENT_ACCOUNT_ID,
        userId: MOCK_USER_ID,
        requisitionId: "req-001",
        institutionId: MONZO_ID,
        institutionName: "Monzo Bank",
        institutionLogo: "https://cdn.nordigen.com/ais/MONZO_MONZGB2L.png",
        name: "Personal Current Account",
        displayName: "Main Spending",
        category: "Spending" as AccountCategory,
        iban: "GB82MONZ04000012345678",
        currency: CURRENCY,
        product: "Current Account",
        lastSynced: new Date().toISOString(),
    },
    {
        id: SAVINGS_ACCOUNT_ID,
        userId: MOCK_USER_ID,
        requisitionId: "req-002",
        institutionId: STARLING_ID,
        institutionName: "Starling Bank",
        institutionLogo: "https://cdn.nordigen.com/ais/STARLING_SRLGGB2L.png",
        name: "Easy Access Savings",
        displayName: "Emergency Fund",
        category: "Savings" as AccountCategory,
        iban: "GB15SRLG60837012345678",
        currency: CURRENCY,
        product: "Savings Account",
        lastSynced: new Date().toISOString(),
    },
    {
        id: ISA_ACCOUNT_ID,
        userId: MOCK_USER_ID,
        requisitionId: "req-003",
        institutionId: NATIONWIDE_ID,
        institutionName: "Nationwide Building Society",
        institutionLogo: "https://cdn.nordigen.com/ais/NATIONWIDE_NAIAGB21.png",
        name: "Cash ISA",
        displayName: "ISA Savings",
        category: "Investment" as AccountCategory,
        iban: "GB33NAIA07001412345678",
        currency: CURRENCY,
        product: "Cash ISA",
        lastSynced: new Date().toISOString(),
    },
];

// ============================================================================
// Transaction Generation Helpers
// ============================================================================

interface TransactionTemplate {
    creditorName?: string;
    debtorName?: string;
    description: string;
    amountRange: [number, number];
    isCredit: boolean;
}

const MONTHLY_SALARY: TransactionTemplate = {
    debtorName: "ACME TECHNOLOGIES LTD",
    description: "SALARY PAYMENT",
    amountRange: [3850, 3850],
    isCredit: true,
};

const RENT: TransactionTemplate = {
    creditorName: "PROPERTY MANAGEMENT CO",
    description: "MONTHLY RENT PAYMENT",
    amountRange: [1250, 1250],
    isCredit: false,
};

const UTILITIES: TransactionTemplate[] = [
    {
        creditorName: "BRITISH GAS",
        description: "GAS & ELECTRICITY DD",
        amountRange: [85, 145],
        isCredit: false,
    },
    {
        creditorName: "THAMES WATER",
        description: "WATER RATES DD",
        amountRange: [35, 45],
        isCredit: false,
    },
    {
        creditorName: "BT GROUP PLC",
        description: "BROADBAND & PHONE",
        amountRange: [45, 55],
        isCredit: false,
    },
    {
        creditorName: "COUNCIL TAX",
        description: "COUNCIL TAX DD",
        amountRange: [145, 165],
        isCredit: false,
    },
];

const SUBSCRIPTIONS: TransactionTemplate[] = [
    {
        creditorName: "NETFLIX.COM",
        description: "NETFLIX SUBSCRIPTION",
        amountRange: [15.99, 15.99],
        isCredit: false,
    },
    {
        creditorName: "SPOTIFY AB",
        description: "SPOTIFY PREMIUM",
        amountRange: [10.99, 10.99],
        isCredit: false,
    },
    {
        creditorName: "PURE GYM LTD",
        description: "GYM MEMBERSHIP",
        amountRange: [24.99, 24.99],
        isCredit: false,
    },
    {
        creditorName: "AMAZON PRIME",
        description: "AMAZON PRIME MEMBERSHIP",
        amountRange: [8.99, 8.99],
        isCredit: false,
    },
];

const TRANSPORT: TransactionTemplate[] = [
    {
        creditorName: "TFL TRAVEL",
        description: "TFL CONTACTLESS TRAVEL",
        amountRange: [2.5, 8.5],
        isCredit: false,
    },
    {
        creditorName: "SHELL UK",
        description: "FUEL PURCHASE",
        amountRange: [45, 75],
        isCredit: false,
    },
    {
        creditorName: "UBER *TRIP",
        description: "UBER TRIP",
        amountRange: [8, 25],
        isCredit: false,
    },
];

const GROCERIES: TransactionTemplate[] = [
    {
        creditorName: "TESCO STORES LTD",
        description: "GROCERIES",
        amountRange: [35, 95],
        isCredit: false,
    },
    {
        creditorName: "SAINSBURYS",
        description: "GROCERIES",
        amountRange: [25, 85],
        isCredit: false,
    },
    {
        creditorName: "M&S FOODHALL",
        description: "FOOD & GROCERIES",
        amountRange: [15, 45],
        isCredit: false,
    },
    {
        creditorName: "LIDL GB",
        description: "GROCERIES",
        amountRange: [20, 60],
        isCredit: false,
    },
];

const DINING_OUT: TransactionTemplate[] = [
    {
        creditorName: "PRET A MANGER",
        description: "FOOD & DRINK",
        amountRange: [5, 12],
        isCredit: false,
    },
    {
        creditorName: "NANDOS",
        description: "RESTAURANT",
        amountRange: [15, 35],
        isCredit: false,
    },
    {
        creditorName: "WAGAMAMA",
        description: "RESTAURANT",
        amountRange: [18, 40],
        isCredit: false,
    },
    {
        creditorName: "THE PUB CO",
        description: "PUB FOOD & DRINKS",
        amountRange: [25, 65],
        isCredit: false,
    },
    {
        creditorName: "DELIVEROO",
        description: "FOOD DELIVERY",
        amountRange: [15, 35],
        isCredit: false,
    },
];

const SHOPPING: TransactionTemplate[] = [
    {
        creditorName: "AMAZON UK",
        description: "AMAZON PURCHASE",
        amountRange: [10, 150],
        isCredit: false,
    },
    {
        creditorName: "JOHN LEWIS",
        description: "RETAIL PURCHASE",
        amountRange: [25, 200],
        isCredit: false,
    },
    {
        creditorName: "BOOTS",
        description: "PHARMACY/HEALTH",
        amountRange: [8, 35],
        isCredit: false,
    },
];

const TRANSFERS: TransactionTemplate[] = [
    {
        creditorName: "TO SAVINGS",
        description: "TRANSFER TO SAVINGS",
        amountRange: [200, 500],
        isCredit: false,
    },
];

const GIFTS_RECEIVED: TransactionTemplate = {
    debtorName: "FAMILY MEMBER",
    description: "GIFT/BIRTHDAY",
    amountRange: [50, 200],
    isCredit: true,
};

// ============================================================================
// Random Helpers
// ============================================================================

function randomBetween(min: number, max: number): number {
    return Math.round((Math.random() * (max - min) + min) * 100) / 100;
}

function randomDate(start: Date, end: Date): Date {
    return new Date(
        start.getTime() + Math.random() * (end.getTime() - start.getTime())
    );
}

function pickRandom<T>(arr: T[]): T {
    return arr[Math.floor(Math.random() * arr.length)];
}

function formatDate(date: Date): string {
    return date.toISOString().split("T")[0];
}

function generateTransactionId(): string {
    return `txn-${Date.now()}-${Math.random().toString(36).substring(2, 11)}`;
}

// ============================================================================
// Transaction Generation
// ============================================================================

function createTransaction(
    accountId: string,
    date: Date,
    template: TransactionTemplate
): Transaction {
    const amount = randomBetween(
        template.amountRange[0],
        template.amountRange[1]
    );
    const signedAmount = template.isCredit ? amount : -amount;
    const txnId = generateTransactionId();

    return {
        id: `${accountId}_${txnId}`,
        accountId,
        transactionId: txnId,
        bookingDate: formatDate(date),
        valueDate: formatDate(date),
        amount: signedAmount.toFixed(2),
        currency: CURRENCY,
        creditorName: template.creditorName,
        debtorName: template.debtorName,
        remittanceInformationUnstructured: template.description,
        isPending: false,
    };
}

function generateCurrentAccountTransactions(): Transaction[] {
    const transactions: Transaction[] = [];
    const currentDate = new Date(START_DATE);

    while (currentDate <= END_DATE) {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        // Salary on last working day of month (approx 28th)
        const salaryDate = new Date(year, month, 28);
        if (salaryDate <= END_DATE) {
            transactions.push(
                createTransaction(
                    CURRENT_ACCOUNT_ID,
                    salaryDate,
                    MONTHLY_SALARY
                )
            );
        }

        // Rent on 1st of month
        const rentDate = new Date(year, month, 1);
        if (rentDate >= START_DATE && rentDate <= END_DATE) {
            transactions.push(
                createTransaction(CURRENT_ACCOUNT_ID, rentDate, RENT)
            );
        }

        // Utilities (various dates in month)
        for (const util of UTILITIES) {
            const utilDate = new Date(
                year,
                month,
                Math.floor(Math.random() * 15) + 5
            );
            if (utilDate >= START_DATE && utilDate <= END_DATE) {
                transactions.push(
                    createTransaction(CURRENT_ACCOUNT_ID, utilDate, util)
                );
            }
        }

        // Subscriptions (various dates)
        for (const sub of SUBSCRIPTIONS) {
            const subDate = new Date(
                year,
                month,
                Math.floor(Math.random() * 10) + 1
            );
            if (subDate >= START_DATE && subDate <= END_DATE) {
                transactions.push(
                    createTransaction(CURRENT_ACCOUNT_ID, subDate, sub)
                );
            }
        }

        // TfL travel (15-25 times per month on weekdays)
        const tflCount = Math.floor(Math.random() * 10) + 15;
        for (let i = 0; i < tflCount; i++) {
            const day = Math.floor(Math.random() * daysInMonth) + 1;
            const tflDate = new Date(year, month, day);
            if (
                tflDate >= START_DATE &&
                tflDate <= END_DATE &&
                tflDate.getDay() !== 0 &&
                tflDate.getDay() !== 6
            ) {
                transactions.push(
                    createTransaction(CURRENT_ACCOUNT_ID, tflDate, TRANSPORT[0])
                );
            }
        }

        // Fuel (1-2 times per month)
        const fuelCount = Math.floor(Math.random() * 2) + 1;
        for (let i = 0; i < fuelCount; i++) {
            const fuelDate = new Date(
                year,
                month,
                Math.floor(Math.random() * daysInMonth) + 1
            );
            if (fuelDate >= START_DATE && fuelDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        fuelDate,
                        TRANSPORT[1]
                    )
                );
            }
        }

        // Uber (2-4 times per month)
        const uberCount = Math.floor(Math.random() * 3) + 2;
        for (let i = 0; i < uberCount; i++) {
            const uberDate = new Date(
                year,
                month,
                Math.floor(Math.random() * daysInMonth) + 1
            );
            if (uberDate >= START_DATE && uberDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        uberDate,
                        TRANSPORT[2]
                    )
                );
            }
        }

        // Groceries (4-6 times per month)
        const groceryCount = Math.floor(Math.random() * 3) + 4;
        for (let i = 0; i < groceryCount; i++) {
            const groceryDate = new Date(
                year,
                month,
                Math.floor(Math.random() * daysInMonth) + 1
            );
            if (groceryDate >= START_DATE && groceryDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        groceryDate,
                        pickRandom(GROCERIES)
                    )
                );
            }
        }

        // Dining out (4-8 times per month)
        const diningCount = Math.floor(Math.random() * 5) + 4;
        for (let i = 0; i < diningCount; i++) {
            const diningDate = new Date(
                year,
                month,
                Math.floor(Math.random() * daysInMonth) + 1
            );
            if (diningDate >= START_DATE && diningDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        diningDate,
                        pickRandom(DINING_OUT)
                    )
                );
            }
        }

        // Shopping (2-4 times per month)
        const shopCount = Math.floor(Math.random() * 3) + 2;
        for (let i = 0; i < shopCount; i++) {
            const shopDate = new Date(
                year,
                month,
                Math.floor(Math.random() * daysInMonth) + 1
            );
            if (shopDate >= START_DATE && shopDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        shopDate,
                        pickRandom(SHOPPING)
                    )
                );
            }
        }

        // Transfer to savings (once per month after salary)
        const transferDate = new Date(year, month, 29);
        if (transferDate >= START_DATE && transferDate <= END_DATE) {
            transactions.push(
                createTransaction(
                    CURRENT_ACCOUNT_ID,
                    transferDate,
                    TRANSFERS[0]
                )
            );
        }

        // Occasional gift (every 3-4 months)
        if (Math.random() < 0.25) {
            const giftDate = randomDate(
                new Date(year, month, 1),
                new Date(year, month + 1, 0)
            );
            if (giftDate >= START_DATE && giftDate <= END_DATE) {
                transactions.push(
                    createTransaction(
                        CURRENT_ACCOUNT_ID,
                        giftDate,
                        GIFTS_RECEIVED
                    )
                );
            }
        }

        currentDate.setMonth(currentDate.getMonth() + 1);
    }

    // Sort by date descending
    return transactions.sort((a, b) => {
        const dateA = new Date(a.bookingDate || "");
        const dateB = new Date(b.bookingDate || "");
        return dateB.getTime() - dateA.getTime();
    });
}

function generateSavingsAccountTransactions(): Transaction[] {
    const transactions: Transaction[] = [];
    const currentDate = new Date(START_DATE);

    while (currentDate <= END_DATE) {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();

        // Monthly transfer from current account
        const transferDate = new Date(year, month, 29);
        if (transferDate >= START_DATE && transferDate <= END_DATE) {
            const amount = randomBetween(200, 500);
            const txnId = generateTransactionId();
            transactions.push({
                id: `${SAVINGS_ACCOUNT_ID}_${txnId}`,
                accountId: SAVINGS_ACCOUNT_ID,
                transactionId: txnId,
                bookingDate: formatDate(transferDate),
                valueDate: formatDate(transferDate),
                amount: amount.toFixed(2),
                currency: CURRENCY,
                debtorName: "FROM CURRENT ACCOUNT",
                remittanceInformationUnstructured: "TRANSFER FROM MAIN ACCOUNT",
                isPending: false,
            });
        }

        // Occasional withdrawal (every 4-5 months for larger expenses)
        if (Math.random() < 0.2) {
            const withdrawDate = randomDate(
                new Date(year, month, 1),
                new Date(year, month + 1, 0)
            );
            if (withdrawDate >= START_DATE && withdrawDate <= END_DATE) {
                const amount = randomBetween(300, 800);
                const txnId = generateTransactionId();
                transactions.push({
                    id: `${SAVINGS_ACCOUNT_ID}_${txnId}`,
                    accountId: SAVINGS_ACCOUNT_ID,
                    transactionId: txnId,
                    bookingDate: formatDate(withdrawDate),
                    valueDate: formatDate(withdrawDate),
                    amount: (-amount).toFixed(2),
                    currency: CURRENCY,
                    creditorName: "TO CURRENT ACCOUNT",
                    remittanceInformationUnstructured: "WITHDRAWAL TO MAIN",
                    isPending: false,
                });
            }
        }

        // Quarterly interest payment
        if (month % 3 === 2) {
            const interestDate = new Date(year, month, 28);
            if (interestDate >= START_DATE && interestDate <= END_DATE) {
                const interest = randomBetween(8, 25);
                const txnId = generateTransactionId();
                transactions.push({
                    id: `${SAVINGS_ACCOUNT_ID}_${txnId}`,
                    accountId: SAVINGS_ACCOUNT_ID,
                    transactionId: txnId,
                    bookingDate: formatDate(interestDate),
                    valueDate: formatDate(interestDate),
                    amount: interest.toFixed(2),
                    currency: CURRENCY,
                    debtorName: "STARLING BANK",
                    remittanceInformationUnstructured: "INTEREST PAYMENT",
                    isPending: false,
                });
            }
        }

        currentDate.setMonth(currentDate.getMonth() + 1);
    }

    return transactions.sort((a, b) => {
        const dateA = new Date(a.bookingDate || "");
        const dateB = new Date(b.bookingDate || "");
        return dateB.getTime() - dateA.getTime();
    });
}

function generateISATransactions(): Transaction[] {
    const transactions: Transaction[] = [];
    const currentDate = new Date(START_DATE);

    // ISA typically has less frequent but larger deposits
    while (currentDate <= END_DATE) {
        const year = currentDate.getFullYear();
        const month = currentDate.getMonth();

        // Monthly ISA contribution (every 2-3 months)
        if (month % 2 === 0) {
            const depositDate = new Date(year, month, 15);
            if (depositDate >= START_DATE && depositDate <= END_DATE) {
                const amount = randomBetween(150, 400);
                const txnId = generateTransactionId();
                transactions.push({
                    id: `${ISA_ACCOUNT_ID}_${txnId}`,
                    accountId: ISA_ACCOUNT_ID,
                    transactionId: txnId,
                    bookingDate: formatDate(depositDate),
                    valueDate: formatDate(depositDate),
                    amount: amount.toFixed(2),
                    currency: CURRENCY,
                    debtorName: "BANK TRANSFER",
                    remittanceInformationUnstructured: "ISA DEPOSIT",
                    isPending: false,
                });
            }
        }

        // Annual interest payment in April
        if (month === 3) {
            const interestDate = new Date(year, month, 5);
            if (interestDate >= START_DATE && interestDate <= END_DATE) {
                const interest = randomBetween(35, 80);
                const txnId = generateTransactionId();
                transactions.push({
                    id: `${ISA_ACCOUNT_ID}_${txnId}`,
                    accountId: ISA_ACCOUNT_ID,
                    transactionId: txnId,
                    bookingDate: formatDate(interestDate),
                    valueDate: formatDate(interestDate),
                    amount: interest.toFixed(2),
                    currency: CURRENCY,
                    debtorName: "NATIONWIDE BS",
                    remittanceInformationUnstructured: "ANNUAL INTEREST",
                    isPending: false,
                });
            }
        }

        currentDate.setMonth(currentDate.getMonth() + 1);
    }

    return transactions.sort((a, b) => {
        const dateA = new Date(a.bookingDate || "");
        const dateB = new Date(b.bookingDate || "");
        return dateB.getTime() - dateA.getTime();
    });
}

// ============================================================================
// Generate All Transactions
// ============================================================================

let cachedTransactions: Map<string, Transaction[]> | null = null;

function getAllTransactions(): Map<string, Transaction[]> {
    if (!cachedTransactions) {
        cachedTransactions = new Map([
            [CURRENT_ACCOUNT_ID, generateCurrentAccountTransactions()],
            [SAVINGS_ACCOUNT_ID, generateSavingsAccountTransactions()],
            [ISA_ACCOUNT_ID, generateISATransactions()],
        ]);

        // Calculate running balances
        cachedTransactions.forEach((transactions, accountId) => {
            // Get initial balance based on account type
            let balance =
                accountId === CURRENT_ACCOUNT_ID
                    ? 1500
                    : accountId === SAVINGS_ACCOUNT_ID
                      ? 3000
                      : 2000;

            // Process from oldest to newest
            const sorted = [...transactions].sort((a, b) => {
                const dateA = new Date(a.bookingDate || "");
                const dateB = new Date(b.bookingDate || "");
                return dateA.getTime() - dateB.getTime();
            });

            sorted.forEach((txn) => {
                balance += parseFloat(txn.amount);
                txn.balanceAfterTransaction = Math.round(balance * 100) / 100;
            });
        });
    }

    return cachedTransactions;
}

// ============================================================================
// Balance Calculation
// ============================================================================

function calculateCurrentBalance(accountId: string): number {
    const transactions = getAllTransactions().get(accountId) || [];
    if (transactions.length === 0) return 0;

    // Get the most recent transaction's balance
    const sortedByDate = [...transactions].sort((a, b) => {
        const dateA = new Date(a.bookingDate || "");
        const dateB = new Date(b.bookingDate || "");
        return dateB.getTime() - dateA.getTime();
    });

    return sortedByDate[0].balanceAfterTransaction || 0;
}

export function getMockBalances(accountId: string): Balance[] {
    const currentBalance = calculateCurrentBalance(accountId);

    return [
        {
            balanceType: "interimAvailable",
            amount: currentBalance.toFixed(2),
            currency: CURRENCY,
            referenceDate: formatDate(new Date()),
        },
        {
            balanceType: "closingBooked",
            amount: currentBalance.toFixed(2),
            currency: CURRENCY,
            referenceDate: formatDate(new Date()),
        },
    ];
}

// ============================================================================
// Net Worth History
// ============================================================================

export function getMockNetWorthHistory(): NetWorthHistoryResponse {
    const allTransactions = getAllTransactions();
    const dataPoints: NetWorthDataPoint[] = [];

    // Start with initial balances
    const initialBalances = new Map<string, number>([
        [CURRENT_ACCOUNT_ID, 1500],
        [SAVINGS_ACCOUNT_ID, 3000],
        [ISA_ACCOUNT_ID, 2000],
    ]);

    // Get all unique dates
    const allDates = new Set<string>();
    allTransactions.forEach((transactions) => {
        transactions.forEach((txn) => {
            if (txn.bookingDate) {
                allDates.add(txn.bookingDate);
            }
        });
    });

    // Sort dates and calculate net worth for each
    const sortedDates = Array.from(allDates).sort();
    const balances = new Map(initialBalances);

    sortedDates.forEach((date) => {
        // Apply all transactions for this date
        allTransactions.forEach((transactions, accountId) => {
            const dayTransactions = transactions.filter(
                (t) => t.bookingDate === date
            );
            dayTransactions.forEach((txn) => {
                const current = balances.get(accountId) || 0;
                balances.set(accountId, current + parseFloat(txn.amount));
            });
        });

        // Calculate total net worth
        let totalNetWorth = 0;
        balances.forEach((balance) => {
            totalNetWorth += balance;
        });

        dataPoints.push({
            date: new Date(date).toISOString(),
            amount: Math.round(totalNetWorth * 100) / 100,
        });
    });

    return {
        dataPoints,
        status: "Calculated",
        lastCalculated: new Date().toISOString(),
    };
}

// ============================================================================
// Paginated Transactions
// ============================================================================

export function getMockTransactions(
    accountId: string,
    page: number = 1,
    pageSize: number = 50
): PagedResponse<Transaction> {
    const allAccountTransactions =
        getAllTransactions().get(accountId) || [];
    const totalCount = allAccountTransactions.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;

    const items = allAccountTransactions.slice(startIndex, endIndex);

    return {
        items,
        page,
        pageSize,
        totalCount,
        totalPages,
        hasNextPage: page < totalPages,
        hasPreviousPage: page > 1,
    };
}

// ============================================================================
// Link Institution Response
// ============================================================================

export function getMockLinkInstitutionResponse(
    institutionId: string
): LinkInstitutionResponse {
    // Check if already linked
    const isLinked = mockAccounts.some(
        (acc) => acc.institutionId === institutionId
    );

    if (isLinked) {
        return {
            status: "AlreadyLinked",
            isAlreadyLinked: true,
            existingRequisitionId: mockAccounts.find(
                (acc) => acc.institutionId === institutionId
            )?.requisitionId,
        };
    }

    return {
        authorizationLink: `https://mock-bank-auth.example.com/authorize?institution=${institutionId}`,
        status: "Created",
        isAlreadyLinked: false,
    };
}
