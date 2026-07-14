# 图书馆座位预约系统 (LibrarySeatReservation)

一个基于 **ASP.NET Core MVC + EF Core + SQL Server** 的图书馆座位预约系统，支持学生端 7 步预约闭环和管理端完整管理。

> **当前阶段**：最终交付（阶段 17），仓库已打 `v1.0-final` 标签。
> **关联文档**：[docs/17-交付说明与项目复盘.md](docs/17-交付说明与项目复盘.md) · [docs/16-联调测试与缺陷闭环.md](docs/16-联调测试与缺陷闭环.md)

---

## 已实现功能（用户端）

| 功能 | 状态 | 说明 |
|------|------|------|
| 体验账号登录 | ✅ 已实现 | 3 个固定学生身份（A/B/C），Session 管理 |
| 首页统计 | ✅ 已实现 | 总座位数 / 空闲座位数 / 已预约数（实时） |
| 座位列表 | ✅ 已实现 | 区域下拉筛选 + 座位卡片 + 实时状态标签 |
| 座位详情 | ✅ 已实现 | 座位信息 + 楼层/描述 + 当前状态 |
| 预约提交 | ✅ 已实现 | 选择日期/时段 + 冲突检测 + 并发防护 |
| 我的预约 | ✅ 已实现 | 预约记录列表 + 状态标签 |
| 取消预约 | ✅ 已实现 | 归属校验 + 幂等处理 + TempData 消息 |

## 已实现功能（管理端）

| 功能 | 状态 | 说明 |
|------|------|------|
| 管理员登录 | ✅ 已实现 | Cookie Authentication + BCrypt 密码验证，ClaimTypes.Role 写入 |
| 预约管理 | ✅ 已实现 | 全部预约列表（含学生名） + 状态下拉筛选 + 取消操作 |
| 统计页 | ✅ 已实现 | 4 个数字卡片（总座位/今日预约/利用率/热门区域） |
| 座位管理 | ✅ 已实现 | 全部座位列表（含已停用） + 新增 Modal + 编辑 Modal + 软删除 |
| 管理端专属布局 | ✅ 已实现 | 顶部导航 + 左侧 220px 侧边栏 + 当前页高亮 |
| 双导航 | ✅ 已实现 | 学生导航 + 管理端入口 |

## 未实现功能

| 功能 | 说明 |
|------|------|
| 404 / 错误页 | 未实现自定义友好错误页 |
| 空状态细化 | 各页面的空状态/加载态优化 |
| 原型样式适配（用户端） | 按 docs/05 UI 规范调整 |

---

## 建议演示路径

```
1. 启动：dotnet run --project src/LibrarySeatReservation.Web
2. 首页：打开 http://localhost:5193 → 看到统计卡片 + 体验按钮
3. 登录：点击"学生A" → 页面刷新为已登录态
4. 选座：点击"查看座位" → 区域筛选下拉 → 查看空闲座位
5. 预约：点击空闲座位"预约" → 选择明天日期 + 时段 → 提交
6. 回看：预约成功后自动跳转"我的预约" → 看到新记录
7. 取消：点击预约中记录右侧"取消"按钮 → 状态变为"已取消"
8. 管理端：访问 /Admin/Login → 输入 admin / 123456
9. 预约管理：查看全部预约 → 状态下拉筛选 → 取消操作
10. 统计：点击"统计" → 查看 4 个数字卡片
11. 座位管理：点击"座位管理" → 新增/编辑/停用座位
```

---

## 技术栈

| 类型 | 技术 |
| --- | --- |
| 后端框架 | ASP.NET Core MVC |
| 目标框架 | .NET 10.0 |
| 编程语言 | C# 13, Razor |
| 数据库 | SQL Server LocalDB |
| ORM | Entity Framework Core 10 |
| 密码哈希 | BCrypt.Net-Next |
| 前端样式 | Bootstrap 5 |
| 认证 | Cookie Authentication（管理端）+ Session（学生端） |
| 测试 | Playwright（msedge 通道）+ Node.js 脚本 |

## 项目结构

