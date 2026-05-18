namespace SecondClassroomManager.Models;

public class DashboardStats
{
    public int StudentCount { get; set; }
    public int RecordCount { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedCount { get; set; }
    public double TotalCredits { get; set; }
    public IReadOnlyList<CategoryStat> CategoryStats { get; set; } = Array.Empty<CategoryStat>();
    public IReadOnlyList<RecentActivity> RecentActivities { get; set; } = Array.Empty<RecentActivity>();
}

public class CategoryStat
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Credits { get; set; }
}

public class RecentActivity
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}
