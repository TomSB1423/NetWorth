import { Configuration, LogLevel, RedirectRequest } from "@azure/msal-browser";

// Check if we're in mock mode - if so, auth config is not needed
const useMockData = import.meta.env.VITE_USE_MOCK_DATA === "true" || 
                    import.meta.env.VITE_USE_MOCK_DATA === "1";

// Environment variables for auth configuration
const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

// Only load auth config if not in mock mode
let instance = "";
let knownAuthority = "";

if (!useMockData) {
    // Get the instance URL and extract the domain for knownAuthorities
    instance = getEnvVar("VITE_ENTRA_INSTANCE");
    knownAuthority = new URL(instance).hostname;
}

// MSAL configuration - only used in non-mock mode
export const msalConfig: Configuration = {
    auth: {
        clientId: useMockData ? "mock-client-id" : getEnvVar("VITE_ENTRA_CLIENT_ID"),
        authority: useMockData ? "https://mock.auth.com/" : `${instance}${getEnvVar("VITE_ENTRA_TENANT_ID")}`,
        redirectUri: window.location.origin + "/",
        postLogoutRedirectUri: window.location.origin + "/",
        // Required for CIAM - tells MSAL to trust this authority
        knownAuthorities: useMockData ? [] : [knownAuthority],
    },
    cache: {
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: false,
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) return;
                switch (level) {
                    case LogLevel.Error:
                        console.error(message);
                        return;
                    case LogLevel.Warning:
                        console.warn(message);
                        return;
                    case LogLevel.Info:
                        console.info(message);
                        return;
                    case LogLevel.Verbose:
                        console.debug(message);
                        return;
                }
            },
            logLevel: LogLevel.Warning,
        },
    },
};

// API scope for the backend
// In CIAM, we use the API's client_id with .default scope since custom scopes are not supported
// This requests all permissions granted to the SPA for this API
export const apiScope = useMockData 
    ? "mock-api-scope" 
    : `${getEnvVar("VITE_ENTRA_API_CLIENT_ID")}/.default`;

// Login request configuration
export const loginRequest: RedirectRequest = {
    scopes: [apiScope],
};

// Scopes for acquiring tokens
export const tokenRequest = {
    scopes: [apiScope],
};
