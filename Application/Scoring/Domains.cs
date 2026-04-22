using CyberOps.Common.Constants;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;
using NCalc;
using NCalc.Handlers;

namespace CyberOps.Application.Scoring;

public interface IDomain
{
    DomainKey Key { get; }
    decimal Weight { get; }
    decimal Calculate(DomainContext context);
}

public static class DomainWeights
{
    public const decimal ThreatLandscape = 0.18m;
    public const decimal VulnerabilityAndAttackSurface = 0.18m;
    public const decimal DetectionAndResponse = 0.18m;
    public const decimal IdentityAndAccessSecurity = 0.14m;
    public const decimal GovernanceAndResilience = 0.12m;
    public const decimal OperationalSecurity = 0.10m;
    public const decimal HumanRisk = 0.10m;
}

public class DomainContext
{
    public required long ClientId { get; init; }
    public IReadOnlyList<Operationalsecitsm> ItsmTickets { get; init; } = [];
    public IReadOnlyList<VulnerabilityAttackSurface> VulnFindings { get; init; } = [];
    public ClientItsmCalcs? ItsmCalcs { get; init; }
    public ClientVulnCalcs? VulnCalcs { get; init; }
}

public static class NCalcEvaluator
{
    public static decimal Evaluate(
        string expression,
        Dictionary<string, object> parameters,
        IReadOnlyList<Operationalsecitsm>? tickets = null)
    {
        var evaluator = new Expression(expression);

        foreach (var (key, value) in parameters)
        {
            evaluator.Parameters[key] = value;
        }

        if (tickets is not null)
        {
            evaluator.Parameters["tickets"] = tickets;
        }

        evaluator.EvaluateFunction += EvaluateCustomFunction;
        return Convert.ToDecimal(evaluator.Evaluate());
    }

    private static void EvaluateCustomFunction(string name, FunctionArgs args)
    {
        if (args.Parameters[0].Evaluate() is not IReadOnlyList<Operationalsecitsm> list)
        {
            return;
        }

        switch (name)
        {
            case "OpenTicketsCount":
                args.Result = list.Count(t => t.Status == ItsmStatus.InProgress);
                break;

            case "Mttr":
                var closed = list.Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue).ToList();
                args.Result = closed.Count == 0 ? 0.0 : (double)closed.Average(t => t.TimeSpentHours!.Value);
                break;

            case "SlaCompliance":
                var closedWithSla = list.Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue).ToList();
                if (closedWithSla.Count == 0)
                {
                    args.Result = 0.0;
                    break;
                }

                var compliant = closedWithSla.Count(t => t.FirstResponseSlaBreached == false);
                args.Result = (double)compliant / closedWithSla.Count;
                break;
        }
    }
}

public class OperationalSecurityDomain : IDomain
{
    public DomainKey Key => DomainKey.OperationalSecurity;
    public decimal Weight => DomainWeights.OperationalSecurity;

    private static readonly IReadOnlyList<(IMetric Metric, decimal Weight)> Metrics =
    [
        (new OpenTicketsMetric(), OperationalSecurityWeights.OpenTickets),
        (new MttrMetric(), OperationalSecurityWeights.Mttr),
        (new SlaComplianceMetric(), OperationalSecurityWeights.SlaCompliance),
    ];

    public decimal Calculate(DomainContext context)
    {
        if (context.ItsmCalcs is null || !context.ItsmTickets.Any())
        {
            return 0m;
        }

        var calcs = context.ItsmCalcs;
        var tickets = context.ItsmTickets;

        var parameters = new Dictionary<string, object>
        {
            ["OpenTicketsBestMax"] = (double)calcs.OpenTicketsBestMax,
            ["OpenTicketsMediumMax"] = (double)calcs.OpenTicketsMediumMax,
            ["MttrTargetHours"] = (double)calcs.MttrTargetHours
        };

        var weighted = Metrics.Sum(metric =>
            NCalcEvaluator.Evaluate(metric.Metric.Expression, parameters, tickets) * metric.Weight);

        return weighted / OperationalSecurityWeights.TotalActiveWeight;
    }
}

public class VulnerabilityAndAttackSurfaceDomain : IDomain
{
    public DomainKey Key => DomainKey.VulnerabilityAndAttackSurface;
    public decimal Weight => DomainWeights.VulnerabilityAndAttackSurface;

    public decimal Calculate(DomainContext context)
    {
        if (!context.VulnFindings.Any())
        {
            return 0m;
        }

        return VulnCalculations.CalculateVulnerabilityDomainTotal(context.VulnFindings);
    }
}

public class ThreatLandscapeDomain : IDomain
{
    public DomainKey Key => DomainKey.ThreatLandscape;
    public decimal Weight => DomainWeights.ThreatLandscape;
    public decimal Calculate(DomainContext context) => 0m;
}

public class DetectionAndResponseDomain : IDomain
{
    public DomainKey Key => DomainKey.DetectionAndResponse;
    public decimal Weight => DomainWeights.DetectionAndResponse;
    public decimal Calculate(DomainContext context) => 0m;
}

public class IdentityAndAccessSecurityDomain : IDomain
{
    public DomainKey Key => DomainKey.IdentityAndAccessSecurity;
    public decimal Weight => DomainWeights.IdentityAndAccessSecurity;
    public decimal Calculate(DomainContext context) => 0m;
}

public class HumanRiskDomain : IDomain
{
    public DomainKey Key => DomainKey.HumanRisk;
    public decimal Weight => DomainWeights.HumanRisk;
    public decimal Calculate(DomainContext context) => 0m;
}

public class GovernanceAndResilienceDomain : IDomain
{
    public DomainKey Key => DomainKey.GovernanceAndResilience;
    public decimal Weight => DomainWeights.GovernanceAndResilience;
    public decimal Calculate(DomainContext context) => 0m;
}

public static class CompositeScoreCalculator
{
    private static readonly IReadOnlyDictionary<DomainKey, decimal> Weights =
        new Dictionary<DomainKey, decimal>
        {
            [DomainKey.OperationalSecurity] = DomainWeights.OperationalSecurity,
            [DomainKey.ThreatLandscape] = DomainWeights.ThreatLandscape,
            [DomainKey.VulnerabilityAndAttackSurface] = DomainWeights.VulnerabilityAndAttackSurface,
            [DomainKey.DetectionAndResponse] = DomainWeights.DetectionAndResponse,
            [DomainKey.IdentityAndAccessSecurity] = DomainWeights.IdentityAndAccessSecurity,
            [DomainKey.HumanRisk] = DomainWeights.HumanRisk,
            [DomainKey.GovernanceAndResilience] = DomainWeights.GovernanceAndResilience
        };

    public static decimal Calculate(IReadOnlyDictionary<DomainKey, decimal> domainScores)
    {
        if (domainScores.Count == 0)
        {
            return 0m;
        }

        var totalWeight = domainScores.Keys.Sum(key => Weights.GetValueOrDefault(key, 0m));
        if (totalWeight == 0m)
        {
            return 0m;
        }

        var weighted = domainScores.Sum(score => score.Value * Weights.GetValueOrDefault(score.Key, 0m));
        return weighted / totalWeight;
    }
}
