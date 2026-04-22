using NCalc;
using NCalc.Handlers;

// motor de expressões NCalc com suporte a funções customizadas sobre dados ITSM
// as expressões das métricas chamam estas funções para calcular os valores em bruto
public static class NCalcEvaluator
{
    public static decimal Evaluate(
        string expression,
        Dictionary<string, object> parameters,
        IReadOnlyList<Operationalsecitsm>? tickets = null)
    {
        var e = new Expression(expression);

        foreach (var (key, value) in parameters)
            e.Parameters[key] = value;

        // passa a coleção de tickets como parâmetro opaco para as funções customizadas
        if (tickets != null)
            e.Parameters["tickets"] = tickets;

        // regista as funções C# que as expressões podem chamar
        e.EvaluateFunction += EvaluateCustomFunction;

        return Convert.ToDecimal(e.Evaluate());
    }

    // implementações das funções expostas ao NCalc
    // o argumento "tickets" dentro da expressão é resolvido via e.Parameters["tickets"]
    private static void EvaluateCustomFunction(string name, FunctionArgs args)
    {
        // o argumento "tickets" dentro da expressão é resolvido via e.Parameters["tickets"]
        if (args.Parameters[0].Evaluate() is not IReadOnlyList<Operationalsecitsm> list)
            return;

        switch (name)
        {
            case "OpenTicketsCount":
                args.Result = list.Count(t => t.Status == ItsmStatus.InProgress);
                break;

            case "Mttr":
                var closed = list.Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue).ToList();
                // se não houver tickets fechados o MTTR é 0 — a expressão trata este caso
                args.Result = closed.Count == 0 ? 0.0 : (double)closed.Average(t => t.TimeSpentHours!.Value);
                break;

            case "SlaCompliance":
                var closedWithSla = list.Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue).ToList();
                if (closedWithSla.Count == 0) { args.Result = 0.0; break; }
                var compliant = closedWithSla.Count(t => t.FirstResponseSlaBreached == false);
                args.Result = (double)compliant / closedWithSla.Count;
                break;
        }
    }
}

// domínio Segurança Operacional (peso 10%)
// cada métrica tem a sua expressão NCalc — o domínio só orquestra a avaliação e aplica os pesos
public class OperationalSecurityDomain : IDomain
{
    public DomainKey Key => DomainKey.OperationalSecurity;
    public decimal Weight => DomainWeights.OperationalSecurity;

    // métricas com os respetivos pesos dentro do domínio
    private static readonly IReadOnlyList<(IMetric Metric, decimal Weight)> Metrics =
    [
        (new OpenTicketsMetric(),    OperationalSecurityWeights.OpenTickets),
        (new MttrMetric(),           OperationalSecurityWeights.Mttr),
        (new SlaComplianceMetric(),  OperationalSecurityWeights.SlaCompliance),
    ];

    public decimal Calculate(DomainContext context)
    {
        if (context.ItsmCalcs is null || !context.ItsmTickets.Any())
            return 0m;

        var calcs = context.ItsmCalcs;
        var tickets = context.ItsmTickets;

        // thresholds por cliente passados como parâmetros escalares
        var parameters = new Dictionary<string, object>
        {
            ["OpenTicketsBestMax"]   = (double)calcs.OpenTicketsBestMax,
            ["OpenTicketsMediumMax"] = (double)calcs.OpenTicketsMediumMax,
            ["MttrTargetHours"]      = (double)calcs.MttrTargetHours,
        };

        // avalia cada métrica via NCalc e aplica o peso correspondente
        var weighted = Metrics.Sum(m =>
            NCalcEvaluator.Evaluate(m.Metric.Expression, parameters, tickets) * m.Weight);

        return weighted / OperationalSecurityWeights.TotalActiveWeight;
    }
}

// domínio Vulnerabilidade & Superfície de Ataque (peso 18%)
// usa dados de scan — criticals, highs, exploits, KEV, cobertura
public class VulnerabilityAndAttackSurfaceDomain : IDomain
{
    public DomainKey Key => DomainKey.VulnerabilityAndAttackSurface;
    public decimal Weight => DomainWeights.VulnerabilityAndAttackSurface;

    public decimal Calculate(DomainContext context)
    {
        if (!context.VulnFindings.Any())
            return 0m;

        return VulnCalculations.CalculateVulnerabilityDomainTotal(context.VulnFindings);
    }
}


// qcriar as métricas com expressões NCalc e implementar o Calculate() qnd tivermos

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

// calcula o score composto a partir dos scores de cada domínio ativo
// normaliza pelo peso total dos domínios ativos — clientes com 3 domínios têm score válido
public static class CompositeScoreCalculator
{
    private static readonly IReadOnlyDictionary<DomainKey, decimal> Weights =
        new Dictionary<DomainKey, decimal>
        {
            [DomainKey.OperationalSecurity]           = DomainWeights.OperationalSecurity,
            [DomainKey.ThreatLandscape]               = DomainWeights.ThreatLandscape,
            [DomainKey.VulnerabilityAndAttackSurface] = DomainWeights.VulnerabilityAndAttackSurface,
            [DomainKey.DetectionAndResponse]          = DomainWeights.DetectionAndResponse,
            [DomainKey.IdentityAndAccessSecurity]     = DomainWeights.IdentityAndAccessSecurity,
            [DomainKey.HumanRisk]                     = DomainWeights.HumanRisk,
            [DomainKey.GovernanceAndResilience]       = DomainWeights.GovernanceAndResilience,
        };

    public static decimal Calculate(IReadOnlyDictionary<DomainKey, decimal> domainScores)
    {
        if (domainScores.Count == 0)
            return 0m;

        var totalWeight = domainScores.Keys.Sum(k => Weights.GetValueOrDefault(k, 0m));
        if (totalWeight == 0m)
            return 0m;

        var weighted = domainScores.Sum(kv => kv.Value * Weights.GetValueOrDefault(kv.Key, 0m));
        return weighted / totalWeight;
    }
}
