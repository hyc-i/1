// @ts-check
const { defineConfig, devices } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './specs',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: 0,
  workers: 1,
  reporter: [['list'], ['html', { outputFolder: 'playwright-report' }]],
  use: {
    baseURL: 'http://localhost:5193',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'msedge',
      use: {
        ...devices['Desktop Edge'],
        channel: 'msedge',
        launchOptions: {
          args: ['--no-sandbox', '--disable-setuid-sandbox'],
        },
      },
    },
  ],
  webServer: {
    command: 'dotnet run --project src/LibrarySeatReservation.Web/LibrarySeatReservation.Web.csproj --urls http://localhost:5193',
    url: 'http://localhost:5193',
    reuseExistingServer: !process.env.CI,
    timeout: 60000,
    cwd: 'D:\\AIWeb\\second-classroom-manager',
  },
});
