public class SogilubJsonDto
{
    public required string TicketKey { get; set; }
    public required double IssueId { get; set; }

    public required string Status { get; set; }
    public required string IssueType { get; set; }
    public required string Priority { get; set; }
    public required string Resolution { get; set; }

    public required string Summary { get; set; }

    public string? Description { get; set; }
    public string? DescriptionHtml { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public string? CreatorName { get; set; }
    public string? CreatorEmail { get; set; }

    public string? AssigneeName { get; set; }
    public string? AssigneeEmail { get; set; }

    public string? ReporterName { get; set; }
    public string? ReporterEmail { get; set; }

    public string? FirstResponseDurationText { get; set; }
    public long? FirstResponseDurationMs { get; set; }
    public DateTimeOffset? FirstResponseSlaStartAt { get; set; }
    public DateTimeOffset? FirstResponseSlaCompleteAt { get; set; }
    public string? FirstResponseSlaBreached { get; set; }

    public decimal? TimeSpentHours { get; set; }
}