using CyberOps.Application.Scoring;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Contracts.Processing;

public interface IProcessor
{
    Task<ProcessorResult> ProcessAsync(ProcessorInput input);
}

public class ProcessorInput
{
    public required long ClientId { get; init; }
    public IReadOnlyList<IDomain> ActiveDomains { get; init; } = [];
    public string? ItsmJson { get; init; }
    public string? VulnJson { get; init; }
    public ClientItsmCalcs? ItsmCalcs { get; init; }
    public ClientVulnCalcs? VulnCalcs { get; init; }
}

public class ProcessorResult
{
    public bool Success { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public IReadOnlyList<Operationalsecitsm> ItsmTickets { get; init; } = [];
    public IReadOnlyList<VulnerabilityAttackSurface> VulnFindings { get; init; } = [];
    public IReadOnlyDictionary<DomainKey, decimal> DomainScores { get; init; } = new Dictionary<DomainKey, decimal>();
    public decimal CompositeScore { get; init; }
}
