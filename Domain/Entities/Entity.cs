using CyberOps.Domain.Enums;

namespace CyberOps.Domain.Entities;

public class Client
{
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public List<Domain> Domains { get; set; } = new();
}

public class Domain
{
    public int Id { get; set; }
    public DomainKey DomainKey { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<IMetric> Metrics { get; set; } = new();
}

public interface IMetric
{
    int Id { get; set; }
    string Name { get; set; }
    string Description { get; set; }
    string Expression { get; set; }
}

public class MttrMetric : IMetric
{
    public int Id { get; set; } = 1;
    public string Name { get; set; } = "MTTR Score";
    public string Description { get; set; } = "Score based on MTTR against target";
    public string Expression { get; set; } = "if(MttrTargetHours <= 0, 0, if(Mttr(tickets) <= MttrTargetHours, 1, MttrTargetHours / Mttr(tickets)))";
}

public class OpenTicketsMetric : IMetric
{
    public int Id { get; set; } = 2;
    public string Name { get; set; } = "Open Tickets Score";
    public string Description { get; set; } = "Score based on number of in-progress tickets";
    public string Expression { get; set; } =
        "if(OpenTicketsCount(tickets) <= OpenTicketsBestMax, 1.0, if(OpenTicketsCount(tickets) <= OpenTicketsMediumMax, 0.7, 0.3))";
}

public class SlaComplianceMetric : IMetric
{
    public int Id { get; set; } = 3;
    public string Name { get; set; } = "SLA Compliance Score";
    public string Description { get; set; } = "Closed tickets without SLA breach ratio";
    public string Expression { get; set; } = "SlaCompliance(tickets)";
}

public class Operationalsecitsm
{
    public long ItsmTicketId { get; set; }
    public long ClientId { get; set; }
    public SourceSystem SourceSystem { get; set; }
    public required string TicketKey { get; set; }
    public long IssueId { get; set; }
    public ItsmStatus Status { get; set; }
    public ItsmTicketType TicketType { get; set; }
    public PriorityLevel Priority { get; set; }
    public ItsmResolution Resolution { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? DescriptionHtml { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
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

public class ClientItsmCalcs
{
    public long ClientItsmCalculationId { get; set; }
    public long ClientId { get; set; }
    public int OpenTicketsBestMax { get; set; }
    public int OpenTicketsMediumMax { get; set; }
    public decimal MttrTargetHours { get; set; }
}

public class VulnerabilityAttackSurface
{
    public long VulnerabilityFindingId { get; set; }
    public long ClientId { get; set; }
    public SourceSystem SourceSystem { get; set; }
    public ScanEngine ScanEngine { get; set; }
    public required string FindingKey { get; set; }
    public VulnSeverity Severity { get; set; }
    public required string Title { get; set; }
    public string? Cve { get; set; }
    public decimal? Cvss { get; set; }
    public string? Host { get; set; }
    public string? Port { get; set; }
    public string? Evidence { get; set; }
    public string? Description { get; set; }
    public string? Impact { get; set; }
    public string? Recommendation { get; set; }
    public bool HasPublicExploit { get; set; }
    public bool IsInternetExposed { get; set; }
    public bool IsInKevCatalog { get; set; }
    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}

public class ClientVulnCalcs
{
    public long ClientVulnCalcsId { get; set; }
    public long ClientId { get; set; }
    public decimal CriticalPatchTargetHours { get; set; }
    public decimal HighPatchTargetHours { get; set; }
    public decimal MediumLowPatchTargetDays { get; set; }
    public decimal ScanCoverageHighThreshold { get; set; }
    public decimal ScanCoverageMediumThreshold { get; set; }
    public decimal AssetsExposedHighThreshold { get; set; }
    public decimal AssetsExposedMediumThreshold { get; set; }
}
