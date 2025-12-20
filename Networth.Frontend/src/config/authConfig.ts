import { Configuration, LogLevel, RedirectRequest } from "@azure/msal-browser";

// Environment variables for auth configuration
const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

// Get the instance URL and extract the domain for knownAuthorities
const instance = getEnvVar("VITE_ENTRA_INSTANCE");
const knownAuthority = new URL(instance).hostname;

// MSAL configuration
export const msalConfig: Configuration = {
    auth: {
        clientId: getEnvVar("VITE_ENTRA_CLIENT_ID"),
        authority: `${instance}${getEnvVar("VITE_ENTRA_TENANT_ID")}`,
        redirectUri: window.location.origin + "/",
        postLogoutRedirectUri: window.location.origin + "/",
        // Required for CIAM - tells MSAL to trust this authority
        knownAuthorities: [knownAuthority],
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
export const apiScope = `${getEnvVar("VITE_ENTRA_API_CLIENT_ID")}/.default`;

// Login request configuration
export const loginRequest: RedirectRequest = {
    scopes: [apiScope],
};

// Scopes for acquiring tokens
export const tokenRequest = {
    scopes: [apiScope],
};