```
second-classroom-manager/
├── src/LibrarySeatReservation.Web/    ← 主项目：图书馆座位预约系统
│   ├── Controllers/                   ← 4 个 Controller
│   ├── Models/Entities/               ← 3 个 Entity 类
│   ├── Models/ViewModels/             ← 12 个 ViewModel 类
│   ├── Services/                      ← 4 个 Service 接口+实现
│   ├── DataAccess/                    ← DbContext + SeedData + Migrations
│   ├── Views/                         ← 12 个 Razor 视图
│   ├── wwwroot/                       ← 静态资源
│   ├── Program.cs                     ← 应用入口
│   └── appsettings.json               ← 配置（连接字符串等）
├── tests/                             ← 自动化测试
│   ├── specs/                         ← Playwright 测试用例（12 个）
│   ├── playwright.config.js           ← Playwright 配置（msedge 通道）
│   ├── smoke-test.mjs                 ← 脚本烟雾测试（9 端点）
│   └── package.json                   ← npm 依赖
├── Database/                          ← 数据库说明
│   ├── README.md                      ← 数据库初始化说明
│   └── schema.sql                     ← 旧项目 SQLite 建表脚本
├── Docs/                              ← 设计文档与开发记录
├── prototype/                         ← 静态原型 HTML
├── LibrarySeatReservation.slnx        ← 解决方案文件
└── README.md                          ← 本文件
```

---

## 自动化测试

### Playwright 自动点击烟雾测试

基于 `@playwright/test` + 系统自带 `Microsoft Edge`（`channel: 'msedge'`），覆盖用户端 6 个测试 + 管理端 6 个测试。

```bash
cd tests
npm install
npx playwright test --config=playwright.config.js
```

### 脚本烟雾测试

基于 Node.js HTTP 请求，覆盖 9 个关键端点。

```bash
cd tests
node smoke-test.mjs
```

### 测试结果

| 测试类型 | 用例数 | 通过率 | 执行时间 |
|----------|--------|--------|---------|
| Playwright 自动点击 | 12 | 100%（12/12）| ~35s |
| 脚本烟雾测试 | 9 | 100%（9/9）| ~2s |

---

## 运行要求

- Windows 10/11（LocalDB 依赖 Windows）
- .NET 10 SDK 或更高版本
- SQL Server LocalDB（随 Visual Studio 安装，或 SSMS）
- Edge / Chrome 浏览器

## 快速启动

```bash
# 编译
dotnet build src/LibrarySeatReservation.Web/LibrarySeatReservation.Web.csproj

# 运行（自动建库 + 种子数据）
dotnet run --project src/LibrarySeatReservation.Web

# 浏览器访问
http://localhost:5193
```

## 登录账号

| 角色 | 账号 | 密码/操作 |
|------|------|-----------|
| 学生 A | 点击"学生A"按钮 | 无需密码，体验登录 |
| 学生 B | 点击"学生B"按钮 | 无需密码，体验登录 |
| 学生 C | 点击"学生C"按钮 | 无需密码，体验登录 |
| 管理员 | `admin` | `123456` |

## 数据库初始化

应用启动时自动执行 `db.Database.MigrateAsync()` + `SeedData.InitializeAsync()`，无需手动建库。详见 [Database/README.md](Database/README.md)。

## 当前已知限制

| 限制 | 说明 | 计划处理 |
|------|------|----------|
| 无页面分页 | 数据量 < 100 条时不需分页 | 数据增长后再实现 |
| 预约时长/次数不限 | 演示场景信任用户 | 后续评估 |
| 无自动过期 | 需定时任务 | 已排除在 MVP 外 |
| 用户端视图样式未对齐原型 | 当前使用 Bootstrap 默认样式 | 后续优化 |
| 仅支持 LocalDB | 切换 SQL Server 需改连接串 | 按需 |
| 仅验证了 msedge 通道 | Chrome/Firefox 未单独测试 | 按需 |

---

# 学生第二课堂登记管理系统（旧项目，保留）

一个基于 **ASP.NET Core MVC + SQLite** 的《.NET网络开发技术》课程设计项目，与上方的座位预约系统为独立项目，保留在此仓库中供参考。

**运行方式**：`dotnet run`（项目根目录），首次运行自动创建 `App_Data/second_classroom.db`，端口 5000。

**登录账号**：管理员 `admin` / `123456`；学生 `2024010101` / `13800000001`。

详见 [Database/README.md](Database/README.md) 及仓库中的旧项目源码与文档。
