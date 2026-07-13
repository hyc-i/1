# 图书馆座位预约系统 (LibrarySeatReservation)

一个基于 **ASP.NET Core MVC + EF Core + SQL Server** 的图书馆座位预约系统，支持学生端 7 步预约闭环和管理端完整管理。

> **当前阶段**：用户端主链路已完成（Sprint 0~4），进入边界与错误处理阶段（Sprint 5）。
> **关联文档**：[docs/13-用户端主链路开发记录.md](docs/13-用户端主链路开发记录.md)

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
| 管理员登录 | ✅ 已实现 | Cookie Authentication + BCrypt 密码验证 |
| 预约管理 | ✅ 已实现 | 全部预约列表 + 状态下拉筛选 + 取消操作 |
| 统计页 | ✅ 已实现 | 4 个数字卡片（总座位/今日预约/利用率/热门区域） |
| 座位管理 | ✅ 已实现（列表） | 全部座位列表（含已停用） |
| 双导航 | ✅ 已实现 | 学生导航 + 管理端入口 |

## 待完成功能

| 功能 | 计划 Sprint | 说明 |
|------|-------------|------|
| 座位 CRUD（新增/编辑/删除） | Sprint 5 | 管理端座位管理完整操作 |
| 404 / 错误页 | Sprint 5 | 友好错误页面处理 |
| 空状态细化 | Sprint 5 | 各页面的空状态/加载态 |
| 端到端测试 | Sprint 6 | 按 docs/09-8.2 逐场景手工测试 |
| 原型样式适配 | Sprint 5 | 按 docs/05 UI 规范调整 |

---

## 建议演示路径

```
1. 启动：dotnet run --project src/LibrarySeatReservation.Web
2. 首页：打开 http://localhost:5000 → 看到统计卡片 + 体验按钮
3. 登录：点击"学生A" → 页面刷新为已登录态
4. 选座：点击"查看座位" → 区域筛选下拉 → 查看空闲座位
5. 预约：点击空闲座位"预约" → 选择明天日期 + 时段 → 提交
6. 回看：预约成功后自动跳转"我的预约" → 看到新记录
7. 取消：点击预约中记录右侧"取消"按钮 → 状态变为"已取消"
8. 管理端：访问 /Admin/Login → 输入 admin / 123456
9. 预约管理：查看全部预约 → 状态下拉筛选 → 取消操作
10. 统计：点击"统计" → 查看 4 个数字卡片
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

## 项目结构

```
second-classroom-manager/
├── src/
│   └── LibrarySeatReservation.Web/   ← 新项目（座位预约）
│       ├── Controllers/
│       ├── Models/Entities/
│       ├── Models/ViewModels/
│       ├── Services/
│       ├── DataAccess/
│       ├── Views/
│       ├── Migrations/
│       └── wwwroot/
├── Docs/                              ← 设计文档与开发记录
├── prototype/                         ← 静态原型
├── SecondClassroomManager.csproj      ← 旧项目（保留）
└── LibrarySeatReservation.slnx        ← 新解决方案文件
```

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
http://localhost:5000
```

## 登录账号

| 角色 | 账号 | 密码/操作 |
|------|------|-----------|
| 学生 A | 点击"学生A"按钮 | 无需密码，体验登录 |
| 学生 B | 点击"学生B"按钮 | 无需密码，体验登录 |
| 学生 C | 点击"学生C"按钮 | 无需密码，体验登录 |
| 管理员 | `admin` | `123456` |

## 当前已知限制

| 限制 | 说明 | 计划处理 |
|------|------|----------|
| 无页面分页 | 数据量 < 100 条时不需分页 | 数据增长后再实现 |
| 预约时长/次数不限 | 演示场景信任用户 | Sprint 6 评估是否需要 |
| 无自动过期 | 需定时任务 | 已排除在 MVP 外 |
| 座位 CRUD 新增/编辑/删除待完整 | 管理端座位管理仅列表可用 | Sprint 5 |
| 原型样式未完全适配 | 当前使用 Bootstrap 默认样式 | Sprint 5 |
| 仅支持 LocalDB | 切换 SQL Server 需改连接串 | 按需 |

---

---

# 学生第二课堂登记管理系统（旧项目，保留）

以下为仓库中原有的第二课堂管理系统，与上方的座位预约系统为独立项目，保留在此仓库中供参考。

