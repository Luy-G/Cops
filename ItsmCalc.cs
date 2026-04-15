using System.Collections.Generic;
using System.Linq;

public static class ItsmCalculations
{
    public static decimal CalculateMttr(List<ItsmTicket> tickets)
    {
        var closedTickets = tickets
            .Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue)
            .ToList();

        if (!closedTickets.Any())
            return 0m;

        return closedTickets.Average(t => t.TimeSpentHours!.Value);
    }

    public static decimal CalculateMttrScore(
        List<ItsmTicket> tickets,
        ClientItsmThreshold threshold)
    {
        var mttr = CalculateMttr(tickets);
        var target = threshold.MttrTargetHours;

        if (target <= 0)
            return 0m;

        var divisor = mttr > target ? mttr : target;

        return target / divisor;
    }

    public static decimal CalculateSlaCompliance(List<ItsmTicket> tickets)
    {
        var closedTickets = tickets
            .Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue)
            .ToList();

        if (!closedTickets.Any())
            return 0m;

        var compliantTickets = closedTickets.Count(t => t.FirstResponseSlaBreached == false);

        return (decimal)compliantTickets / closedTickets.Count;
    }

    public static int CalculateOpenTickets(List<ItsmTicket> tickets)
    {
        return tickets.Count(t => t.Status == ItsmStatus.InProgress);
    }

    public static decimal CalculateOpenTicketsScore(
        List<ItsmTicket> tickets,
        ClientItsmThreshold threshold)
    {
        var openTickets = CalculateOpenTickets(tickets);

        if (openTickets <= threshold.OpenTickets100Max)
            return 1m;

        if (openTickets <= threshold.OpenTickets70Max)
            return 0.7m;

        return 0.3m;
    }

    public static decimal CalculateOpenTicketsWeighted(
        List<ItsmTicket> tickets,
        ClientItsmThreshold threshold)
    {
        return CalculateOpenTicketsScore(tickets, threshold) * OperationalSecurityWeights.OpenTickets;
    }

    public static decimal CalculateMttrWeighted(
        List<ItsmTicket> tickets,
        ClientItsmThreshold threshold)
    {
        return CalculateMttrScore(tickets, threshold) * OperationalSecurityWeights.Mttr;
    }

    public static decimal CalculateSlaComplianceWeighted(List<ItsmTicket> tickets)
    {
        return CalculateSlaCompliance(tickets) * OperationalSecurityWeights.SlaCompliance;
    }

    public static decimal CalculateOperationalSecurityItsmTotal(
        List<ItsmTicket> tickets,
        ClientItsmThreshold threshold)
    {
        return CalculateOpenTicketsWeighted(tickets, threshold)
             + CalculateMttrWeighted(tickets, threshold)
             + CalculateSlaComplianceWeighted(tickets);
    }
}