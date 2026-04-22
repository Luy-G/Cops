public class Client
{
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;

    // deixa desativar um cliente sem o apagar da base de dados
    public bool IsActive { get; set; } = true;

    public List<Domain> Domains { get; set; } = new() ;
}

public class Domain
{
    public int Id { get; set; }
    public DomainKey DomainKey { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<IMetrics> Metrics { get; set; } = new() ;
}

public interface IMetric
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Expression { get; set; } = null!;
}


public class MttrMetric : IMetric
{
    public string Name => "MTTR Score";
    public string Description => "Score baseado no MTTR face ao target";
    public string Expression => "if(MttrTargetHours <= 0, 0, if(mttrHours <= MttrTargetHours, 1, MttrTargetHours / mttrHours))";
}

//operationalsecitsm

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
    public bool? FirstResponseSlaBreached { get; set; }     // true se o SLA foi violado

    public decimal? TimeSpentHours { get; set; }

    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}


//constantes sogilub,etc  mesmo para vulns
public class ClientItsmCalcs
{
    public long ClientItsmCalculationId { get; set; }
    public long ClientId { get; set; }

    //número máximo de tickets abertos para score máximo
    public int OpenTicketsBestMax { get; set; }

    // número máximo de tickets abertos para score médio
    public int OpenTicketsMediumMax { get; set; }

    // objetivo de mttr em horas
    public decimal MttrTargetHours { get; set; }
}

// vulnerabilidade encontrada num scan de segurança
public class VulnerabilityAttackSurface
{
    public long VulnerabilityFindingId { get; set; }

    public long ClientId { get; set; }

    public SourceSystem SourceSystem { get; set; }

    // ferramenta que fez o scan — confirmar com o cliente antes de definir
    public ScanEngine ScanEngine { get; set; }

    // F-01
    public required string FindingKey { get; set; }

    //Critical, High, Medium, Low, Info
    public VulnSeverity Severity { get; set; }

    public required string Title { get; set; }

    public string? Cve { get; set; }      // CVE-2019-1068
    public decimal? Cvss { get; set; }    // 8.8
    public string? Host { get; set; }
    public string? Port { get; set; }

    public string? Evidence { get; set; }

    public string? Description { get; set; }
    public string? Impact { get; set; }
    public string? Recommendation { get; set; }

    public bool HasPublicExploit { get; set; }   //exploit público conhecido?
    public bool IsInternetExposed { get; set; }  //exposto à internet?
    public bool IsInKevCatalog { get; set; }

    public DateTime IngestedAt { get; set; } = DateTime.UtcNow;
}

public class ClientVulnCalcs
{
    public long ClientVulnCalcsId { get; set; }
    public long ClientId { get; set; }

    // MeanTimeToPatch por severidade
    public decimal CriticalPatchTargetHours { get; set; }       // default: 4h
    public decimal HighPatchTargetHours { get; set; }           // default: 12h
    public decimal MediumLowPatchTargetDays { get; set; }       // default: 5 dias úteis

    //thresholds para ScanCoverage e AssetsExposed
    public decimal ScanCoverageHighThreshold { get; set; }
    public decimal ScanCoverageMediumThreshold { get; set; }
    public decimal AssetsExposedHighThreshold { get; set; }
    public decimal AssetsExposedMediumThreshold { get; set; }
}




//ASSETS DE CLIENTE  tabela fabricante/modelo/licenciamento/ 