using System.Globalization;
using Microsoft.Data.Sqlite;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Data;

public class SecondClassroomRepository
{
    private readonly DatabaseOptions _options;

    public SecondClassroomRepository(DatabaseOptions options)
    {
        _options = options;
    }

    public DashboardStats GetDashboardStats()
    {
        using var connection = OpenConnection();
        return new DashboardStats
        {
            StudentCount = ScalarInt(connection, "SELECT COUNT(*) FROM Students;"),
            RecordCount = ScalarInt(connection, "SELECT COUNT(*) FROM ActivityRecords;"),
            PendingCount = ScalarInt(connection, "SELECT COUNT(*) FROM ActivityRecords WHERE Status = @Status;", ("@Status", ActivityOptions.PendingStatus)),
            ApprovedCount = ScalarInt(connection, "SELECT COUNT(*) FROM ActivityRecords WHERE Status = @Status;", ("@Status", ActivityOptions.ApprovedStatus)),
            TotalCredits = ScalarDouble(connection, "SELECT COALESCE(SUM(Credits), 0) FROM ActivityRecords WHERE Status = @Status;", ("@Status", ActivityOptions.ApprovedStatus)),
            CategoryStats = GetCategoryStats(connection),
            RecentActivities = GetRecentActivities(connection)
        };
    }

    public List<Student> GetStudents(string? keyword = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT * FROM Students
WHERE @Keyword = ''
   OR StudentNo LIKE @LikeKeyword
   OR Name LIKE @LikeKeyword
   OR College LIKE @LikeKeyword
   OR Major LIKE @LikeKeyword
   OR ClassName LIKE @LikeKeyword
ORDER BY ClassName, StudentNo;";
        var normalizedKeyword = keyword?.Trim() ?? string.Empty;
        Add(command, "@Keyword", normalizedKeyword);
        Add(command, "@LikeKeyword", $"%{normalizedKeyword}%");

        var students = new List<Student>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }

        return students;
    }

    public Student? GetStudent(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Students WHERE Id = @Id;";
        Add(command, "@Id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapStudent(reader) : null;
    }

    public bool StudentNoExists(string studentNo, int? exceptId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = exceptId.HasValue
            ? "SELECT COUNT(*) FROM Students WHERE StudentNo = @StudentNo AND Id <> @Id;"
            : "SELECT COUNT(*) FROM Students WHERE StudentNo = @StudentNo;";
        Add(command, "@StudentNo", studentNo.Trim());
        if (exceptId.HasValue)
        {
            Add(command, "@Id", exceptId.Value);
        }

        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
    }

    public int CreateStudent(Student student)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Students (StudentNo, Name, Gender, College, Major, ClassName, Phone, Email, CreatedAt)
VALUES (@StudentNo, @Name, @Gender, @College, @Major, @ClassName, @Phone, @Email, @CreatedAt);
SELECT last_insert_rowid();";
        AddStudentParameters(command, student);
        Add(command, "@CreatedAt", ToDateTimeString(DateTime.Now));
        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    public void UpdateStudent(Student student)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE Students
SET StudentNo = @StudentNo,
    Name = @Name,
    Gender = @Gender,
    College = @College,
    Major = @Major,
    ClassName = @ClassName,
    Phone = @Phone,
    Email = @Email
WHERE Id = @Id;";
        Add(command, "@Id", student.Id);
        AddStudentParameters(command, student);
        command.ExecuteNonQuery();
    }

    public bool UpsertStudentByNo(Student student)
    {
        var existingId = FindStudentIdByNo(student.StudentNo);
        if (existingId.HasValue)
        {
            student.Id = existingId.Value;
            UpdateStudent(student);
            return false;
        }

        CreateStudent(student);
        return true;
    }

    public void DeleteStudent(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Students WHERE Id = @Id;";
        Add(command, "@Id", id);
        command.ExecuteNonQuery();
    }

    public List<ActivityRecord> GetActivityRecords(string? keyword = null, string? status = null, string? category = null, int? studentId = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT r.*, s.StudentNo, s.Name AS StudentName, s.College, s.ClassName
FROM ActivityRecords r
JOIN Students s ON s.Id = r.StudentId
WHERE (@Keyword = ''
       OR r.ActivityName LIKE @LikeKeyword
       OR r.Organizer LIKE @LikeKeyword
       OR s.StudentNo LIKE @LikeKeyword
       OR s.Name LIKE @LikeKeyword
       OR s.ClassName LIKE @LikeKeyword)
  AND (@Status = '' OR r.Status = @Status)
  AND (@Category = '' OR r.Category = @Category)
  AND (@StudentId = 0 OR r.StudentId = @StudentId)
ORDER BY CASE r.Status WHEN '待审核' THEN 0 WHEN '未通过' THEN 1 ELSE 2 END,
         r.SubmittedAt DESC,
         r.Id DESC;";
        var normalizedKeyword = keyword?.Trim() ?? string.Empty;
        Add(command, "@Keyword", normalizedKeyword);
        Add(command, "@LikeKeyword", $"%{normalizedKeyword}%");
        Add(command, "@Status", status?.Trim() ?? string.Empty);
        Add(command, "@Category", category?.Trim() ?? string.Empty);
        Add(command, "@StudentId", studentId ?? 0);

        var records = new List<ActivityRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            records.Add(MapActivityRecord(reader));
        }

        return records;
    }

    public ActivityRecord? GetActivityRecord(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT r.*, s.StudentNo, s.Name AS StudentName, s.College, s.ClassName
FROM ActivityRecords r
JOIN Students s ON s.Id = r.StudentId
WHERE r.Id = @Id;";
        Add(command, "@Id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapActivityRecord(reader) : null;
    }

    public int CreateActivityRecord(ActivityRecord record)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO ActivityRecords (StudentId, Category, ActivityName, Level, Organizer, StartDate, EndDate, Hours, Description, Evidence, Status, Credits, ReviewOpinion, SubmittedAt)
VALUES (@StudentId, @Category, @ActivityName, @Level, @Organizer, @StartDate, @EndDate, @Hours, @Description, @Evidence, @Status, @Credits, @ReviewOpinion, @SubmittedAt);
SELECT last_insert_rowid();";
        AddActivityParameters(command, record);
        Add(command, "@Status", ActivityOptions.PendingStatus);
        Add(command, "@Credits", 0);
        Add(command, "@ReviewOpinion", string.Empty);
        Add(command, "@SubmittedAt", ToDateTimeString(DateTime.Now));
        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    public void UpdateActivityRecord(ActivityRecord record)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE ActivityRecords
SET StudentId = @StudentId,
    Category = @Category,
    ActivityName = @ActivityName,
    Level = @Level,
    Organizer = @Organizer,
    StartDate = @StartDate,
    EndDate = @EndDate,
    Hours = @Hours,
    Description = @Description,
    Evidence = @Evidence
WHERE Id = @Id;";
        Add(command, "@Id", record.Id);
        AddActivityParameters(command, record);
        command.ExecuteNonQuery();
    }

    public void ReviewActivityRecord(int id, string status, double credits, string reviewOpinion)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE ActivityRecords
