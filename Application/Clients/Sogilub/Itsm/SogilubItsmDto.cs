using System.Text.Json.Serialization;

namespace CyberOps.Application.Clients.Sogilub.Itsm;

public class SogilubItsmDto
{
    public string? Key { get; set; }

    [JsonPropertyName("Issue ID")]
    public double? IssueId { get; set; }

    [JsonPropertyName("Current Status")]
    public string? Status { get; set; }

    [JsonPropertyName("Issue Type")]
    public string? IssueType { get; set; }

    public string? Priority { get; set; }
    public string? Resolution { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }

    [JsonPropertyName("Description (HTML)")]
    public string? DescriptionHtml { get; set; }

    public DateTimeOffset? Created { get; set; }
    public DateTimeOffset? Resolved { get; set; }
    public DateTimeOffset? Updated { get; set; }

    [JsonPropertyName("Creator: Name")]
    public string? CreatorName { get; set; }

    [JsonPropertyName("Creator: Email")]
    public string? CreatorEmail { get; set; }

    [JsonPropertyName("Current Assignee: Name")]
    public string? AssigneeName { get; set; }

    [JsonPropertyName("Current Assignee: Email")]
    public string? AssigneeEmail { get; set; }

    [JsonPropertyName("Reporter: Name")]
    public string? ReporterName { get; set; }

    [JsonPropertyName("Reporter: Email")]
    public string? ReporterEmail { get; set; }

    [JsonPropertyName("Time to first response")]
    public string? FirstResponseDurationText { get; set; }

    [JsonPropertyName("Time to first response (ms)")]
    public double? FirstResponseDurationMs { get; set; }

    [JsonPropertyName("Time to first response: SLA Start date")]
    public DateTimeOffset? FirstResponseSlaStartAt { get; set; }

    [JsonPropertyName("Time to first response: SLA Complete date")]
    public DateTimeOffset? FirstResponseSlaCompleteAt { get; set; }

    [JsonPropertyName("Time to first response: Breached?")]
    public string? FirstResponseSlaBreached { get; set; }

    [JsonPropertyName("Time Spent")]
    public decimal? TimeSpentHours { get; set; }
}
