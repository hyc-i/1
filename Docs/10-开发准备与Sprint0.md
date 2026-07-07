# 开发准备与 Sprint 0

---

## 1. 仓库结构

### 1.1 物理路径

```
second-classroom-manager/          ← 父级目录（原项目保留）
├── docs/                          ← 全部设计文档
│   ├── 01-项目立项单.md
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
│   ├── 10-开发准备与Sprint0.md           ← 本文件
│   └── 项目任务板与迭代记录.md
├── prototype/                      ← 静态原型
│   ├── static-v1/
│   └── static-v2/
├── review-1/
│   └── 原型评审清单.md
└── SeatReservation/                ← 代码根目录（Sprint 0 创建）
    ├── SeatReservation.sln
    ├── SeatReservation/
    │   ├── ...
    └── README.md
```

### 1.2 远程仓库

**当前状态**：❌ 尚未推送（待补远端推送）

**建议地址格式**：
```
git@github.com:<用户名>/seat-reservation.git
```

**后续操作**：Sprint 0 完成后将本地仓库推送到远端。

---

## 2. 分支策略

### 2.1 分支模型

```
main             ← 稳定版本，只从 dev 合并
  └── dev        ← 开发主分支，日常开发基于此
       ├── feat/infra          ← Sprint 0-1: 基础设施
       ├── feat/student-chain  ← Sprint 2: 学生端闭环
       ├── feat/admin-chain    ← Sprint 3: 管理端
       ├── feat/admin-ext      ← Sprint 4: 统计+座位管理
       ├── feat/ui-polish      ← Sprint 5: 视图完善
       └── feat/final-test     ← Sprint 6: 端到端测试
```

### 2.2 分支规则

| 分支 | 来源 | 合并到 | 命名约定 |
|------|------|--------|----------|
| `main` | — | — | 只读，保护分支 |
| `dev` | `main` | `main`（Sprint 结束） | 开发主线 |
| `feat/*` | `dev` | `dev`（功能完成） | `feat/<英文短名>` |
| `fix/*` | `dev` | `dev`（修复完成） | `fix/<问题简述>` |

### 2.3 单人开发简化建议

单人开发时可直接在 `dev` 分支开发，每完成一个可验收功能后 commit 到 `dev`，Sprint 结束时合并到 `main`。`feat/*` 分支仅建议在有明确功能边界时使用（如 Sprint 2 预约闭环）。

---

## 3. 提交规范

### 3.1 提交信息格式

```
<类型>: <简短描述>

<可选详细说明>
```

### 3.2 类型前缀

| 类型 | 适用场景 | 示例 |
|------|----------|------|
| `feat` | 新增功能 | `feat: 实现预约冲突检测 Service 方法` |
| `fix` | Bug 修复 | `fix: 修复预约提交表单日期校验为空的异常` |
| `docs` | 文档变更 | `docs: 补充 README 演示账号章节` |
| `refactor` | 重构 | `refactor: 抽取座位状态计算为独立方法` |
| `chore` | 构建/配置 | `chore: 添加 EF Core SqlServer NuGet 包` |
| `style` | 格式调整 | `style: 统一 Controller 中空行风格` |

### 3.3 提交颗粒度

- 每个可独立验证的功能点一次 commit
- 不要在一次 commit 中混入多个无关变更
- 示例：`SeatService` 和 `SeatsController` 虽属同一 Sprint，但应分别提交

---

## 4. Sprint 0 目标

### 4.1 Sprint 0 定义

Sprint 0 是"开发准备冲刺"，目的是建立可运行的项目骨架，但不包含任何业务功能代码。

### 4.2 Sprint 0 任务清单

