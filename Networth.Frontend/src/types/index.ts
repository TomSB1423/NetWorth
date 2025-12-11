export interface Account {
    id: string;
    institutionId: string;
    name?: string; // Inferred, might be there
    lastSynced?: string;
    // Add other fields as discovered
}

export interface Balance {
    balanceType: string;
    amount: string; // API returns string usually for money
    currency: string;
}

export interface AccountBalances {
    accountId: string;
    balances: Balance[];
}

export interface Institution {
    id: string;
    name: string;
    logo?: string;
}

export interface Transaction {
    transactionId: string;
    bookingDate: string;
    amount: string;
    currency: string;
    description: string;
    remittanceInformationUnstructured?: string;
}

export interface NetWorthDataPoint {
    date: string;
    value: number;
}
