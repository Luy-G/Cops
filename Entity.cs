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

    public List<IMetric> Metrics { get; set; } = new();
}

public interface IMetric
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    // expressão NCalc avaliada em runtime com os parâmetros do contexto
    public string Expression { get; set; }
}

public class MttrMetric : IMetric
{
    public int Id { get; set; } = 1;
    public string Name { get; set; } = "MTTR Score";
    public string Description { get; set; } = "Score baseado no MTTR face ao target";
    // Mttr(tickets) é uma função customizada exposta ao NCalc — calcula a média de horas nos tickets fechados
    public string Expression { get; set; } = "if(MttrTargetHours <= 0, 0, if(Mttr(tickets) <= MttrTargetHours, 1, MttrTargetHours / Mttr(tickets)))";
}

public class OpenTicketsMetric : IMetric
{
    public int Id { get; set; } = 2;
    public string Name { get; set; } = "Open Tickets Score";
    public string Description { get; set; } = "Score baseado no número de tickets em progresso";
    // OpenTicketsCount(tickets) conta quantos tickets têm estado InProgress
    public string Expression { get; set; } =
        "if(OpenTicketsCount(tickets) <= OpenTicketsBestMax, 1.0, if(OpenTicketsCount(tickets) <= OpenTicketsMediumMax, 0.7, 0.3))";
}

public class SlaComplianceMetric : IMetric
{
    public int Id { get; set; } = 3;
    public string Name { get; set; } = "SLA Compliance Score";
    public string Description { get; set; } = "Percentagem de tickets fechados sem violação de SLA";
    // SlaCompliance(tickets) devolve o rácio de tickets fechados sem breach de SLA
    public string Expression { get; set; } = "SlaCompliance(tickets)";
}

// interface que todos os domínios de scoring têm de implementar
public interface IDomain
{
    DomainKey Key { get; }
    // peso do domínio no score composto (ex: 0.18 para 18%)
    decimal Weight { get; }
    // retorna o score entre 0 e 1 (0 = risco máximo, 1 = sem risco)
    decimal Calculate(DomainContext context);
}

// pesos fixos dos 7 domínios - iguais para todos os clientes
public static class DomainWeights
{
    public const decimal ThreatLandscape               = 0.18m;
    public const decimal VulnerabilityAndAttackSurface = 0.18m;
    public const decimal DetectionAndResponse          = 0.18m;
    public const decimal IdentityAndAccessSecurity     = 0.14m;
    public const decimal GovernanceAndResilience       = 0.12m;
    public const decimal OperationalSecurity           = 0.10m;
    public const decimal HumanRisk                     = 0.10m;
}

// agrupa os dados disponíveis para um cliente num dado momento
public class DomainContext
{
    public required long ClientId { get; init; }
    public IReadOnlyList<Operationalsecitsm> ItsmTickets { get; init; } = [];
    public IReadOnlyList<VulnerabilityAttackSurface> VulnFindings { get; init; } = [];
    public ClientItsmCalcs? ItsmCalcs { get; init; }
    public ClientVulnCalcs? VulnCalcs { get; init; }
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

    // ferramenta que fez o scan — confirmar
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