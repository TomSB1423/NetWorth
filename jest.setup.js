// jest.setup.js - Expo and React Native Testing Setup

// Mock expo-router
jest.mock('expo-router', () => ({
    router: {
        push: jest.fn(),
        replace: jest.fn(),
        back: jest.fn(),
        canGoBack: jest.fn(() => true),
    },
    useLocalSearchParams: jest.fn(() => ({})),
    useRouter: jest.fn(() => ({
        push: jest.fn(),
        replace: jest.fn(),
        back: jest.fn(),
        canGoBack: jest.fn(() => true),
    })),
    Redirect: ({ href }) => null,
    Link: ({ children, href, ...props }) => children,
    usePathname: jest.fn(() => '/'),
    useSegments: jest.fn(() => []),
}));

// Mock Expo modules
jest.mock('expo-constants', () => ({
    default: {
        statusBarHeight: 20,
        deviceName: 'Test Device',
    },
}));

// Mock react-native-svg
jest.mock('react-native-svg', () => ({
    Svg: 'Svg',
    Circle: 'Circle',
    Path: 'Path',
    Line: 'Line',
    Text: 'SvgText',
}));

// Mock AsyncStorage
jest.mock('@react-native-async-storage/async-storage', () =>
    require('@react-native-async-storage/async-storage/jest/async-storage-mock')
);

// Global test timeout
jest.setTimeout(10000);

// Mock Dimensions for responsive components
import { Dimensions } from 'react-native';
jest.spyOn(Dimensions, 'get').mockReturnValue({
    width: 375,
    height: 812,
    scale: 2,
    fontScale: 1,
});
