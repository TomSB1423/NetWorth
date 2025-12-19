export type AccountCategory = "Spending" | "Savings" | "Investment";

export interface Account {
    id: string;
    userId: string;
    requisitionId: string;
    institutionId: string;
    institutionName?: string;
    institutionLogo?: string;
    name: string;
    displayName?: string;
    category?: AccountCategory;
    iban?: string;
    currency: string;
    product?: string;
    lastSynced?: string;
}

export interface Balance {
    balanceType: string;
    amount: string;
    currency: string;
    creditLimitIncluded?: boolean;
    referenceDate?: string;
}

export interface AccountBalances {
    accountId: string;
    balances: Balance[];
}

export interface Institution {
    id: string;
    name: string;
    logoUrl?: string;
}

export interface Transaction {
    id: string;
    accountId: string;
    transactionId?: string;
    bookingDate?: string;
    valueDate?: string;
    amount: string;
    currency: string;
    creditorName?: string;
    debtorName?: string;
    creditorAccount?: string;
    debtorAccount?: string;
    remittanceInformationUnstructured?: string;
    bankTransactionCode?: string;
    proprietaryBankTransactionCode?: string;
    endToEndId?: string;
    mandateId?: string;
    creditorId?: string;
    ultimateCreditor?: string;
    ultimateDebtor?: string;
    purposeCode?: string;
    additionalInformation?: string;
    balanceAfterTransaction?: number;
    isPending: boolean;
}

export interface PagedResponse<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}

export interface NetWorthDataPoint {
    date: string;
    amount: number;
}

export interface User {
    userId: string;
    name: string;
    isNewUser: boolean;
    hasCompletedOnboarding: boolean;
}

export interface LinkInstitutionResponse {
    authorizationLink?: string;
    status: string;
    isAlreadyLinked: boolean;
    existingRequisitionId?: string;
}
