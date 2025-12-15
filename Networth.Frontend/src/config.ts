interface Config {
    api: {
        baseUrl: string;
    };
}

const getEnvVar = (key: string): string => {
    const value = import.meta.env[key];
    if (value === undefined || value === "") {
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

export const config: Config = {
    api: {
        baseUrl: getEnvVar("VITE_API_URL"),
    },
};
