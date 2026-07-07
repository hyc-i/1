# 图书馆座位预约系统

ASP.NET Core MVC 演示项目 —— 面向图书馆场景的座位预约管理系统。

---

## 项目简介

本系统是一个面向图书馆场景的座位预约管理系统，包含学生端和管理端两条主链路：

- **学生端**：体验登录 → 浏览座位列表/详情 → 提交预约 → 我的预约 → 取消预约
- **管理端**：管理员登录 → 预约管理（筛选/取消） → 统计页

系统使用体验账号机制（无需注册），适合课堂演示。

---

## 技术栈

| 层级 | 技术 | 版本 |
|------|------|------|
| 框架 | ASP.NET Core MVC | .NET 8 |
| ORM | Entity Framework Core | 8.x |
| 数据库 | SQL Server LocalDB | 随 VS 安装 |
| 前端 | Bootstrap 5.3 | CDN |
| 认证 | Cookie Authentication + Session | 内置 |
| 构建 | NuGet | — |

---

## 目录结构

### 当前已存在（本阶段产物）

```
SeatReservation/
├── README.md                      ← 项目说明（本文件，Sprint 0 初稿）
├── docs/
│   ├── 01-项目立项单.md            ← 需求分析阶段文档
│   ├── 02-需求分析与用户故事.md
│   ├── 03-PRD-Lite.md
│   ├── 04-页面树与业务流程.md
│   ├── 05-页面卡与UI规范.md
│   ├── 06-静态原型与原型评审.md
│   ├── 06-静态原型与原型评审-审计.md
│   ├── 07-系统设计说明.md
│   ├── 08-数据库设计.md
│   ├── 08-数据库设计-审计.md
│   ├── 09-关键链路详细设计.md
│   ├── 09-关键链路详细设计-审计.md
│   ├── 10-开发准备与Sprint0.md    ← 本阶段产物
│   └── 项目任务板与迭代记录.md    ← 本阶段产物
├── prototype/
│   ├── static-v1/                 ← 静态原型（v1）
│   └── static-v2/                 ← 静态原型（v2，审计修复版）
└── review-1/
    └── 原型评审清单.md
```

### 后续计划 / 待生成项

```
SeatReservation/
├── SeatReservation.sln             ← Sprint 0: dotnet new sln
├── SeatReservation/                ← Sprint 0: dotnet new mvc
│   ├── Program.cs                  ← Sprint 0: 应用入口
│   ├── appsettings.json            ← Sprint 0: 连接字符串
│   ├── appsettings.Development.json
│   ├── Controllers/
│   │   ├── HomeController.cs       ← Sprint 1
│   │   ├── SeatsController.cs      ← Sprint 1
│   │   ├── ReservationsController.cs ← Sprint 2
│   │   └── AdminController.cs      ← Sprint 3
│   ├── Models/Entities/
│   │   ├── Seat.cs                 ← Sprint 1
│   │   ├── Reservation.cs          ← Sprint 1
│   │   └── Admin.cs                ← Sprint 1
│   ├── Models/ViewModels/
│   │   ├── HomeIndexViewModel.cs   ← Sprint 1
│   │   ├── SeatListViewModel.cs    ← Sprint 1
│   │   ├── SeatDetailViewModel.cs  ← Sprint 1
│   │   ├── ReservationCreateViewModel.cs ← Sprint 2
│   │   ├── ReservationListViewModel.cs   ← Sprint 2
│   │   ├── AdminLoginViewModel.cs  ← Sprint 3
│   │   ├── AdminReservationsViewModel.cs ← Sprint 3
│   │   ├── AdminSeatsViewModel.cs  ← Sprint 4
│   │   └── AdminStatisticsViewModel.cs   ← Sprint 4
│   ├── Services/
│   │   ├── ISeatService.cs         ← Sprint 1
│   │   ├── SeatService.cs          ← Sprint 1
│   │   ├── IReservationService.cs  ← Sprint 2
│   │   ├── ReservationService.cs   ← Sprint 2
│   │   ├── IAdminService.cs        ← Sprint 3
│   │   ├── AdminService.cs         ← Sprint 3
│   │   ├── IStudentContext.cs      ← Sprint 1
│   │   └── StudentContext.cs       ← Sprint 1
│   ├── DataAccess/
│   │   ├── AppDbContext.cs         ← Sprint 1
│   │   └── SeedData.cs             ← Sprint 1
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml      ← Sprint 4
│   │   │   └── Error.cshtml        ← Sprint 5
│   │   ├── Home/
│   │   │   └── Index.cshtml        ← Sprint 1
│   │   ├── Seats/
│   │   │   ├── Index.cshtml        ← Sprint 1
│   │   │   └── Details.cshtml      ← Sprint 1
│   │   ├── Reservations/
│   │   │   ├── Create.cshtml       ← Sprint 2
│   │   │   └── My.cshtml           ← Sprint 2
│   │   └── Admin/
│   │       ├── Login.cshtml        ← Sprint 3
│   │       ├── Reservations.cshtml ← Sprint 3
│   │       ├── Seats.cshtml        ← Sprint 4
│   │       └── Statistics.cshtml   ← Sprint 4
│   └── wwwroot/
│       ├── css/
│       │   └── site.css            ← Sprint 5
│       └── js/
│           └── site.js             ← Sprint 5
```

