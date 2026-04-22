namespace CyberOps.Common.Constants;

public static class OperationalSecurityWeights
{
    public const decimal OpenTickets = 0.25m;
    public const decimal Mttr = 0.20m;
    public const decimal SlaCompliance = 0.10m;
    public const decimal TotalActiveWeight = OpenTickets + Mttr + SlaCompliance;
}

public static class OpenTicketsScorePercentages
{
    public const decimal Best = 1.0m;
    public const decimal Medium = 0.7m;
    public const decimal Worst = 0.3m;
}
