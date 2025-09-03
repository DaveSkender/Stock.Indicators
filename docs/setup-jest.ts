import { setupZoneTestEnv } from 'jest-preset-angular/setup-env/zone';
import { ngMocks } from 'ng-mocks';

// Configure Jest environment
setupZoneTestEnv();

// Configure Jest-DOM matchers
import '@testing-library/jest-dom';

// Configure ng-mocks globally
ngMocks.autoSpy('jest');

// Mock global objects commonly used in browser environment
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: jest.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(), // deprecated
    removeListener: jest.fn(), // deprecated
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    dispatchEvent: jest.fn(),
  })),
});

// Mock window.ResizeObserver
Object.defineProperty(window, 'ResizeObserver', {
  writable: true,
  value: jest.fn().mockImplementation(() => ({
    observe: jest.fn(),
    unobserve: jest.fn(),
    disconnect: jest.fn(),
  })),
});

// Mock console methods to reduce noise in test output
global.console = {
  ...console,
  warn: jest.fn(),
  error: jest.fn(),
};
