/**
 * Mock Implementation of Institutions Service
 * Used when USE_MOCK_INSTITUTIONS feature flag is enabled
 */

import { IInstitutionsService } from './IInstitutionsService';
import {
    MOCK_INSTITUTIONS_DATA,
    MOCK_ACCOUNTS_DATA,
    MOCK_TRANSACTIONS_DATA
} from '../constants/mockData';

/**
 * Mock implementation of the institutions service
 * Simulates API calls with realistic delays and responses
 */
export class MockInstitutionsService extends IInstitutionsService {
    constructor() {
        super();
        this.institutions = [...MOCK_INSTITUTIONS_DATA];
        this.accounts = [...MOCK_ACCOUNTS_DATA];
        this.transactions = [...MOCK_TRANSACTIONS_DATA];
    }

    /**
     * Simulate network delay
     * @param {number} ms - Milliseconds to delay
     * @returns {Promise} Promise that resolves after delay
     */
    _delay(ms = 500) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    /**
     * Get all available institutions
     * @returns {Promise<Institution[]>} List of institutions
     */
    async getInstitutions() {
        await this._delay(300);
        return [...this.institutions];
    }

    /**
     * Get accounts for a specific institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<Account[]>} List of accounts
     */
    async getAccountsByInstitution(institutionId) {
        await this._delay(400);
        return this.accounts.filter(account => account.institutionId === institutionId);
    }

    /**
     * Get all accounts across all institutions
     * @returns {Promise<Account[]>} List of all accounts
     */
    async getAllAccounts() {
        await this._delay(500);
        return [...this.accounts];
    }

    /**
     * Get transactions for a specific account
     * @param {string} accountId - Account identifier
     * @param {Object} options - Query options
     * @param {number} options.limit - Maximum number of transactions to return
     * @param {string} options.fromDate - Start date for transactions (ISO string)
     * @param {string} options.toDate - End date for transactions (ISO string)
     * @returns {Promise<Transaction[]>} List of transactions
     */
    async getTransactions(accountId, options = {}) {
        await this._delay(600);

        const { limit = 50, fromDate, toDate } = options;

        // Find the account to get its name for filtering
        const account = this.accounts.find(acc => acc.id.toString() === accountId.toString());
        if (!account) {
            throw new Error(`Account with ID ${accountId} not found`);
        }

        let filteredTransactions = this.transactions.filter(
            transaction => transaction.account.includes(account.name.split(' ')[0]) // Match by bank name
        );

        // Apply date filtering if provided
        if (fromDate) {
            filteredTransactions = filteredTransactions.filter(
                transaction => new Date(transaction.date) >= new Date(fromDate)
            );
        }

        if (toDate) {
            filteredTransactions = filteredTransactions.filter(
                transaction => new Date(transaction.date) <= new Date(toDate)
            );
        }

        // Sort by date (newest first) and apply limit
        return filteredTransactions
            .sort((a, b) => new Date(b.date) - new Date(a.date))
            .slice(0, limit);
    }

    /**
     * Sync data for a specific institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<boolean>} Success status
     */
    async syncInstitution(institutionId) {
        await this._delay(2000); // Longer delay to simulate sync process

        // Update the last sync time for the institution
        const institutionIndex = this.institutions.findIndex(inst => inst.id === institutionId);
        if (institutionIndex !== -1) {
            this.institutions[institutionIndex].lastSync = 'Just now';

            // Update related accounts' lastUpdated time
            this.accounts.forEach(account => {
                if (account.institutionId === institutionId) {
                    account.lastUpdated = 'Just now';
                    // Simulate small balance changes
                    const changePercent = (Math.random() - 0.5) * 2; // Random change between -1% and +1%
                    account.balance = Math.round(account.balance * (1 + changePercent / 100));
                    account.change = parseFloat(changePercent.toFixed(2));
                }
            });

            return true;
        }

        throw new Error(`Institution with ID ${institutionId} not found`);
    }

    /**
     * Connect a new institution (mock implementation)
     * @param {Object} institutionData - Institution connection data
     * @returns {Promise<Institution>} Connected institution
     */
    async connectInstitution(institutionData) {
        await this._delay(3000); // Longer delay to simulate connection process

        const newInstitution = {
            id: `mock_${Date.now()}`,
            name: institutionData.name || 'New Institution',
            logo: institutionData.logo || '/logos/default.png',
            type: institutionData.type || 'bank',
            isConnected: true,
            lastSync: 'Just now',
            accountCount: 0
        };

        this.institutions.push(newInstitution);
        return newInstitution;
    }

    /**
     * Disconnect an institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<boolean>} Success status
     */
    async disconnectInstitution(institutionId) {
        await this._delay(1000);

        const institutionIndex = this.institutions.findIndex(inst => inst.id === institutionId);
        if (institutionIndex !== -1) {
            this.institutions[institutionIndex].isConnected = false;

            // Remove related accounts
            this.accounts = this.accounts.filter(account => account.institutionId !== institutionId);

            return true;
        }

        throw new Error(`Institution with ID ${institutionId} not found`);
    }

    /**
     * Get net worth calculation (additional utility method)
     * @returns {Promise<Object>} Net worth data
     */
    async getNetWorth() {
        await this._delay(400);

        const totalAssets = this.accounts
            .filter(account => account.balance > 0)
            .reduce((sum, account) => sum + account.balance, 0);

        const totalLiabilities = Math.abs(this.accounts
            .filter(account => account.balance < 0)
            .reduce((sum, account) => sum + account.balance, 0));

        return {
            totalAssets,
            totalLiabilities,
            netWorth: totalAssets - totalLiabilities,
            lastUpdated: new Date().toISOString()
        };
    }
}

export default MockInstitutionsService;
