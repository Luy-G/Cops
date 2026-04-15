using System.Linq;

public static class ItsmEnumMapper
{

    public static ItsmStatus MapStatus(string rawStatus)
    {
        ItsmStatus result = rawStatus switch
        {
            var x when IsOneOf(x, "New") => ItsmStatus.New,
            var x when IsOneOf(x, "Open") => ItsmStatus.Open,
            var x when IsOneOf(x, "Triaged") => ItsmStatus.Triaged,
            var x when IsOneOf(x, "In Progress", "InProgress") => ItsmStatus.InProgress,
            var x when IsOneOf(x, "Pending") => ItsmStatus.Pending,
            var x when IsOneOf(x, "On Hold", "OnHold") => ItsmStatus.OnHold,
            var x when IsOneOf(x, "Resolved") => ItsmStatus.Resolved,
            var x when IsOneOf(x, "Closed") => ItsmStatus.Closed,
            var x when IsOneOf(x, "Cancelled", "Canceled") => ItsmStatus.Cancelled,
            var x when IsOneOf(x, "Reopened") => ItsmStatus.Reopened,
            _ => ItsmStatus.Unknown
        };

        return result;
    }

    public static ItsmIssueType MapIssueType(string rawIssueType)
    {
        ItsmIssueType result = rawIssueType switch
        {
            var x when IsOneOf(x, "Incident") => ItsmIssueType.Incident,
            var x when IsOneOf(x, "Task") => ItsmIssueType.Task,
            var x when IsOneOf(x, "Sub-task", "Subtask") => ItsmIssueType.Subtask,
            var x when IsOneOf(x, "Service Request", "ServiceRequest") => ItsmIssueType.ServiceRequest,
            var x when IsOneOf(x, "Problem") => ItsmIssueType.Problem,
            var x when IsOneOf(x, "Change") => ItsmIssueType.Change,
            var x when IsOneOf(x, "Case") => ItsmIssueType.Case,
            var x when IsOneOf(x, "Other") => ItsmIssueType.Other,
            _ => ItsmIssueType.Unknown
        };

        return result;
    }

    public static PriorityLevel MapPriority(string rawPriority)
    {
        PriorityLevel result = rawPriority switch
        {
            var x when IsOneOf(x, "P1", "P1 (Critical)", "Critical") => PriorityLevel.Critical,
            var x when IsOneOf(x, "P2", "P2 (Urgent)", "High") => PriorityLevel.High,
            var x when IsOneOf(x, "P3", "P3 (Normal)", "Medium") => PriorityLevel.Medium,
            var x when IsOneOf(x, "P4", "P4 (Low)", "Low") => PriorityLevel.Low,
            _ => PriorityLevel.Unknown
        };

        return result;
    }

    public static ItsmResolution MapResolution(string rawResolution)
    {
        ItsmResolution result = rawResolution switch
        {
            var x when string.IsNullOrWhiteSpace(x) => ItsmResolution.None,
            var x when IsOneOf(x, "Done") => ItsmResolution.Done,
            var x when IsOneOf(x, "Resolved") => ItsmResolution.Resolved,
            var x when IsOneOf(x, "Fixed") => ItsmResolution.Fixed,
            var x when IsOneOf(x, "Workaround") => ItsmResolution.Workaround,
            var x when IsOneOf(x, "Duplicate") => ItsmResolution.Duplicate,
            var x when IsOneOf(x, "Won't Fix", "Wont Fix") => ItsmResolution.WontFix,
            var x when IsOneOf(x, "Not Reproducible") => ItsmResolution.NotReproducible,
            var x when IsOneOf(x, "Cancelled", "Canceled") => ItsmResolution.Cancelled,
            var x when IsOneOf(x, "Rejected") => ItsmResolution.Rejected,
            _ => ItsmResolution.Unknown
        };

        return result;
    }



        private static bool IsOneOf(string value, params string[] options)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim();

        return options.Any(option => normalized.Equals(option, StringComparison.OrdinalIgnoreCase));
    }
}