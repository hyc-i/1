namespace SecondClassroomManager.Models;

public class StudentSummary
{
    public int StudentId { get; set; }
    public string StudentNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string College { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int PendingRecords { get; set; }
    public int ApprovedRecords { get; set; }
    public double TotalCredits { get; set; }
}
