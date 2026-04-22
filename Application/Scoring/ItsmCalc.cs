using CyberOps.Common.Constants;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Scoring;

public static class ItsmCalculations
{
    public static decimal CalculateMttrScore(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var mttr = CalculateMttr(tickets);
        var target = calcs.MttrTargetHours;

        if (target <= 0)
        {
            return 0m;
        }

        var divisor = mttr > target ? mttr : target;
        return target / divisor;
    }

    public static decimal CalculateSlaCompliance(IEnumerable<Operationalsecitsm> tickets)
    {
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue).ToList();
        if (!closedTickets.Any())
        {
            return 0m;
        }

        var compliantCount = closedTickets.Count(t => t.FirstResponseSlaBreached == false);
        return (decimal)compliantCount / closedTickets.Count;
    }

    public static int CalculateOpenTickets(IEnumerable<Operationalsecitsm> tickets)
    {
        return tickets.Count(t => t.Status == ItsmStatus.InProgress);
    }

    public static decimal CalculateOpenTicketsScore(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var count = CalculateOpenTickets(tickets);

        if (count <= calcs.OpenTicketsBestMax)
        {
            return OpenTicketsScorePercentages.Best;
        }

        if (count <= calcs.OpenTicketsMediumMax)
        {
            return OpenTicketsScorePercentages.Medium;
        }

        return OpenTicketsScorePercentages.Worst;
    }

    public static decimal CalculateOpenTicketsWeighted(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        return CalculateOpenTicketsScore(tickets, calcs) * OperationalSecurityWeights.OpenTickets;
    }

    public static decimal CalculateMttrWeighted(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        return CalculateMttrScore(tickets, calcs) * OperationalSecurityWeights.Mttr;
    }

    public static decimal CalculateSlaComplianceWeighted(IEnumerable<Operationalsecitsm> tickets)
    {
        return CalculateSlaCompliance(tickets) * OperationalSecurityWeights.SlaCompliance;
    }

    public static decimal CalculateOperationalSecurityItsmTotal(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var weighted = CalculateOpenTicketsWeighted(tickets, calcs)
            + CalculateMttrWeighted(tickets, calcs)
            + CalculateSlaComplianceWeighted(tickets);

        return weighted / OperationalSecurityWeights.TotalActiveWeight;
    }

    private static decimal CalculateMttr(IEnumerable<Operationalsecitsm> tickets)
    {
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue).ToList();
        if (!closedTickets.Any())
        {
            return 0m;
        }

        return closedTickets.Average(t => t.TimeSpentHours!.Value);
    }
}
