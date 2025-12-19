interface Config {
    api: {
        baseUrl: string;
    };
    auth: {
        clientId: string;
        tenantId: string;
        apiClientId: string;
    };
}

const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

export const config: Config = {
    api: {
        baseUrl: getEnvVar("VITE_API_URL"),
    },
    auth: {
        clientId: getEnvVar("VITE_ENTRA_CLIENT_ID"),
        tenantId: getEnvVar("VITE_ENTRA_TENANT_ID"),
        apiClientId: getEnvVar("VITE_ENTRA_API_CLIENT_ID"),
    },
};
