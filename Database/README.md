# 数据库初始化说明

本项目包含两个独立项目，数据库初始化方式不同：

---

## 图书馆座位预约系统（主项目）

**位置**：`src/LibrarySeatReservation.Web/`

### 方案
**Code First（EF Core）**。所有表结构由 Entity 类定义，通过迁移自动生成。

### 首次建库建表

应用启动时自动执行（`Program.cs:58-63`）：

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();      // 自动建库建表
    await SeedData.InitializeAsync(app.Services); // 种子数据
}
```

等价于手动命令：

```bash
# 在 src/LibrarySeatReservation.Web 目录下
dotnet ef database update
```

### 种子数据

由 `src/LibrarySeatReservation.Web/DataAccess/SeedData.cs` 自动初始化：

| 数据 | 数量 | 说明 |
|------|------|------|
| 座位 | 40 条 | 一楼大厅 10 个（C-01~C-10）、二楼阅览区 15 个（B-01~B-15）、三楼自习区 15 个（A-01~A-15） |
| 管理员 | 1 条 | 用户名 `admin`，密码 `123456`（BCrypt 哈希） |

### 重新生成数据库

```bash
# 删除数据库
dotnet ef database drop

# 重新创建
dotnet run
```

### 迁移历史

| 迁移名称 | 说明 |
|----------|------|
| `20260707041652_InitialCreate` | 初始迁移，创建 Seats / Reservations / Admins 三张表 + 索引 |
| `20260714091700_FixUniqueIndex` | 修复：为 `IX_Reservation_Seat_Time` 添加 `.IsUnique()` |

### 连接字符串

`src/LibrarySeatReservation.Web/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibrarySeatReservation;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 依赖

- SQL Server LocalDB（随 Visual Studio 安装，或 SSMS）
- 仅支持 Windows

---

## 学生第二课堂登记管理系统（旧项目，保留）

**位置**：项目根目录（`SecondClassroomManager.csproj`）

### 方案
**SQLite 文件数据库**，首次运行自动创建 `App_Data/second_classroom.db`。

### 建表脚本

`Database/schema.sql` 包含完整建表 SQL（Students / ActivityRecords / ReviewLogs + 索引 + 视图 + 触发器）。

### 种子数据

由 `SeedData.cs` 自动初始化 100+ 学生 + 100+ 活动记录。

### 重新生成

```bash
# 关闭程序后删除
del App_Data/second_classroom.db

# 重新运行
dotnet run
```
