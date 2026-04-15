public enum ItsmStatus
{
        Unknown = 0,
        New = 1,
        Open = 2,
        Triaged = 3,
        InProgress = 4,
        Pending = 5,
        OnHold = 6,
        Resolved = 7,
        Closed = 8,
        Cancelled = 9,
        Reopened = 10
}

public enum ItsmIssueType
{
        Unknown = 0,
        Incident = 1,
        Task = 2,
        Subtask = 3,
        ServiceRequest = 4,
        Problem = 5,
        Change = 6,
        Case = 7,
        Other = 8
}

public enum PriorityLevel
{
        Unknown = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
}
public enum ItsmResolution
{
        None = 0,
        Unknown = 1,
        Done = 2,
        Resolved = 3,
        Fixed = 4,
        Workaround = 5,
        Duplicate = 6,
        WontFix = 7,
        NotReproducible = 8,
        Cancelled = 9,
        Rejected = 10
}



public enum DomainKey
{
        Unknown = 0,
        OperationalSecurity = 1,
        ThreatLandscape = 2,
        DetectionAndResponse = 3,
        HumanRisk = 4,
        VulnerabilityAndAttackSurface = 5,
        IdentityAndAccessSecurity = 6,
        GovernanceAndResilience = 7
}


public enum SourceSystem
{
    Other = 0,
    Jira = 1
}