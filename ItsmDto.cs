using System.Text.Json.Serialization;

public class SogilubJsonDto
{
    //SR-5295
    public string? Key { get; set; }

    [JsonPropertyName("Issue ID")]
    public double? IssueId { get; set; }

    // Closed, In Progress, Open, etc.
    [JsonPropertyName("Current Status")]
    public string? Status { get; set; }

    //Incident, Task, Sub-task
    [JsonPropertyName("Issue Type")]
    public string? IssueType { get; set; }

    // P1 (Critical), P2 (Urgent), etc.
    public string? Priority { get; set; }

    // Done, null se ainda não resolvido
    public string? Resolution { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }

    [JsonPropertyName("Description (HTML)")]
    public string? DescriptionHtml { get; set; }

    public DateTimeOffset? Created { get; set; }

    //pode ser nulo se ainda estiver aberto
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

    // 59m, 3h 59m
    [JsonPropertyName("Time to first response")]
    public string? FirstResponseDurationText { get; set; }

    // double porque: 1.4366804E7
    [JsonPropertyName("Time to first response (ms)")]
    public double? FirstResponseDurationMs { get; set; }

    // data em que o SLA de primeira resposta começou a contar
    [JsonPropertyName("Time to first response: SLA Start date")]
    public DateTimeOffset? FirstResponseSlaStartAt { get; set; }

    // data limite do SLA de primeira resposta
    [JsonPropertyName("Time to first response: SLA Complete date")]
    public DateTimeOffset? FirstResponseSlaCompleteAt { get; set; }

    // "true" ou "false" em texto
    [JsonPropertyName("Time to first response: Breached?")]
    public string? FirstResponseSlaBreached { get; set; }

    // horas a trabalhar no ticket
    [JsonPropertyName("Time Spent")]
    public decimal? TimeSpentHours { get; set; }
}
