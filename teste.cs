using System;
using System.Collections.Generic;
using Xunit;

public class ItsmCalculationsTests
{
    [Fact]
    public void CalculateOpenTicketsScore_ShouldReturnFull_WhenBelow100Threshold()
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

        var calcs = new ClientItsmCalcs
        {
            ClientId = 1,
            OpenTicketsCeilingMax = 10,
            OpenTicketsFloorMax = 50,
            MttrTargetHours = 24
        };

        var score = ItsmCalculations.CalculateOpenTicketsScore(tickets, calcs);

        Assert.Equal(OpenTicketsScorePercentages.Ceiling, score);
    }
}