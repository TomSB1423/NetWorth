import { Configuration, LogLevel, PopupRequest } from "@azure/msal-browser";

// Environment variables for auth configuration
const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

// MSAL configuration
export const msalConfig: Configuration = {
    auth: {
        clientId: getEnvVar("VITE_ENTRA_CLIENT_ID"),
        authority: `https://login.microsoftonline.com/${getEnvVar(
            "VITE_ENTRA_TENANT_ID"
        )}`,
        redirectUri: window.location.origin + "/",
        postLogoutRedirectUri: window.location.origin + "/",
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
export const apiScope = `api://${getEnvVar(
    "VITE_ENTRA_API_CLIENT_ID"
)}/user_impersonation`;

// Login request configuration
export const loginRequest: PopupRequest = {
    scopes: [apiScope],
};

// Scopes for acquiring tokens
export const tokenRequest = {
    scopes: [apiScope],
};
