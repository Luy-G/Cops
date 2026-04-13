public class Client
{
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}


public class ItsmTicket
{
    public long ItsmTicketId { get; set; }

    public long ClientId { get; set; }
    public string SourceSystem { get; set; } = "unknown";

    public string TicketKey { get; set; } = null!;
    public string IssueId { get; set; } = null!;

    public ItsmStatus Status { get; set; } = ItsmStatus.Unknown;
    public ItsmIssueType IssueType { get; set; } = ItsmIssueType.Unknown;
    public PriorityLevel Priority { get; set; } = PriorityLevel.Unknown;
    public ItsmResolution Resolution { get; set; } = ItsmResolution.None;

    public string? Title { get; set; }
    public string? DescriptionText { get; set; }
    public string? DescriptionHtml { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? CreatorName { get; set; }
    public string? CreatorEmail { get; set; }

    public string? CurrentAssigneeName { get; set; }
    public string? CurrentAssigneeEmail { get; set; }

    public string? ReporterName { get; set; }
    public string? ReporterEmail { get; set; }

    public string? FirstResponseDurationText { get; set; }
    public long? FirstResponseDurationMs { get; set; }
    public DateTime? FirstResponseSlaStartAt { get; set; }
    public DateTime? FirstResponseSlaCompleteAt { get; set; }
    public bool? FirstResponseSlaBreached { get; set; }

    public decimal? TimeSpentHours { get; set; }

    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}