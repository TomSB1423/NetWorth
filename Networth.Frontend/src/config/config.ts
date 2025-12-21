interface Config {
    api: {
        baseUrl: string;
    };
    auth: {
        clientId: string;
        tenantId: string;
        apiClientId: string;
    };
    /** Enable mock API data for development/demo purposes */
    useMockData: boolean;
}

const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

const getBoolEnvVar = (key: string, defaultValue: boolean = false): boolean => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        return defaultValue;
    }
    return value.toLowerCase() === "true" || value === "1";
};

export const config: Config = {
    api: {
        baseUrl: getEnvVar("VITE_API_URL", "http://localhost:7071"),
    },
    auth: {
        clientId: getEnvVar("VITE_ENTRA_CLIENT_ID", ""),
        tenantId: getEnvVar("VITE_ENTRA_TENANT_ID", ""),
        apiClientId: getEnvVar("VITE_ENTRA_API_CLIENT_ID", ""),
    },
    useMockData: getBoolEnvVar("VITE_USE_MOCK_DATA", false),
};