---

## 运行前提

### 环境要求

| 依赖 | 最低版本 | 验证命令 |
|------|----------|----------|
| .NET SDK | 8.0 | `dotnet --version` |
| SQL Server LocalDB | 随 VS 安装 | `SqlLocalDB info` |
| 浏览器 | 支持 ES6 | — |

### 首次运行

```bash
# 1. 进入项目目录
cd SeatReservation/SeatReservation

# 2. 还原 NuGet 包
dotnet restore

# 3. 应用数据库迁移（自动建表）
dotnet ef database update

# 4. 启动
dotnet run

# 5. 浏览器打开
# http://localhost:5000
```

> **注意**：以上步骤中的 `.sln`、`.csproj`、`appsettings.json`、迁移文件等为后续 Sprint 产物。首次运行前需先完成 **Sprint 0**（项目初始化）和 **Sprint 1**（Entity + DbContext + SeedData）。

---

## 当前阶段

**开发准备与 Sprint 0**（设计文档阶段 → 代码开发过渡期）

- ✅ 需求分析与 PRD 完成
- ✅ 页面树与业务流程完成
- ✅ 静态原型（v1 + v2）通过审计
- ✅ 系统设计与数据库设计完成
- ✅ 关键链路详细设计完成并通过审计
- 🔄 开发准备与 Sprint 0 进行中
- ⬜ Sprint 1~6（代码实现）

详细进度见 `docs/项目任务板与迭代记录.md`。

---

## 已实现范围

> 此章节在代码开发阶段逐步填充。

| 功能 | Sprint | 状态 |
|------|--------|------|
| — | — | 待开发 |

---

## 数据库初始化方式

系统使用 EF Core Code-First + 种子数据：

1. 首次运行时 `SeedData.cs` 自动检查数据库是否已初始化
2. 如未初始化，写入：30~50 个座位（3 个区域）、1 个管理员账号（密码哈希）、1~2 条演示预约记录
3. 无需手动执行 SQL

详细种子数据计划见 `docs/08-数据库设计.md` 第 10.3 节。

---

## 演示账号

### 学生端（体验账号）

| 显示名 | 标识符 |
|--------|--------|
| 学生 A | student_a |
| 学生 B | student_b |
| 学生 C | student_c |

学生无需密码，点击首页按钮即可体验登录。

### 管理端

| 用户名 | 密码 |
|--------|------|
| admin | 123456 |

---

## 已知限制

> 此章节随开发推进持续更新。

- V1.0 不做自动过期/签到功能
- 预约时长不做强制限制（演示场景信任用户）
- 同一学生可同时预约不同座位（不限制一人一座）
- 跨天预约不支持（如 22:00~02:00）
- 无数据导出功能
- 无操作日志审计

---

## 设计文档索引

| 阶段 | 文档 | 审计 |
|------|------|------|
| 需求分析 | `docs/01-项目立项单.md` ~ `docs/05-页面卡与UI规范.md` | — |
| 原型设计 | `docs/06-静态原型与原型评审.md` | `docs/06-静态原型与原型评审-审计.md` |
| 系统设计 | `docs/07-系统设计说明.md` | — |
| 数据库设计 | `docs/08-数据库设计.md` | `docs/08-数据库设计-审计.md` |
| 链路设计 | `docs/09-关键链路详细设计.md` | `docs/09-关键链路详细设计-审计.md` |
| 开发准备 | `docs/10-开发准备与Sprint0.md` | — |
| 任务跟踪 | `docs/项目任务板与迭代记录.md` | — |
