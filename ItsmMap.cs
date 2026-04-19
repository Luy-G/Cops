public static class SogilubItsmMapper
{
    // verifica se um texto é igual a alguma das opções
    private static bool IsOneOf(string? value, params string[] options)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim();

        return options.Any(option => normalized.Equals(option, StringComparison.OrdinalIgnoreCase));
    }

    //converte o estado do ticket em texto para o enum ItsmStatus
    public static ItsmStatus MapStatus(string? rawStatus)
    {
        return rawStatus switch
        {
            var x when IsOneOf(x, "Closed")      => ItsmStatus.Closed,
            var x when IsOneOf(x, "Resolved")    => ItsmStatus.Resolved,
            var x when IsOneOf(x, "In Progress") => ItsmStatus.InProgress,
            var x when IsOneOf(x, "Open")        => ItsmStatus.Open,
            var x when IsOneOf(x, "Pending")     => ItsmStatus.Pending,
            _                                    => ItsmStatus.Unknown
        };
    }

    // converte o tipo de ticket para o enum ItsmTicketType
    public static ItsmTicketType MapTicketType(string? rawTicketType)
    {
        return rawTicketType switch
        {
            var x when IsOneOf(x, "Incident") => ItsmTicketType.Incident,
            var x when IsOneOf(x, "Task")     => ItsmTicketType.Task,
            var x when IsOneOf(x, "Sub-task") => ItsmTicketType.Subtask,
            _                                 => ItsmTicketType.Unknown
        };
    }

    // converte a prioridade para o enum PriorityLevel
    public static PriorityLevel MapPriority(string? rawPriority)
    {
        return rawPriority switch
        {
            var x when IsOneOf(x, "P1 (Critical)") => PriorityLevel.Critical,
            var x when IsOneOf(x, "P2 (Urgent)")   => PriorityLevel.High,
            var x when IsOneOf(x, "P3 (Normal)")   => PriorityLevel.Medium,
            var x when IsOneOf(x, "P4 (Low)")      => PriorityLevel.Low,
            _                                      => PriorityLevel.Unknown
        };
    }

    // converte a resolução para o enum ItsmResolution
    // se vier nulo ou vazio (ticket ainda aberto) fica como none
    public static ItsmResolution MapResolution(string? rawResolution)
    {
        return rawResolution switch
        {
            var x when string.IsNullOrWhiteSpace(x) => ItsmResolution.None,
            var x when IsOneOf(x, "Done")           => ItsmResolution.Done,
            _                                       => ItsmResolution.Unknown
        };
    }
}