SET Status = @Status,
    Credits = @Credits,
    ReviewOpinion = @ReviewOpinion,
    ReviewedAt = @ReviewedAt
WHERE Id = @Id;";
        Add(command, "@Id", id);
        Add(command, "@Status", status);
        Add(command, "@Credits", status == ActivityOptions.ApprovedStatus ? credits : 0);
        Add(command, "@ReviewOpinion", reviewOpinion.Trim());
        Add(command, "@ReviewedAt", ToDateTimeString(DateTime.Now));
        command.ExecuteNonQuery();
    }

    public void DeleteActivityRecord(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM ActivityRecords WHERE Id = @Id;";
        Add(command, "@Id", id);
        command.ExecuteNonQuery();
    }

    public List<StudentSummary> GetStudentSummaries(string? keyword = null)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT * FROM StudentSecondClassroomSummary
WHERE @Keyword = ''
   OR StudentNo LIKE @LikeKeyword
   OR Name LIKE @LikeKeyword
   OR College LIKE @LikeKeyword
   OR Major LIKE @LikeKeyword
   OR ClassName LIKE @LikeKeyword
ORDER BY TotalCredits DESC, PendingRecords DESC, StudentNo;";
        var normalizedKeyword = keyword?.Trim() ?? string.Empty;
        Add(command, "@Keyword", normalizedKeyword);
        Add(command, "@LikeKeyword", $"%{normalizedKeyword}%");

        var summaries = new List<StudentSummary>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            summaries.Add(new StudentSummary
            {
                StudentId = ReadInt(reader, "StudentId"),
                StudentNo = ReadString(reader, "StudentNo"),
                Name = ReadString(reader, "Name"),
                College = ReadString(reader, "College"),
                Major = ReadString(reader, "Major"),
                ClassName = ReadString(reader, "ClassName"),
                TotalRecords = ReadInt(reader, "TotalRecords"),
                PendingRecords = ReadInt(reader, "PendingRecords"),
                ApprovedRecords = ReadInt(reader, "ApprovedRecords"),
                TotalCredits = ReadDouble(reader, "TotalCredits")
            });
        }

        return summaries;
    }

    private int? FindStudentIdByNo(string studentNo)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id FROM Students WHERE StudentNo = @StudentNo;";
        Add(command, "@StudentNo", studentNo.Trim());
        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private List<CategoryStat> GetCategoryStats(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT Category,
       COUNT(*) AS Count,
       COALESCE(SUM(CASE WHEN Status = '已通过' THEN Credits ELSE 0 END), 0) AS Credits
FROM ActivityRecords
GROUP BY Category
ORDER BY Count DESC, Category;";

        var stats = new List<CategoryStat>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            stats.Add(new CategoryStat
            {
                Category = ReadString(reader, "Category"),
                Count = ReadInt(reader, "Count"),
                Credits = ReadDouble(reader, "Credits")
            });
        }

        return stats;
    }

    private List<RecentActivity> GetRecentActivities(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT r.Id, s.Name AS StudentName, r.ActivityName, r.Category, r.Status, r.SubmittedAt
FROM ActivityRecords r
JOIN Students s ON s.Id = r.StudentId
ORDER BY r.SubmittedAt DESC, r.Id DESC
LIMIT 6;";

        var activities = new List<RecentActivity>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            activities.Add(new RecentActivity
            {
                Id = ReadInt(reader, "Id"),
                StudentName = ReadString(reader, "StudentName"),
                ActivityName = ReadString(reader, "ActivityName"),
                Category = ReadString(reader, "Category"),
                Status = ReadString(reader, "Status"),
                SubmittedAt = ReadRequiredDate(reader, "SubmittedAt")
            });
        }

        return activities;
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

    private static int ScalarInt(SqliteConnection connection, string sql, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static double ScalarDouble(SqliteConnection connection, string sql, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? 0 : Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static void AddStudentParameters(SqliteCommand command, Student student)
    {
        Add(command, "@StudentNo", student.StudentNo.Trim());
        Add(command, "@Name", student.Name.Trim());
        Add(command, "@Gender", student.Gender.Trim());
        Add(command, "@College", student.College.Trim());
        Add(command, "@Major", student.Major.Trim());
        Add(command, "@ClassName", student.ClassName.Trim());
        Add(command, "@Phone", student.Phone.Trim());
        Add(command, "@Email", student.Email.Trim());
    }

    private static void AddActivityParameters(SqliteCommand command, ActivityRecord record)
    {
        Add(command, "@StudentId", record.StudentId);
        Add(command, "@Category", record.Category.Trim());
        Add(command, "@ActivityName", record.ActivityName.Trim());
        Add(command, "@Level", record.Level.Trim());
        Add(command, "@Organizer", record.Organizer.Trim());
        Add(command, "@StartDate", ToDateString(record.StartDate));
        Add(command, "@EndDate", ToDateString(record.EndDate));
        Add(command, "@Hours", record.Hours);
        Add(command, "@Description", record.Description.Trim());
        Add(command, "@Evidence", record.Evidence.Trim());
    }

    private static Student MapStudent(SqliteDataReader reader)
    {
        return new Student
        {
            Id = ReadInt(reader, "Id"),
            StudentNo = ReadString(reader, "StudentNo"),
            Name = ReadString(reader, "Name"),
            Gender = ReadString(reader, "Gender"),
            College = ReadString(reader, "College"),
            Major = ReadString(reader, "Major"),
            ClassName = ReadString(reader, "ClassName"),
            Phone = ReadString(reader, "Phone"),
            Email = ReadString(reader, "Email"),
            CreatedAt = ReadRequiredDate(reader, "CreatedAt")
        };
    }

    private static ActivityRecord MapActivityRecord(SqliteDataReader reader)
    {
        return new ActivityRecord
        {
            Id = ReadInt(reader, "Id"),
            StudentId = ReadInt(reader, "StudentId"),
            StudentNo = ReadString(reader, "StudentNo"),
            StudentName = ReadString(reader, "StudentName"),
            College = ReadString(reader, "College"),
            ClassName = ReadString(reader, "ClassName"),
            Category = ReadString(reader, "Category"),
            ActivityName = ReadString(reader, "ActivityName"),
            Level = ReadString(reader, "Level"),
            Organizer = ReadString(reader, "Organizer"),
            StartDate = ReadNullableDate(reader, "StartDate"),
            EndDate = ReadNullableDate(reader, "EndDate"),
            Hours = ReadDouble(reader, "Hours"),
            Description = ReadString(reader, "Description"),
            Evidence = ReadString(reader, "Evidence"),
            Status = ReadString(reader, "Status"),
            Credits = ReadDouble(reader, "Credits"),
            ReviewOpinion = ReadString(reader, "ReviewOpinion"),
            SubmittedAt = ReadRequiredDate(reader, "SubmittedAt"),
            ReviewedAt = ReadNullableDate(reader, "ReviewedAt")
        };
    }

    private static void Add(SqliteCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private static string ReadString(SqliteDataReader reader, string column)
    {
        var value = reader[column];
        return value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
    }

    private static int ReadInt(SqliteDataReader reader, string column)
    {
        var value = reader[column];
        return value == DBNull.Value ? 0 : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static double ReadDouble(SqliteDataReader reader, string column)
    {
        var value = reader[column];
        return value == DBNull.Value ? 0 : Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static DateTime ReadRequiredDate(SqliteDataReader reader, string column)
    {
        return ReadNullableDate(reader, column) ?? DateTime.Now;
    }

    private static DateTime? ReadNullableDate(SqliteDataReader reader, string column)
    {
        var text = ReadString(reader, column);
        return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;
    }

    private static object ToDateString(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : DBNull.Value;
    }

    private static string ToDateTimeString(DateTime value)
    {
        return value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
