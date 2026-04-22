public static class ItsmCalculations
{


    // passa o mttr para um score entre 0 e 1
    // quanto mais o mttr ultrapassar o objetivo mais o score se aproxima de 0
    public static decimal CalculateMttrScore(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var mttr = CalculateMttr(tickets);
        var target = calcs.MttrTargetHours;

        //objetivo a zero não faz sentido
        if (target <= 0)
            return 0m;

        // se o mttr for melhor que o objetivo, usa o objetivo como divisor para não passar 1.0
        var divisor = mttr > target ? mttr : target;

        return target / divisor;
    }

    // quantos tickets fechados não violaram o SLA.
    // 0.9 significa 90% de conformidade.
    public static decimal CalculateSlaCompliance(IEnumerable<Operationalsecitsm> tickets)
    {
        // conta tickets fechados com o campo SlaBreached preenchido
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.FirstResponseSlaBreached.HasValue).ToList();

        if (!closedTickets.Any())
            return 0m;

        //tickets onde o SLA nao foi violado
        var compliantCount = closedTickets.Count(t => t.FirstResponseSlaBreached == false);

        return (decimal)compliantCount / closedTickets.Count;
    }

    // cnta quantos tickets estão neste momento com estado In Progress
    //número que determina o score de tickets abertos.
    public static int CalculateOpenTickets(IEnumerable<Operationalsecitsm> tickets)
    {
        return tickets.Count(t => t.Status == ItsmStatus.InProgress);
    }

    // Abaixo de BestMax = score máximo, etc
    public static decimal CalculateOpenTicketsScore(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var count = CalculateOpenTickets(tickets);

        if (count <= calcs.OpenTicketsBestMax)
            return OpenTicketsScorePercentages.Best;

        if (count <= calcs.OpenTicketsMediumMax)
            return OpenTicketsScorePercentages.Medium;

        return OpenTicketsScorePercentages.Worst;
    }

    // multiplica o score de tickets abertos pelo seu peso no domínio(25%)
    public static decimal CalculateOpenTicketsWeighted(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        return CalculateOpenTicketsScore(tickets, calcs) * OperationalSecurityWeights.OpenTickets;
    }

    // multiplica o score de mttr pelo seu peso no domínio(20%)
    public static decimal CalculateMttrWeighted(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        return CalculateMttrScore(tickets, calcs) * OperationalSecurityWeights.Mttr;
    }

    // multiplica o score de SLA pelo seu peso no domínio(10%)
    public static decimal CalculateSlaComplianceWeighted(IEnumerable<Operationalsecitsm> tickets)
    {
        return CalculateSlaCompliance(tickets) * OperationalSecurityWeights.SlaCompliance;
    }

    // Calcula o score total do domínio Segurança Operacional com os dados ITSM disponíveis.
    public static decimal CalculateOperationalSecurityItsmTotal(IEnumerable<Operationalsecitsm> tickets, ClientItsmCalcs calcs)
    {
        var weighted = CalculateOpenTicketsWeighted(tickets, calcs)
            + CalculateMttrWeighted(tickets, calcs)
            + CalculateSlaComplianceWeighted(tickets);

        return weighted / OperationalSecurityWeights.TotalActiveWeight;
    }




        // média de horas gastas nos tickets fechados.
    //conta tickets com estado Closed e que tenham o campo TimeSpentHours not null
    private static decimal CalculateMttr(IEnumerable<Operationalsecitsm> tickets)
    {
        var closedTickets = tickets.Where(t => t.Status == ItsmStatus.Closed && t.TimeSpentHours.HasValue).ToList();

        // se não houver tickets fechados da 0
        if (!closedTickets.Any())
            return 0m;

        return closedTickets.Average(t => t.TimeSpentHours!.Value);
    }
}
