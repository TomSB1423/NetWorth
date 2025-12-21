import { initializeApp, FirebaseApp } from "firebase/app";
import { getAuth, Auth } from "firebase/auth";

// Check if we're in mock mode - if so, Firebase config is not needed
const useMockData =
    import.meta.env.VITE_USE_MOCK_DATA === "true" ||
    import.meta.env.VITE_USE_MOCK_DATA === "1";

// Environment variables for Firebase configuration
const getEnvVar = (key: string, defaultValue?: string): string => {
    const value = import.meta.env[key] as string | undefined;
    if (value === undefined || value === "") {
        if (defaultValue !== undefined) return defaultValue;
        throw new Error(`Environment variable ${key} is not defined`);
    }
    return value;
};

// Firebase configuration - only used in non-mock mode
const firebaseConfig = useMockData
    ? {
          apiKey: "mock-api-key",
          authDomain: "mock.firebaseapp.com",
          projectId: "mock-project",
      }
    : {
          apiKey: getEnvVar("VITE_FIREBASE_API_KEY"),
          authDomain: getEnvVar("VITE_FIREBASE_AUTH_DOMAIN"),
          projectId: getEnvVar("VITE_FIREBASE_PROJECT_ID"),
      };

// Initialize Firebase app
let app: FirebaseApp | null = null;
let auth: Auth | null = null;

if (!useMockData) {
    app = initializeApp(firebaseConfig);
    auth = getAuth(app);
}

export { app, auth, firebaseConfig, useMockData };
