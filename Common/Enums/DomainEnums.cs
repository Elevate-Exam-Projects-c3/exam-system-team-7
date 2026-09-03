namespace exam_system.Common.Enums;

public enum UserRole
{
    Student = 1,
    Admin = 2
}

public enum AccountStatus
{
    Pending = 1,
    Active = 2,
    Locked = 3,
    Suspended = 4
}

public enum QuizStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

public enum AttemptStatus
{
    InProgress = 1,
    Submitted = 2,
    TimedOut = 3
}
