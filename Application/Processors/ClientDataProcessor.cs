using CyberOps.Application.Contracts.Processing;
using CyberOps.Application.Ingestion.Shared;
using CyberOps.Application.Scoring;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Processors;

public class ClientDataProcessor : IProcessor
{
    private readonly IReadOnlyList<IItsmIngestionAdapter> _itsmAdapters;
    private readonly IReadOnlyList<IVulnIngestionAdapter> _vulnAdapters;

    public ClientDataProcessor(IEnumerable<IItsmIngestionAdapter> itsmAdapters, IEnumerable<IVulnIngestionAdapter> vulnAdapters)
    {
        _itsmAdapters = itsmAdapters.ToList();
        _vulnAdapters = vulnAdapters.ToList();
    }

    public Task<ProcessorResult> ProcessAsync(ProcessorInput input)
    {
        var errors = new List<string>();
        var tickets = new List<Domain.Entities.Operationalsecitsm>();
        var findings = new List<Domain.Entities.VulnerabilityAttackSurface>();

        if (!string.IsNullOrWhiteSpace(input.ItsmJson))
        {
            var adapter = _itsmAdapters.FirstOrDefault(candidate => candidate.CanHandle(input.ClientId));
            if (adapter is null)
            {
                errors.Add($"No ITSM adapter registered for clientId {input.ClientId}.");
            }
            else
            {
                var itmsResult = adapter.Ingest(input.ItsmJson, input.ClientId);
                errors.AddRange(itmsResult.Errors);
                tickets.AddRange(itmsResult.Items);
            }
        }

        if (!string.IsNullOrWhiteSpace(input.VulnJson))
        {
            var adapter = _vulnAdapters.FirstOrDefault(candidate => candidate.CanHandle(input.ClientId));
            if (adapter is null)
            {
                errors.Add($"No Vulnerability adapter registered for clientId {input.ClientId}.");
            }
            else
            {
                var vulnResult = adapter.Ingest(input.VulnJson, input.ClientId);
                errors.AddRange(vulnResult.Errors);
                findings.AddRange(vulnResult.Items);
            }
        }

        var context = new DomainContext
        {
            ClientId = input.ClientId,
            ItsmTickets = tickets,
            VulnFindings = findings,
            ItsmCalcs = input.ItsmCalcs,
            VulnCalcs = input.VulnCalcs
        };

        var domainScores = new Dictionary<DomainKey, decimal>();
        foreach (var domain in input.ActiveDomains)
        {
            domainScores[domain.Key] = domain.Calculate(context);
        }

        var composite = CompositeScoreCalculator.Calculate(domainScores);

        return Task.FromResult(new ProcessorResult
        {
            Success = errors.Count == 0,
            Errors = errors,
            ItsmTickets = tickets,
            VulnFindings = findings,
            DomainScores = domainScores,
            CompositeScore = composite
        });
    }
}
