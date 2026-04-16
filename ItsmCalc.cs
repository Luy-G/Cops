using System.Collections.Generic;
using System.Linq;

public static class ItsmCalculations
{
    public static decimal CalculateMttr(IEnumerable<ItsmTicket> tickets)
    {
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue).ToList();

        if (!closedTickets.Any())
            return 0m;

        return closedTickets.Average(t => t.TimeSpentHours!.Value);
    }

    public static decimal CalculateMttrScore(IEnumerable<ItsmTicket> tickets,ClientItsmCalcVar calcs)
    {
        var mttr = CalculateMttr(tickets);
        var target = calcs.MttrTargetHours;

        if (target <= 0)
            return 0m;

        var divisor = mttr > target ? mttr : target;

        return target / divisor;
    }

    public static decimal CalculateSlaCompliance(IEnumerable<ItsmTicket> tickets)
    {
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue).ToList();

        if (!closedTickets.Any())
            return 0m;

        var compliantTickets = closedTickets.Count(t => t.FirstResponseSlaBreached == false);

        return (decimal)compliantTickets / closedTickets.Count;
    }

    public static int CalculateOpenTickets(IEnumerable<ItsmTicket> tickets)
    {
        return tickets.Count(t => t.Status == ItsmStatus.InProgress);
    }

    public static decimal CalculateOpenTicketsScore(IEnumerable<ItsmTicket> tickets,ClientItsmCalcs calcs)
    {
        var openTickets = CalculateOpenTickets(tickets);

        if (openTickets <= calcs.OpenTicketsBestMax)
            return OpenTicketsScorePercentages.Best;

        if (openTickets <= calcs.OpenTicketsMediumMax)
            return OpenTicketsScorePercentages.Medium;

        return OpenTicketsScorePercentages.Worst;
    }

    public static decimal CalculateOpenTicketsWeighted(IEnumerable<ItsmTicket> tickets,ClientItsmCalcs calcs)
    {
        return CalculateOpenTicketsScore(tickets, calcs) * OperationalSecurityWeights.OpenTickets;
    }

    public static decimal CalculateMttrWeighted(IEnumerable<ItsmTicket> tickets,ClientItsmCalcs calcs)
    {
        return CalculateMttrScore(tickets, calcs) * OperationalSecurityWeights.Mttr;
    }

    public static decimal CalculateSlaComplianceWeighted(IEnumerable<ItsmTicket> tickets)
    {
        return CalculateSlaCompliance(tickets) * OperationalSecurityWeights.SlaCompliance;
    }

    public static decimal CalculateOperationalSecurityItsmTotal(IEnumerable<ItsmTicket> tickets,ClientItsmCalcs calcs)
    {
        return CalculateOpenTicketsWeighted(tickets, calcs)+ CalculateMttrWeighted(tickets, calcs)+ CalculateSlaComplianceWeighted(tickets);
    }
}