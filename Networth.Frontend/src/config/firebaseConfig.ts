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

/**
 * Check whether Firebase credentials are available.
 * When VITE_USE_MOCK_DATA is false but credentials are missing,
 * we fall back to mock mode with a console warning instead of crashing.
 */
function hasFirebaseCredentials(): boolean {
    const apiKey = import.meta.env.VITE_FIREBASE_API_KEY as string | undefined;
    const authDomain = import.meta.env.VITE_FIREBASE_AUTH_DOMAIN as
        | string
        | undefined;
    const projectId = import.meta.env.VITE_FIREBASE_PROJECT_ID as
        | string
        | undefined;
    return !!(apiKey && authDomain && projectId);
}

const firebaseMissing = !useMockData && !hasFirebaseCredentials();

if (firebaseMissing) {
    console.warn(
        "%c[Auth] Firebase credentials missing (VITE_FIREBASE_API_KEY, VITE_FIREBASE_AUTH_DOMAIN, VITE_FIREBASE_PROJECT_ID). " +
            "Falling back to mock authentication. Set VITE_USE_MOCK_DATA=true or provide Firebase credentials.",
        "color: #f59e0b; font-weight: bold",
    );
}

/** Effective mock mode: explicit opt-in OR forced by missing credentials. */
const effectiveUseMockData = useMockData || firebaseMissing;

// Firebase configuration - only used in non-mock mode
const firebaseConfig = effectiveUseMockData
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

if (!effectiveUseMockData) {
    app = initializeApp(firebaseConfig);
    auth = getAuth(app);
}

export { app, auth, firebaseConfig, effectiveUseMockData as useMockData };
