/**
 * Category for classifying bank accounts.
 * Must match backend Networth.Domain.Enums.AccountCategory
 */
export type AccountCategory = "Spending" | "Savings" | "Investment" | "Credit";

/**
 * Represents a user's bank account.
 * Maps to backend Networth.Domain.Entities.UserAccount
 */
export interface Account {
    /** Unique identifier for the account */
    id: string;
    /** ID of the user who owns this account */
    userId: string;
    /** GoCardless requisition ID that created this account */
    requisitionId: string;
    /** Institution metadata ID */
    institutionId: string;
    /** Name of the financial institution */
    institutionName?: string;
    /** URL to the institution's logo */
    institutionLogo?: string;
    /** Bank-provided account name */
    name: string;
    /** User-defined display name */
    displayName?: string;
    /** User-specified category for the account */
    category?: AccountCategory;
    /** Account IBAN */
    iban?: string;
    /** Currency code (e.g., "GBP", "EUR") */
    currency: string;
    /** Bank product name/type */
    product?: string;
    /** ISO 8601 timestamp of last sync */
    lastSynced?: string;
}

/**
 * Represents an account balance at a point in time.
 * Maps to backend Networth.Domain.Entities.AccountBalance
 */
export interface Balance {
    /** Type of balance (e.g., "interimAvailable", "expected", "closingBooked") */
    balanceType: string;
    /** Balance amount as a decimal string */
    amount: string;
    /** Currency code */
    currency: string;
    /** Whether credit limit is included in this balance */
    creditLimitIncluded?: boolean;
    /** ISO 8601 date reference for this balance */
    referenceDate?: string;
}

/**
 * Container for account balances, used in frontend context.
 * Groups balances with their parent account ID.
 */
export interface AccountBalances {
    /** The account these balances belong to */
    accountId: string;
    /** List of balance records for this account */
    balances: Balance[];
}

/**
 * Represents a financial institution (bank).
 * Maps to backend Networth.Domain.Entities.InstitutionMetadata
 */
export interface Institution {
    /** Unique identifier for the institution */
    id: string;
    /** Name of the bank */
    name: string;
    /** URL to the institution's logo */
    logoUrl?: string;
}

/**
 * Represents a financial transaction.
 * Maps to backend Networth.Domain.Entities.Transaction
 */
export interface Transaction {
    /** Composite identifier (accountId_transactionId) */
    id: string;
    /** Account ID this transaction belongs to */
    accountId: string;
    /** Bank-provided transaction identifier */
    transactionId?: string;
    /** ISO 8601 date when transaction was booked */
    bookingDate?: string;
    /** ISO 8601 date when value is applied */
    valueDate?: string;
    /** Transaction amount as decimal string (negative for outgoing) */
    amount: string;
    /** Currency code */
    currency: string;
    /** Name of the creditor (recipient) */
    creditorName?: string;
    /** Name of the debtor (payer) */
    debtorName?: string;
    /** Creditor's account IBAN */
    creditorAccount?: string;
    /** Debtor's account IBAN */
    debtorAccount?: string;
    /** Transaction description/reference */
    remittanceInformationUnstructured?: string;
    /** Bank transaction code */
    bankTransactionCode?: string;
    /** Proprietary bank transaction code */
    proprietaryBankTransactionCode?: string;
    /** End-to-end identification */
    endToEndId?: string;
    /** Mandate identifier for direct debits */
    mandateId?: string;
    /** Creditor identifier */
    creditorId?: string;
    /** Ultimate creditor name */
    ultimateCreditor?: string;
    /** Ultimate debtor name */
    ultimateDebtor?: string;
    /** Purpose code for the transaction */
    purposeCode?: string;
    /** Additional transaction information */
    additionalInformation?: string;
    /** Running balance after this transaction */
    balanceAfterTransaction?: number;
    /** Whether this transaction is pending */
    isPending: boolean;
}

/**
 * Generic paginated response wrapper.
 * Maps to backend Networth.Application.Models.PagedResult
 */
export interface PagedResponse<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

/**
 * A single data point in net worth history.
 * Maps to backend Networth.Domain.Entities.NetWorthPoint
 */
export interface NetWorthDataPoint {
    /** ISO 8601 date string */
    date: string;
    /** Net worth amount on this date */
    amount: number;
}

/**
 * Status of net worth calculation.
 * Maps to backend Networth.Domain.Enums.NetWorthCalculationStatus
 */
export type NetWorthCalculationStatus =
    | "NotCalculated"
    | "Calculating"
    | "Calculated";

/**
 * Response containing net worth history and calculation status.
 */
export interface NetWorthHistoryResponse {
    dataPoints: NetWorthDataPoint[];
    status: NetWorthCalculationStatus;
    lastCalculated: string | null;
}

/**
 * Represents a user of the application.
 * Maps to backend user representation in API responses
 */
export interface User {
    /** Unique user identifier */
    userId: string;
    /** User's display name */
    name: string;
    /** Whether this is a newly created user */
    isNewUser: boolean;
    /** Whether user has completed onboarding */
    hasCompletedOnboarding: boolean;
}

/**
 * Response from institution linking endpoint.
 */
export interface LinkInstitutionResponse {
    /** URL to redirect user for bank authorization */
    authorizationLink?: string;
    /** Status of the linking request */
    status: string;
    /** Whether this institution is already linked */
    isAlreadyLinked: boolean;
    /** Existing requisition ID if already linked */
    existingRequisitionId?: string;
}
