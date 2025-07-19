// jest.polyfills.js - Environment polyfills for Jest testing

// Prevent window property redefinition errors
const originalDefineProperty = Object.defineProperty;
Object.defineProperty = function(obj, prop, descriptor) {
  if (obj === global && prop === 'window') {
    return obj;
  }
  return originalDefineProperty.call(this, obj, prop, descriptor);
};

// Polyfill for global objects that might be missing in test environment
global.window = global.window || {};
global.document = global.document || {};

// Mock Alert for React Native
global.alert = jest.fn();

// Mock fetch if not available
if (!global.fetch) {
  global.fetch = jest.fn(() =>
    Promise.resolve({
      ok: true,
      json: () => Promise.resolve({}),
      text: () => Promise.resolve(''),
    })
  );
}

// Mock console.warn for tests to reduce noise
const originalWarn = console.warn;
console.warn = (...args) => {
  // Filter out specific warnings that are expected in test environment
  const message = args[0];
  if (
    typeof message === 'string' &&
    (message.includes('componentWillReceiveProps') ||
     message.includes('componentWillMount') ||
     message.includes('Unknown action type'))
  ) {
    return;
  }
  originalWarn.apply(console, args);
};