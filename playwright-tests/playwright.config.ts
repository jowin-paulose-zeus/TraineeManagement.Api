import { defineConfig } from '@playwright/test';
import dotenv from "dotenv";
/**
 * Read environment variables from file.
 * https://github.com/motdotla/dotenv
 */

dotenv.config();

export default defineConfig({
    testDir: './tests',

    fullyParallel: true,

    workers: 10,

    timeout: 30_000,

    expect: {
        timeout: 5_000
    },

    reporter: [
        ['list'],
        ['html', { outputFolder: 'playwright-report', open: 'never' }]
    ],

    use: {
        baseURL: 'http://localhost:5000',

        extraHTTPHeaders: {
            Accept: 'application/json'
        }
    }
});