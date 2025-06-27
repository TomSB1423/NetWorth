// jest.setup.js - Simplified setup without React Native dependencies

// Mock expo-router
jest.mock('expo-router', () => ({
    router: {
        push: jest.fn(),
        replace: jest.fn(),
        back: jest.fn(),
        canGoBack: jest.fn(() => true),
    },
    useLocalSearchParams: jest.fn(() => ({})),
}));

// Global test timeout
jest.setTimeout(10000);
