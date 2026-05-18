PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Students (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentNo TEXT NOT NULL UNIQUE,
    Name TEXT NOT NULL,
    Gender TEXT NOT NULL DEFAULT '',
    College TEXT NOT NULL DEFAULT '',
    Major TEXT NOT NULL DEFAULT '',
    ClassName TEXT NOT NULL DEFAULT '',
    Phone TEXT NOT NULL DEFAULT '',
    Email TEXT NOT NULL DEFAULT '',
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime'))
);

CREATE TABLE IF NOT EXISTS ActivityRecords (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentId INTEGER NOT NULL,
    Category TEXT NOT NULL,
    ActivityName TEXT NOT NULL,
    Level TEXT NOT NULL DEFAULT '',
    Organizer TEXT NOT NULL DEFAULT '',
    StartDate TEXT NULL,
    EndDate TEXT NULL,
    Hours REAL NOT NULL DEFAULT 0,
    Description TEXT NOT NULL DEFAULT '',
    Evidence TEXT NOT NULL DEFAULT '',
    Status TEXT NOT NULL DEFAULT '待审核',
    Credits REAL NOT NULL DEFAULT 0,
    ReviewOpinion TEXT NOT NULL DEFAULT '',
    SubmittedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
    ReviewedAt TEXT NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Students_StudentNo ON Students(StudentNo);
CREATE INDEX IF NOT EXISTS IX_ActivityRecords_StudentId ON ActivityRecords(StudentId);
CREATE INDEX IF NOT EXISTS IX_ActivityRecords_Status ON ActivityRecords(Status);
CREATE INDEX IF NOT EXISTS IX_ActivityRecords_Category ON ActivityRecords(Category);

CREATE VIEW IF NOT EXISTS StudentSecondClassroomSummary AS
SELECT
    s.Id AS StudentId,
    s.StudentNo,
    s.Name,
    s.College,
    s.Major,
    s.ClassName,
    COUNT(r.Id) AS TotalRecords,
    SUM(CASE WHEN r.Status = '待审核' THEN 1 ELSE 0 END) AS PendingRecords,
    SUM(CASE WHEN r.Status = '已通过' THEN 1 ELSE 0 END) AS ApprovedRecords,
    COALESCE(SUM(CASE WHEN r.Status = '已通过' THEN r.Credits ELSE 0 END), 0) AS TotalCredits
FROM Students s
LEFT JOIN ActivityRecords r ON r.StudentId = s.Id
GROUP BY s.Id, s.StudentNo, s.Name, s.College, s.Major, s.ClassName;

CREATE TRIGGER IF NOT EXISTS trg_activity_reviewed_at
AFTER UPDATE OF Status, Credits, ReviewOpinion ON ActivityRecords
WHEN NEW.Status <> '待审核'
BEGIN
    UPDATE ActivityRecords
    SET ReviewedAt = COALESCE(NEW.ReviewedAt, datetime('now','localtime'))
    WHERE Id = NEW.Id;
END;
