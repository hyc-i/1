using System.Globalization;
using Microsoft.Data.Sqlite;

namespace SecondClassroomManager.Data;

public class DatabaseInitializer
{
    private readonly DatabaseOptions _options;
    private readonly IWebHostEnvironment _environment;

    public DatabaseInitializer(DatabaseOptions options, IWebHostEnvironment environment)
    {
        _options = options;
        _environment = environment;
    }

    public void Initialize()
    {
        using var connection = OpenConnection();
        var schemaPath = Path.Combine(_environment.ContentRootPath, "Database", "schema.sql");
        using (var command = connection.CreateCommand())
        {
            command.CommandText = File.ReadAllText(schemaPath);
            command.ExecuteNonQuery();
        }

        if (CountRows(connection, "Students") == 0)
        {
            SeedStudents(connection);
            SeedActivityRecords(connection);
        }

        EnsureGeneratedSampleData(connection);
        RepairActivityRecordDefaults(connection);
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_options.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
        return connection;
    }

    private static long CountRows(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {tableName};";
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    private static long CountGeneratedStudents(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Students WHERE StudentNo LIKE '202405%';";
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    private static long CountGeneratedRecords(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM ActivityRecords WHERE Evidence LIKE 'DEMO-SC-%';";
        return (long)(command.ExecuteScalar() ?? 0L);
    }

    private static void EnsureGeneratedSampleData(SqliteConnection connection)
    {
        if (CountRows(connection, "Students") < 100 && CountGeneratedStudents(connection) == 0)
        {
            SeedGeneratedStudents(connection, 120);
        }

        if (CountRows(connection, "ActivityRecords") < 100 && CountGeneratedRecords(connection) == 0)
        {
            SeedGeneratedActivityRecords(connection, 160);
        }
    }

    private static void RepairActivityRecordDefaults(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE ActivityRecords
SET Credits = CASE
    WHEN Category = '科研与竞赛' THEN 1.5
    WHEN Category = '认证考试' THEN 1.0
    WHEN Category = '评奖评优' THEN 0.8
    WHEN Category = '奖励表彰' THEN 1.5
    WHEN Category = '创新创业' THEN 1.5
    WHEN Category = '志愿服务' THEN 1.0
    ELSE 1.0
END
WHERE Status = '待审核' AND Credits <= 0;

UPDATE ActivityRecords
SET Hours = CASE
    WHEN Category IN ('科研与竞赛', '创新创业') THEN 8
    WHEN Category IN ('社会活动', '志愿服务') THEN 4
    ELSE 2
END
WHERE Hours <= 0;";
        command.ExecuteNonQuery();
    }

    private static void SeedStudents(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Students (StudentNo, Name, Gender, College, Major, ClassName, Phone, Email) VALUES
('2024010101', '林一航', '男', '数据学院', '数据科学与大数据技术', '数科2401', '13800000001', '2024010101@example.edu'),
('2024010102', '周雨晴', '女', '数据学院', '数据科学与大数据技术', '数科2401', '13800000002', '2024010102@example.edu'),
('2024010201', '陈知远', '男', '数据学院', '软件工程', '软工2402', '13800000003', '2024010201@example.edu'),
('2024010202', '许念', '女', '数据学院', '软件工程', '软工2402', '13800000004', '2024010202@example.edu'),
('2024010301', '何沐辰', '男', '数据学院', '人工智能', '智能2403', '13800000005', '2024010301@example.edu');";
        command.ExecuteNonQuery();
    }

    private static void SeedActivityRecords(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '科研与竞赛', '大学生数学建模校赛', '校级', '教务处', '2026-03-12', '2026-03-14', 18, '参与建模、论文撰写与答辩展示。', '获奖证书编号 MCM-2026-0314', '已通过', 1.5, '材料完整，按校级竞赛二等奖认定。', '2026-03-15 09:21:00', '2026-03-16 14:10:00' FROM Students WHERE StudentNo = '2024010101';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '社会活动', '社区数字助老志愿服务', '市级', '青年志愿者协会', '2026-04-05', '2026-04-05', 6, '为社区老人提供手机支付与政务应用辅导。', '志愿服务证明 6 小时', '待审核', 1.0, '', '2026-04-06 11:30:00', NULL FROM Students WHERE StudentNo = '2024010102';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '认证考试', '全国计算机等级考试二级 Python', '国家级', '教育部教育考试院', '2026-03-30', '2026-03-30', 2, '通过全国计算机等级考试二级 Python。', '证书编号 NCRE-PY-260330', '已通过', 1.0, '证书信息可核验。', '2026-04-02 08:15:00', '2026-04-03 10:12:00' FROM Students WHERE StudentNo = '2024010201';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '评奖评优', '学院优秀学生干部', '院级', '数据学院', '2026-05-06', '2026-05-06', 2, '担任班级学习委员，组织学风建设活动。', '学院公示截图', '待审核', 0.8, '', '2026-05-07 17:20:00', NULL FROM Students WHERE StudentNo = '2024010202';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '奖励表彰', '蓝桥杯软件赛省赛三等奖', '省级', '工业和信息化人才交流中心', '2026-04-20', '2026-04-20', 8, '参加程序设计竞赛并获得省赛三等奖。', '获奖证书扫描件', '已通过', 2.0, '按省级竞赛三等奖认定。', '2026-04-25 13:45:00', '2026-04-26 09:40:00' FROM Students WHERE StudentNo = '2024010301';";
        command.ExecuteNonQuery();
    }

    private static void SeedGeneratedStudents(SqliteConnection connection, int count)
    {
        var surnames = new[] { "赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈", "褚", "卫", "蒋", "沈", "韩", "杨", "朱", "秦", "尤", "许", "何", "吕", "施", "张" };
        var givenNames = new[] { "沐阳", "星澜", "知夏", "景行", "若溪", "承宇", "安然", "清越", "思源", "书宁", "亦辰", "嘉禾", "明远", "南乔", "舒桐", "彦舟", "雨棠", "子衿", "云舒", "初尧", "言蹊", "可昕", "予墨", "辰安" };
        var programs = new[]
        {
            new StudentProgram("数据学院", "数据科学与大数据技术", "数科"),
            new StudentProgram("数据学院", "软件工程", "软工"),
            new StudentProgram("数据学院", "人工智能", "智能"),
            new StudentProgram("信息工程学院", "计算机科学与技术", "计科"),
            new StudentProgram("信息工程学院", "网络工程", "网工"),
            new StudentProgram("管理学院", "信息管理与信息系统", "信管"),
            new StudentProgram("管理学院", "电子商务", "电商"),
            new StudentProgram("外国语学院", "商务英语", "商英")
        };

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"
INSERT OR IGNORE INTO Students (StudentNo, Name, Gender, College, Major, ClassName, Phone, Email)
VALUES (@StudentNo, @Name, @Gender, @College, @Major, @ClassName, @Phone, @Email);";

        for (var i = 1; i <= count; i++)
        {
            var program = programs[(i - 1) % programs.Length];
            var name = surnames[(i - 1) % surnames.Length] + givenNames[(i * 5 - 1) % givenNames.Length];
            var classIndex = ((i - 1) / programs.Length) % 4 + 1;
            var studentNo = $"202405{i:0000}";
            var className = $"{program.ClassPrefix}24{classIndex:00}";

            command.Parameters.Clear();
            Add(command, "@StudentNo", studentNo);
            Add(command, "@Name", name);
            Add(command, "@Gender", i % 2 == 0 ? "女" : "男");
            Add(command, "@College", program.College);
            Add(command, "@Major", program.Major);
            Add(command, "@ClassName", className);
            Add(command, "@Phone", $"139{(50000000 + i):00000000}");
            Add(command, "@Email", $"{studentNo}@example.edu");
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static void SeedGeneratedActivityRecords(SqliteConnection connection, int count)
    {
        var students = GetStudentIds(connection);
        if (students.Count == 0)
        {
            return;
        }

        var templates = new[]
        {
            new ActivityTemplate("社会活动", "社区数字助老志愿服务", "市级", "青年志愿者协会", 6, 1.0, "参与社区便民服务，为居民提供数字应用指导并完成志愿服务记录。"),
            new ActivityTemplate("科研与竞赛", "大学生数学建模竞赛", "省级", "教务处", 18, 2.0, "完成问题建模、数据分析、论文撰写和答辩准备。"),
            new ActivityTemplate("评奖评优", "学院优秀学生干部", "院级", "数据学院", 2, 0.8, "承担班级事务组织与学风建设工作，材料经班级和学院公示。"),
            new ActivityTemplate("认证考试", "全国计算机等级考试二级", "国家级", "教育部教育考试院", 2, 1.0, "通过认证考试，证书编号可在线核验。"),
            new ActivityTemplate("奖励表彰", "蓝桥杯软件赛获奖", "省级", "工业和信息化人才交流中心", 8, 2.0, "参加程序设计竞赛并获得省赛奖项。"),
            new ActivityTemplate("志愿服务", "校园迎新志愿服务", "校级", "校团委", 10, 1.2, "协助新生报到、路线引导和材料发放，完成志愿时长登记。"),
            new ActivityTemplate("创新创业", "大学生创新创业训练计划", "校级", "创新创业学院", 16, 1.5, "参与项目申报、原型开发、阶段汇报和结题材料整理。")
        };
        var statuses = new[] { "已通过", "已通过", "已通过", "待审核", "待审核", "未通过" };

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
VALUES (@StudentId, @Category, @ActivityName, @Level, @Organizer, @StartDate, @EndDate, @Hours, @Description, @Evidence, @Status, @Credits, @ReviewOpinion, @SubmittedAt, @ReviewedAt);";

        var startBase = new DateTime(2026, 1, 8);
        for (var i = 1; i <= count; i++)
        {
            var studentId = students[(i - 1) % students.Count];
            var template = templates[(i - 1) % templates.Length];
            var status = statuses[(i - 1) % statuses.Length];
            var startDate = startBase.AddDays((i * 3) % 140);
            var endDate = template.Hours >= 12 ? startDate.AddDays(1) : startDate;
            var credits = status == "未通过" ? 0 : template.Credits + ((i % 3) * 0.2);
            var reviewedAt = status == "待审核" ? null : startDate.AddDays(2).AddHours(10 + i % 6).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            command.Parameters.Clear();
            Add(command, "@StudentId", studentId);
            Add(command, "@Category", template.Category);
            Add(command, "@ActivityName", $"{template.ActivityName}（第{i:000}批）");
            Add(command, "@Level", template.Level);
            Add(command, "@Organizer", template.Organizer);
            Add(command, "@StartDate", startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Add(command, "@EndDate", endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Add(command, "@Hours", template.Hours);
            Add(command, "@Description", template.Description);
            Add(command, "@Evidence", $"DEMO-SC-{i:0000}");
            Add(command, "@Status", status);
            Add(command, "@Credits", credits);
            Add(command, "@ReviewOpinion", BuildReviewOpinion(status, template.Level));
            Add(command, "@SubmittedAt", startDate.AddDays(1).AddHours(8 + i % 9).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            Add(command, "@ReviewedAt", reviewedAt);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static List<int> GetStudentIds(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id FROM Students ORDER BY StudentNo;";
        var ids = new List<int>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ids.Add(reader.GetInt32(0));
        }

        return ids;
    }

    private static string BuildReviewOpinion(string status, string level)
    {
        return status switch
        {
            "已通过" => $"材料完整，按{level}第二课堂项目标准认定。",
            "未通过" => "证明材料不完整，需补充后重新提交。",
            _ => string.Empty
        };
    }

    private static void Add(SqliteCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private record StudentProgram(string College, string Major, string ClassPrefix);

    private record ActivityTemplate(string Category, string ActivityName, string Level, string Organizer, double Hours, double Credits, string Description);
}
