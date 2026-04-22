// interface que define o contrato do processador por cliente
// cada fonte de dados (Sogilub, etc) tem a sua implementação
public interface IProcessor
{
    Task<ProcessorResult> ProcessAsync(ProcessorInput input);
}

// o que o processador recebe para correr o pipeline completo
public class ProcessorInput
{
    public required long ClientId { get; init; }

    // domínios ativos para este cliente — só estes são calculados
    public IReadOnlyList<IDomain> ActiveDomains { get; init; } = [];

    // JSON raw do Jira ITSM (pode vir nulo se não houver dados ITSM)
    public string? ItsmJson { get; init; }

    // JSON raw do relatório de vulnerabilidades (pode vir nulo)
    public string? VulnJson { get; init; }

    // configurações de cálculo por cliente
    public ClientItsmCalcs? ItsmCalcs { get; init; }
    public ClientVulnCalcs? VulnCalcs { get; init; }
}

// resultado do pipeline completo para um cliente
public class ProcessorResult
{
    public bool Success { get; init; }

    // erros de validação ou de processamento, se houver
    public IReadOnlyList<string> Errors { get; init; } = [];

    // entidades ingeridas e normalizadas
    public IReadOnlyList<Operationalsecitsm> ItsmTickets { get; init; } = [];
    public IReadOnlyList<VulnerabilityAttackSurface> VulnFindings { get; init; } = [];

    // scores por domínio (0–1)
    public IReadOnlyDictionary<DomainKey, decimal> DomainScores { get; init; } =
        new Dictionary<DomainKey, decimal>();

    // score composto final ponderado pelos domínios ativos
    public decimal CompositeScore { get; init; }
}
