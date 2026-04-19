public static class OperationalSecurityWeights
{
    public const decimal OpenTickets = 0.25m;   // tickets com estado In Progress
    public const decimal Mttr = 0.20m;          // média de horas até resolução
    public const decimal SlaCompliance = 0.10m; // percentagem de tickets sem violação de SLA
    public const decimal TotalActiveWeight = OpenTickets + Mttr + SlaCompliance;
}

public static class OpenTicketsScorePercentages
{
    public const decimal Best = 1.0m;   // número de tickets abertos está dentro do limite ideal
    public const decimal Medium = 0.7m; // número de tickets abertos está acima do ideal mas aceitável
    public const decimal Worst = 0.3m;  // demasiados tickets abertos
}
