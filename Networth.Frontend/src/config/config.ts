import { useMockData as effectiveMockData } from "./firebaseConfig";

interface Config {
    api: {
        baseUrl: string;
    };
    /** Enable mock API data for development/demo purposes.
     *  Also true when Firebase credentials are missing (graceful fallback). */
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

export const config: Config = {
    api: {
        baseUrl: getEnvVar("VITE_API_URL", "http://localhost:7071"),
    },
    useMockData: effectiveMockData,
};
