/**
 * Institutions Service Interface
 * Defines the contract for institution data operations
 */

/**
 * Institution data structure
 * @typedef {Object} Institution
 * @property {string} id - Unique identifier for the institution
 * @property {string} name - Display name of the institution
 * @property {string} logo - Logo URL or path
 * @property {string} type - Type of institution (bank, credit, investment)
 * @property {boolean} isConnected - Whether the institution is connected
 * @property {string} lastSync - Last synchronization time
 * @property {number} accountCount - Number of accounts at this institution
 */

/**
 * Account data structure
 * @typedef {Object} Account
 * @property {string} id - Unique identifier for the account
 * @property {string} name - Display name of the account
 * @property {string} type - Type of account (Checking, Savings, etc.)
 * @property {number} balance - Current balance
 * @property {number} change - Percentage change
 * @property {string} bank - Bank/Institution name
 * @property {string} lastUpdated - Last update time
 * @property {string} institutionId - Associated institution ID
 */

/**
 * Transaction data structure
 * @typedef {Object} Transaction
 * @property {string} id - Unique identifier for the transaction
 * @property {string} date - Transaction date
 * @property {string} description - Transaction description
 * @property {number} amount - Transaction amount (negative for debits)
 * @property {string} category - Transaction category
 * @property {string} account - Account name
 * @property {string} type - Transaction type (credit/debit)
 */

/**
 * Interface for institutions service operations
 */
export class IInstitutionsService {
    /**
     * Get all available institutions
     * @returns {Promise<Institution[]>} List of institutions
     */
    async getInstitutions() {
        throw new Error('getInstitutions method must be implemented');
    }

    /**
     * Get accounts for a specific institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<Account[]>} List of accounts
     */
    async getAccountsByInstitution(institutionId) {
        throw new Error('getAccountsByInstitution method must be implemented');
    }

    /**
     * Get all accounts across all institutions
     * @returns {Promise<Account[]>} List of all accounts
     */
    async getAllAccounts() {
        throw new Error('getAllAccounts method must be implemented');
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
        throw new Error('getTransactions method must be implemented');
    }

    /**
     * Sync data for a specific institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<boolean>} Success status
     */
    async syncInstitution(institutionId) {
        throw new Error('syncInstitution method must be implemented');
    }

    /**
     * Connect a new institution
     * @param {Object} institutionData - Institution connection data
     * @returns {Promise<Institution>} Connected institution
     */
    async connectInstitution(institutionData) {
        throw new Error('connectInstitution method must be implemented');
    }

    /**
     * Disconnect an institution
     * @param {string} institutionId - Institution identifier
     * @returns {Promise<boolean>} Success status
     */
    async disconnectInstitution(institutionId) {
        throw new Error('disconnectInstitution method must be implemented');
    }
}

export default IInstitutionsService;