| # | 任务 | 产出 | 验收标准 |
|---|------|------|----------|
| T00-01 | 创建解决方案 `.sln` | `SeatReservation.sln` | `dotnet build` 通过 |
| T00-02 | 创建 Web 项目 `.csproj` | ASP.NET Core MVC 项目 | `dotnet run` 打开浏览器看到默认页面 |
| T00-03 | 安装 NuGet 包 | `packages.config` / 项目依赖 | `dotnet build` 无缺失依赖 |
| T00-04 | 配置 `appsettings.json` | 连接字符串指向 LocalDB | 配置可读 |
| T00-05 | 创建项目目录结构 | Controllers/、Models/、Services/、DataAccess/、Views/、wwwroot/ | 目录存在 |
| T00-06 | 创建 Entity 类 | `Seat.cs`, `Reservation.cs`, `Admin.cs` | 字段与 08-4 一致 |
| T00-07 | 创建 `AppDbContext.cs` | `OnModelCreating` 配置 | `dotnet ef migrations add InitialCreate` 成功 |
| T00-08 | 创建首次迁移并建库建表 | 迁移文件 + LocalDB 数据库 | `dotnet ef database update` 成功，SSMS 可查 3 张表 |
| T00-09 | 实现 `SeedData.cs` 最小化 | 初始化时写入座位和管理员 | 启动后数据库中有 30~50 条座位 + 1 条管理员 |
| T00-10 | 初始化 Git 仓库并首次提交 | `.gitignore` + 首次 commit | `git log` 看到首次提交 |
| T00-11 | [可选] 推送远端 | 远端仓库同步 | `git push -u origin main` 成功 |

### 4.3 Sprint 0 完成标准

- ✅ `dotnet build` 通过
- ✅ `dotnet run` 可启动，浏览器可访问
- ✅ 数据库通过迁移成功创建，3 张表均存在
- ✅ 种子数据写入成功
- ✅ Git 本地仓库已初始化并提交

---

## 5. Sprint 1-4 主 Sprint 粗计划

### 5.1 概览

```
Sprint 0 ─── 开发准备（本阶段）
  │
  ├── Sprint 1 ─── 基础设施 + 学生端浏览路线
  │     （多轮推进）
  │
  ├── Sprint 2 ─── 学生端预约闭环
  │     （多轮推进）
  │
  ├── Sprint 3 ─── 管理端预约管理
  │     （多轮推进）
  │
  └── Sprint 4 ─── 管理端统计 + 视图完善
        （多轮推进）
```

### 5.2 Sprint 1：基础设施 + 学生端浏览路线

| 属性 | 值 |
|------|-----|
| 目标 | 学生可登录、浏览首页、查看座位列表和详情 |
| 阶段最低完成线 | `HomeController.Index` 显示体验按钮 + `SeatsController` 显示座椅列表 |
| 关联功能 | F1 体验登录, F2 空闲座位概览, F3 座位列表+详情 |
| 关联文档 | 07-7.1 第 1~3 步, 09-L1-1~L1-4 |
| 预估文件数 | ~15 个（Entity 3 + ViewModel 3 + Service 2 + Controller 2 + View 3 + DataAccess 2） |
| 任务卡编号 | T10-01 ~ T10-06 |

**本轮可推进轮次数**：3 轮
- 第 1 轮：Entity + DbContext + SeedData（建表）
- 第 2 轮：StudentContext + HomeController（登录）
- 第 3 轮：SeatService + SeatsController（浏览）

### 5.3 Sprint 2：学生端预约闭环

| 属性 | 值 |
|------|-----|
| 目标 | 学生端完整闭环：选座→预约→查看→取消 |
| 阶段最低完成线 | `ReservationsController.Create` + `My` + `Cancel` 可用 |
| 关联功能 | F4 预约提交+冲突检测, F5 我的预约, F6 取消预约 |
| 关联文档 | 07-7.1 第 4~6 步, 09-L1-5~L1-7 |
| 预估文件数 | ~6 个（Service 2 + Controller 1 + ViewModel 2 + View 2） |
| 任务卡编号 | T20-01 ~ T20-05 |

**本轮可推进轮次数**：2 轮
- 第 1 轮：CreateReservationAsync + Create（GET+POST）
- 第 2 轮：My + CancelMyReservationAsync + Cancel

### 5.4 Sprint 3：管理端预约管理

| 属性 | 值 |
|------|-----|
| 目标 | 管理员可登录、管理预约（查看+筛选+取消） |
| 阶段最低完成线 | `AdminController.Login` + `Reservations` + `Cancel` 可用 |
| 关联功能 | F7 管理员登录, F8 预约管理+筛选+取消 |
| 关联文档 | 07-7.2, 09-L2-1~L2-2 |
| 预估文件数 | ~6 个（Service 2 + Controller 1 + ViewModel 2 + View 2） |
| 任务卡编号 | T30-01 ~ T30-05 |

**本轮可推进轮次数**：2 轮
- 第 1 轮：AdminService + AdminController.Login
- 第 2 轮：AdminController.Reservations + Cancel

### 5.5 Sprint 4：管理端统计 + 管理端补充

