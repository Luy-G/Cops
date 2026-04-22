using CyberOps.Application.Scoring;
using CyberOps.Common.Constants;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;
using Xunit;

namespace CyberOps.Tests;

public class ItsmCalculationsTests
{
    [Fact]
    public void CalculateOpenTicketsScore_ShouldReturnBest_WhenBelowBestMaxThreshold()
    {
        var tickets = new List<Operationalsecitsm>
        {
            new()
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
            OpenTicketsBestMax = 10,
            OpenTicketsMediumMax = 50,
            MttrTargetHours = 24
        };

        var score = ItsmCalculations.CalculateOpenTicketsScore(tickets, calcs);
        Assert.Equal(OpenTicketsScorePercentages.Best, score);
    }
}
