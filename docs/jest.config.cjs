/** @type {import('jest').Config} */
const config = {
    preset: 'jest-preset-angular',
    testEnvironment: 'jsdom',
    resolver: 'jest-preset-angular/build/resolvers/ng-jest-resolver.js',
    setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
    transform: {
        '^.+\\.(ts|mjs|js|html|svg)$': [
            'jest-preset-angular',
            {
                tsconfig: '<rootDir>/tsconfig.spec.json',
                stringifyContentPathRegex: '\\.(html|svg)$'
            }
        ]
    },
    transformIgnorePatterns: ['node_modules/(?!.*\\.mjs$)'],
    moduleFileExtensions: ['ts', 'html', 'js', 'mjs', 'json'],
    extensionsToTreatAsEsm: ['.ts'],
    moduleNameMapper: {
        '\\.(css|scss|sass)$': 'identity-obj-proxy',
        '^src/(.*)$': '<rootDir>/src/$1'
    },
    testMatch: ['<rootDir>/src/**/*.spec.ts'],
    testPathIgnorePatterns: ['/e2e/'],
    testEnvironmentOptions: {
        customExportConditions: ['node', 'node-addons']
    }
};

module.exports = config;