| 属性 | 值 |
|------|-----|
| 目标 | 管理端完整闭环 + 双导航 + 视图美化 |
| 阶段最低完成线 | `AdminController.Statistics` + `_Layout.cshtml` 双导航 |
| 关联功能 | F9 统计页, 座位管理 CRUD（可选P1）, 双导航 |
| 关联文档 | 07-7.2 第 3 步, 09-L2-3, 07-4.7 |
| 预估文件数 | ~6 个（Service 1 + Controller 1 + ViewModel 2 + View 2 + Layout 1） |
| 任务卡编号 | T40-01 ~ T40-05 |

**本轮可推进轮次数**：2 轮
- 第 1 轮：AdminService.GetStatisticsAsync + Statistics View
- 第 2 轮：Seat CRUD + _Layout 双导航 + 权限拦截

### 5.6 Sprint 5-6 预留

Sprint 5（边界 + 视图完善）和 Sprint 6（端到端测试 + 迭代优化）的详细计划见 `09-关键链路详细设计.md` 第 10.2 节，具体任务卡在进入该阶段时补充。

---

## 6. 里程碑节点

### 6.1 里程碑定义

| 里程碑 | 对应 Sprint | 完成标志 | 预计日期 |
|--------|-----------|----------|----------|
| **M1: 项目骨架可用** | Sprint 0 | `dotnet run` 启动 + 建库建表成功 | TBD（当前阶段） |
| **M2: 学生端闭环** | Sprint 1-2 | 学生可完成完整选座→预约→查看→取消流程 | TBD |
| **M3: 管理端闭环** | Sprint 3-4 | 管理员可完成登录→管理→统计流程 | TBD |
| **M4: 可交付** | Sprint 5-6 | 边界场景覆盖 + 端到端测试通过 | TBD |

### 6.2 里程碑评审节点

每个里程碑完成后执行：
1. 功能验收：按主链路逐步骤人工测试
2. 边界检查：按 09 文档边界清单逐条验证
3. 代码审计：检查分层是否越界（Controller 中不应出现业务逻辑）
4. 文档更新：README 中"已实现范围"章节同步更新

---

## 7. 默认补足项 / 当前假设

### 7.1 前序文档已明确的内容

| 内容 | 来源 | 说明 |
|------|------|------|
| 38 个文件的目录结构 | 07-3 | 直接作为 Sprint 0 目录创建清单 |
| NuGet 包清单 | 07-2.1 | 仅需 2 个包（EF Core SqlServer + Tools） |
| Entity 字段 | 08-4 | 与代码实体类字段完全一致 |
| SeedData 种子数据计划 | 08-10.3 | 3 区域 30~50 座位 + 1 管理员 + 可选演示预约 |
| 6 个 Sprint 实现顺序 | 09-10.2 | Sprint 1~6 均以主链路步骤为依据 |
| 可验证标准 | 09-10.3 | 每个 Sprint 的验收命令/操作 |

### 7.2 本次补足的内容

| # | 补足项 | 位置 | 理由 |
|---|--------|------|------|
| 1 | 分支策略 | 第 2 节 | 前序文档未定义，为代码开发建立基本分支约定 |
| 2 | 提交规范 | 第 3 节 | 同上，建立一致 commit 风格 |
| 3 | Sprint 0 任务分解（11 项） | 4.2 | 前序文档未细化开发准备阶段 |
| 4 | 里程碑 4 个节点 | 第 6 节 | 前序文档未定义可验收里程碑 |
| 5 | 里程碑评审节点 | 6.2 | 确保每个里程碑有明确的验收步骤 |
| 6 | Sprint 1-4 的"多轮推进"说明 | 5.2~5.5 | 避免误以为每个 Sprint 只能一轮完成 |
| 7 | 单人开发简化建议 | 2.3 | 针对本项目单人场景的分支策略调整 |
| 8 | 远端推送状态标记 | 1.2 | 当前无远端地址时标记待补 |

### 7.3 当前假设

| 假设 | 说明 |
|------|------|
| 开发环境为 Windows + Visual Studio 或 VS Code | LocalDB 依赖 Windows 平台 |
| 已安装 .NET 8 SDK | `dotnet` CLI 可用 |
| SQL Server LocalDB 已安装 | 随 Visual Studio 安装，默认可用 |
| 单人开发 | 分支策略可简化，无需 PR 流程 |
| 项目最终在 localhost 演示 | 无需部署和 CI/CD 配置 |
| 文档与代码在同一仓库 | 设计文档与源码不分离，便于追溯 |
