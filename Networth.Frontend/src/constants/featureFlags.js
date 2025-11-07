/**
 * Feature flags for the NetWorth application
 * Used to toggle features on/off during development and deployment
 */

export const FEATURE_FLAGS = {
    // Institution service implementation
    USE_MOCK_INSTITUTIONS: true, // Set to false when real service is ready
};

/**
 * Check if a feature is enabled
 * @param {string} feature - Feature flag key
 * @returns {boolean} - Whether the feature is enabled
 */
export const isFeatureEnabled = (feature) => {
    return FEATURE_FLAGS[feature] === true;
};

export default FEATURE_FLAGS;
