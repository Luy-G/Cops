public class JsonSogilubDto
{
    public string? TicketKey { get; set; }
    public string? IssueId { get; set; }

    public string? Status { get; set; }
    public string? IssueType { get; set; }
    public string? Priority { get; set; }
    public string? Resolution { get; set; }

    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string? DescriptionHtml { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? CreatorName { get; set; }
    public string? CreatorEmail { get; set; }

    public string? AssigneeName { get; set; }
    public string? AssigneeEmail { get; set; }

    public string? ReporterName { get; set; }
    public string? ReporterEmail { get; set; }

    public string? FirstResponseDurationText { get; set; }
    public long? FirstResponseDurationMs { get; set; }
    public DateTime? FirstResponseSlaStartAt { get; set; }
    public DateTime? FirstResponseSlaCompleteAt { get; set; }
    public bool? FirstResponseSlaBreached { get; set; }

    public decimal? TimeSpentHours { get; set; }
}