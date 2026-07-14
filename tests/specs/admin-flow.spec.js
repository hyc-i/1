const { test, expect } = require('@playwright/test');

test.describe('管理端主链路烟雾测试', () => {

  test('L2-1: 管理员登录页加载', async ({ page }) => {
    await page.goto('/Admin/Login');
    await expect(page.locator('h2')).toContainText('管理员登录');
    await expect(page.locator('input[name="Username"]')).toBeVisible();
    await expect(page.locator('input[name="Password"]')).toBeVisible();
  });

  test('L2-2: 管理员登录成功 + 预约管理页', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', '123456');
    await page.click('button[type="submit"]');
    await expect(page).toHaveURL(/\/Admin\/Reservations/);
    await expect(page.locator('h2')).toContainText('预约管理');
  });

  test('管理端座位管理页', async ({ page }) => {
    // Login first
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', '123456');
    await page.click('button[type="submit"]');
    // Navigate to seats
    await page.click('text=座位管理');
    await expect(page.locator('h2')).toContainText('座位管理');
    await expect(page.locator('table tbody tr').first()).toBeVisible();
  });

  test('管理端统计页', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', '123456');
    await page.click('button[type="submit"]');
    await page.click('text=统计');
    await expect(page.locator('h2')).toContainText('数据统计');
  });

  test('管理端登出', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', '123456');
    await page.click('button[type="submit"]');
    // Click logout
    await page.click('text=退出');
    await expect(page).toHaveURL(/\/(Home\/Index)?$/);
    await expect(page.locator('h1')).toContainText('图书馆座位预约系统');
  });

  test('错误密码登录不通过', async ({ page }) => {
    await page.goto('/Admin/Login');
    await page.fill('input[name="Username"]', 'admin');
    await page.fill('input[name="Password"]', 'wrongpassword');
    await page.click('button[type="submit"]');
    await expect(page.locator('text=用户名或密码错误')).toBeVisible();
  });

});
