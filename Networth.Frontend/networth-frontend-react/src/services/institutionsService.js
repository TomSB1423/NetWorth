/**
 * Institutions Service Factory
 * Returns the appropriate service implementation based on feature flags
 */

import { isFeatureEnabled } from '../constants/featureFlags';
import { MockInstitutionsService } from './MockInstitutionsService';
// import { RealInstitutionsService } from './RealInstitutionsService'; // To be implemented later

/**
 * Factory function to get the appropriate institutions service
 * @returns {IInstitutionsService} Service instance
 */
export const createInstitutionsService = () => {
    if (isFeatureEnabled('USE_MOCK_INSTITUTIONS')) {
        console.info('🔧 Using Mock Institutions Service');
        return new MockInstitutionsService();
    } else {
        console.info('🌐 Using Real Institutions Service');
        // return new RealInstitutionsService(); // To be implemented later
        throw new Error('Real institutions service not yet implemented. Enable USE_MOCK_INSTITUTIONS feature flag.');
    }
};

// Singleton instance
let institutionsServiceInstance = null;

/**
 * Get the institutions service instance (singleton)
 * @returns {IInstitutionsService} Service instance
 */
export const getInstitutionsService = () => {
    if (!institutionsServiceInstance) {
        institutionsServiceInstance = createInstitutionsService();
    }
    return institutionsServiceInstance;
};

/**
 * Reset the service instance (useful for testing)
 */
export const resetInstitutionsService = () => {
    institutionsServiceInstance = null;
};

export default getInstitutionsService;
