using CyberOps.Domain.Enums;

namespace CyberOps.Application.Clients.Sogilub.Itsm;

public static class SogilubItsmMapper
{
    private static bool IsOneOf(string? value, params string[] options)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        return options.Any(option => normalized.Equals(option, StringComparison.OrdinalIgnoreCase));
    }

    public static ItsmStatus MapStatus(string? rawStatus)
    {
        return rawStatus switch
        {
            var x when IsOneOf(x, "Closed") => ItsmStatus.Closed,
            var x when IsOneOf(x, "Resolved") => ItsmStatus.Resolved,
            var x when IsOneOf(x, "In Progress") => ItsmStatus.InProgress,
            var x when IsOneOf(x, "Open") => ItsmStatus.Open,
            var x when IsOneOf(x, "Pending") => ItsmStatus.Pending,
            _ => ItsmStatus.Unknown
        };
    }

    public static ItsmTicketType MapTicketType(string? rawTicketType)
    {
        return rawTicketType switch
        {
            var x when IsOneOf(x, "Incident") => ItsmTicketType.Incident,
            var x when IsOneOf(x, "Task") => ItsmTicketType.Task,
            var x when IsOneOf(x, "Sub-task") => ItsmTicketType.Subtask,
            _ => ItsmTicketType.Unknown
        };
    }

    public static PriorityLevel MapPriority(string? rawPriority)
    {
        return rawPriority switch
        {
            var x when IsOneOf(x, "P1 (Critical)") => PriorityLevel.Critical,
            var x when IsOneOf(x, "P2 (Urgent)") => PriorityLevel.High,
            var x when IsOneOf(x, "P3 (Normal)") => PriorityLevel.Medium,
            var x when IsOneOf(x, "P4 (Low)") => PriorityLevel.Low,
            _ => PriorityLevel.Unknown
        };
    }

    public static ItsmResolution MapResolution(string? rawResolution)
    {
        return rawResolution switch
        {
            var x when string.IsNullOrWhiteSpace(x) => ItsmResolution.None,
            var x when IsOneOf(x, "Done") => ItsmResolution.Done,
            _ => ItsmResolution.Unknown
        };
    }
}