一个基于 **ASP.NET Core MVC + SQLite** 的《.NET网络开发技术》课程设计项目，用于管理学生第二课堂档案、活动登记、审核授分和管理员查询统计。

## 旧项目技术栈

| 类型 | 技术 |
| --- | --- |
| 后端框架 | ASP.NET Core MVC |
| 目标框架 | .NET 6.0 |
| 编程语言 | C#、Razor、HTML、CSS |
| 数据库 | SQLite |
| 数据访问 | ADO.NET / Microsoft.Data.Sqlite |
| 前端样式 | Bootstrap + 自定义现代化 CSS |

## 运行环境

- Windows 10/11、Linux 或 macOS
- .NET SDK 6.0 或更高版本
- Edge、Chrome 或 Firefox 浏览器

## 快速开始

进入项目目录：

```bash
cd "D:\Document\Course\C#\大作业\SecondClassroomManager"
```

如果在 WSL 中运行当前 D 盘项目，可以使用：

```bash
cd "/mnt/d/Document/Course/C#/大作业/SecondClassroomManager"
"/mnt/c/Program Files/dotnet/dotnet.exe" run --urls "http://127.0.0.1:5080"
```

然后打开：

```text
http://127.0.0.1:5080
```

还原依赖：

```bash
dotnet restore
```

编译项目：

```bash
dotnet build
```

启动系统：

```bash
dotnet run
```

浏览器打开终端输出的地址，例如：

```text
http://localhost:5000
https://localhost:5001
```

## 登录账号

- **管理员端**：账号 `admin`，密码 `123456`。
- **学生端**：使用学生档案中的学号和手机号登录，演示账号 `2024010101`，密码 `13800000001`。
- 学生登录页会默认填入演示账号，便于直接进入学生个人中心展示功能。

## 实用增强功能

- **达标状态**：管理员汇总页和学生个人中心会显示累计学分、达标状态和距离目标学分。
- **申请学分**：学生提交待审核活动时可填写申请学分，管理员审核时再认定最终学分。
- **活动时长校验**：新建和编辑申请时要求活动时长大于 0，演示数据启动时也会自动修复 0 小时记录。
- **审核历史**：活动详情页展示每次审核的状态变化、学分变化、审核意见和审核时间。
- **CSV 导出**：管理员汇总页可导出学生汇总，活动记录页可按关键词、状态和类别导出当前结果，并对 Excel 公式风险字段进行转义。
- **首页统计**：首页增加达标人数、接近达标人数、未达标人数、待审核占比和学院累计学分排行。

## 默认数据

系统首次运行会自动创建数据库：

```text
App_Data/second_classroom.db
```

数据库会自动包含：

- 100+ 名学生样例数据
- 100+ 条第二课堂活动登记样例数据
- 已通过、待审核、未通过等多种审核状态
- 待审核记录带有申请学分，不会以 0 学分进入审核
- 活动记录带有大于 0 的活动时长
- 社会活动、科研竞赛、评奖评优、认证考试、奖励表彰等多种类别

如果想重新生成初始数据，可以关闭程序后删除：

```text
App_Data/second_classroom.db
```

然后重新执行：

```bash
dotnet run
```

## 学生导入格式

支持 `.csv` 或 `.txt` 文件，字段顺序如下：

```text
学号,姓名,性别,学院,专业,班级,电话,邮箱
2024010106,王星河,男,数据学院,软件工程,软工2402,13800000006,2024010106@example.edu
```

## 功能截图

### 系统总览

![系统总览](Docs/screenshots/01-首页.png)

### 学生基本信息管理

![学生基本信息管理](Docs/screenshots/02-学生档案.png)

### 第二课堂登记与审核

![第二课堂登记与审核](Docs/screenshots/03-活动审核.png)

### 管理员查询

![管理员查询](Docs/screenshots/04-管理员查询.png)

## 课程设计提交内容

项目已包含课程设计要求中的主要交付物：

- 完整 ASP.NET Core MVC 源码
- SQLite 数据库文件
- 数据库建表脚本
- 软件配置与使用说明
- 功能模块截图
- 课程设计报告文档

相关文档位于：

```text
Docs/
```

## 说明

本项目用于课程设计学习与展示。提交前请根据个人情况补充课程设计报告封面中的专业班级、学号、姓名等信息。
