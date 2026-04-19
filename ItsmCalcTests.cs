using Xunit;
public class ItsmCalculationsTests
{
    // ve que com apenas 1 ticket aberto (abaixo do limite de 10), o score é o melhor possível
    [Fact]
    public void CalculateOpenTicketsScore_ShouldReturnBest_WhenBelowBestMaxThreshold()
    {
        var tickets = new List<ItsmTicket>
        {
            new ItsmTicket
            {
                ClientId = 1,
                SourceSystem = SourceSystem.Jira,
                TicketKey = "SR-1",
                IssueId = 1,
                Status = ItsmStatus.InProgress,
                TicketType = ItsmTicketType.Incident,
                Priority = PriorityLevel.Medium,
                Resolution = ItsmResolution.None,
                Title = "Test",
                CreatedAt = DateTime.UtcNow
            }
        };

        // configuramos os limites até 10 = best, até 50 = medium, acima = worst
        var calcs = new ClientItsmCalcs
        {
            ClientId = 1,
            OpenTicketsBestMax = 10,
            OpenTicketsMediumMax = 50,
            MttrTargetHours = 24
        };

        var score = ItsmCalculations.CalculateOpenTicketsScore(tickets, calcs);

        // Com 1 ticket aberto esperamos o score máximo
        Assert.Equal(OpenTicketsScorePercentages.Best, score);
    }
}
