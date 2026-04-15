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

    public static decimal CalculateMttrScore(List<ItsmTicket> tickets)
    {
        var mttr = CalculateMttr(tickets);

        if (mttr <= 4)
            return 1m;

        if (mttr <= 8)
            return 0.7m;

        return 0.3m;
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

    public static decimal CalculateSlaComplianceScore(List<ItsmTicket> tickets)
    {
        return CalculateSlaCompliance(tickets);
    }

    public static int CalculateOpenTickets(List<ItsmTicket> tickets)
    {
        return tickets.Count(t => t.Status == ItsmStatus.InProgress);
    }

    public static decimal CalculateOpenTicketsScore(List<ItsmTicket> tickets)
    {
        var openTickets = CalculateOpenTickets(tickets);

        if (openTickets <= 10)
            return 1m;

        if (openTickets <= 50)
            return 0.7m;

        return 0.3m;
    }
}