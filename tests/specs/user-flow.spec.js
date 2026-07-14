const { test, expect } = require('@playwright/test');

test.describe('用户端主链路烟雾测试', () => {

  test('L1-1+L1-2: 首页加载 + 统计卡片可见', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toContainText('图书馆座位预约系统');
    await expect(page.locator('text=总座位数')).toBeVisible();
    await expect(page.locator('text=空闲座位')).toBeVisible();
    await expect(page.locator('text=已预约')).toBeVisible();
  });

  test('L1-1: 学生A体验登录', async ({ page }) => {
    await page.goto('/');
    await page.click('button[value="学生A"]');
    await expect(page.locator('text=当前身份：学生A')).toBeVisible();
  });

  test('L1-3: 座位列表加载（登录后）', async ({ page }) => {
    await page.goto('/');
    await page.click('button[value="学生A"]');
    await page.click('text=查看座位');
    await expect(page.locator('h2')).toContainText('座位列表');
    const cards = page.locator('.card');
    await expect(cards.first()).toBeVisible();
  });

  test('L1-4: 座位详情/预约页', async ({ page }) => {
    await page.goto('/');
    await page.click('button[value="学生A"]');
    await page.goto('/Seats');
    const reserveBtn = page.locator('a.btn-success:has-text("预约")').first();
    if (await reserveBtn.count() > 0) {
      await reserveBtn.click();
      await expect(page.locator('h2')).toContainText('预约座位');
    }
  });

  test('L1-5+L1-6: 预约提交 + 我的预约', async ({ page }) => {
    await page.goto('/');
    await page.click('button[value="学生B"]');
    await page.goto('/Seats');
    const reserveBtn = page.locator('a.btn-success:has-text("预约")').first();
    if (await reserveBtn.count() > 0) {
      await reserveBtn.click();
      // Use different time slot to avoid conflicts with other test runs
      await page.fill('#StartTime', '13:00');
      await page.fill('#EndTime', '14:00');
      await page.click('button[type="submit"]');
      await expect(page.locator('h2')).toContainText('我的预约', { timeout: 5000 });
    }
  });

  test('L1-7: 取消预约', async ({ page }) => {
    await page.goto('/');
    await page.click('button[value="学生B"]');
    await page.goto('/Reservations/My');

    const cancelBtn = page.locator('button.btn-danger:has-text("取消")').first();
    if (await cancelBtn.count() > 0) {
      await page.on('dialog', dialog => dialog.accept());
      await cancelBtn.click();
      await expect(page.locator('.alert-success')).toContainText('已取消', { timeout: 5000 });
    }
  });

});
