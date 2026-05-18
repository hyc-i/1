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
SELECT Id, '社会活动', '社区数字助老志愿服务', '市级', '青年志愿者协会', '2026-04-05', '2026-04-05', 6, '为社区老人提供手机支付与政务应用辅导。', '志愿服务证明 6 小时', '待审核', 0, '', '2026-04-06 11:30:00', NULL FROM Students WHERE StudentNo = '2024010102';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '认证考试', '全国计算机等级考试二级 Python', '国家级', '教育部教育考试院', '2026-03-30', '2026-03-30', 0, '通过全国计算机等级考试二级 Python。', '证书编号 NCRE-PY-260330', '已通过', 1.0, '证书信息可核验。', '2026-04-02 08:15:00', '2026-04-03 10:12:00' FROM Students WHERE StudentNo = '2024010201';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '评奖评优', '学院优秀学生干部', '院级', '数据学院', '2026-05-06', '2026-05-06', 0, '担任班级学习委员，组织学风建设活动。', '学院公示截图', '待审核', 0, '', '2026-05-07 17:20:00', NULL FROM Students WHERE StudentNo = '2024010202';
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt, ReviewedAt)
SELECT Id, '奖励表彰', '蓝桥杯软件赛省赛三等奖', '省级', '工业和信息化人才交流中心', '2026-04-20', '2026-04-20', 8, '参加程序设计竞赛并获得省赛三等奖。', '获奖证书扫描件', '已通过', 2.0, '按省级竞赛三等奖认定。', '2026-04-25 13:45:00', '2026-04-26 09:40:00' FROM Students WHERE StudentNo = '2024010301';";
        command.ExecuteNonQuery();
    }
}
