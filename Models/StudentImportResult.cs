namespace SecondClassroomManager.Models;

public class StudentImportResult
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public List<string> Messages { get; } = new();
}
