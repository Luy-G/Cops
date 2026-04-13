public static class ItsmEnumMapper
{
    public static ItsmStatus MapStatus(string? rawStatus)
    {
        rawStatus = rawStatus?.Trim();

        if (string.IsNullOrWhiteSpace(rawStatus))
            return ItsmStatus.Unknown;

        if (rawStatus.Equals("New", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.New;

        if (rawStatus.Equals("Open", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Open;

        if (rawStatus.Equals("Triaged", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Triaged;

        if (rawStatus.Equals("In Progress", StringComparison.OrdinalIgnoreCase) ||
            rawStatus.Equals("InProgress", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.InProgress;

        if (rawStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Pending;

        if (rawStatus.Equals("On Hold", StringComparison.OrdinalIgnoreCase) ||
            rawStatus.Equals("OnHold", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.OnHold;

        if (rawStatus.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Resolved;

        if (rawStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Closed;

        if (rawStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) ||
            rawStatus.Equals("Canceled", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Cancelled;

        if (rawStatus.Equals("Reopened", StringComparison.OrdinalIgnoreCase))
            return ItsmStatus.Reopened;

        return ItsmStatus.Unknown;
    }

    public static ItsmIssueType MapIssueType(string? rawIssueType)
    {
        rawIssueType = rawIssueType?.Trim();

        if (string.IsNullOrWhiteSpace(rawIssueType))
            return ItsmIssueType.Unknown;

        if (rawIssueType.Equals("Incident", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Incident;

        if (rawIssueType.Equals("Task", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Task;

        if (rawIssueType.Equals("Sub-task", StringComparison.OrdinalIgnoreCase) ||
            rawIssueType.Equals("Subtask", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Subtask;

        if (rawIssueType.Equals("Service Request", StringComparison.OrdinalIgnoreCase) ||
            rawIssueType.Equals("ServiceRequest", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.ServiceRequest;

        if (rawIssueType.Equals("Problem", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Problem;

        if (rawIssueType.Equals("Change", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Change;

        if (rawIssueType.Equals("Case", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Case;

        if (rawIssueType.Equals("Other", StringComparison.OrdinalIgnoreCase))
            return ItsmIssueType.Other;

        return ItsmIssueType.Unknown;
    }

    public static PriorityLevel MapPriority(string? rawPriority)
    {
        rawPriority = rawPriority?.Trim();

        if (string.IsNullOrWhiteSpace(rawPriority))
            return PriorityLevel.Unknown;

        if (rawPriority.Equals("P1", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("P1 (Critical)", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("Critical", StringComparison.OrdinalIgnoreCase))
            return PriorityLevel.Critical;

        if (rawPriority.Equals("P2", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("P2 (Urgent)", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("High", StringComparison.OrdinalIgnoreCase))
            return PriorityLevel.High;

        if (rawPriority.Equals("P3", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("P3 (Normal)", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("Medium", StringComparison.OrdinalIgnoreCase))
            return PriorityLevel.Medium;

        if (rawPriority.Equals("P4", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("P4 (Low)", StringComparison.OrdinalIgnoreCase) ||
            rawPriority.Equals("Low", StringComparison.OrdinalIgnoreCase))
            return PriorityLevel.Low;

        return PriorityLevel.Unknown;
    }

    public static ItsmResolution MapResolution(string? rawResolution)
    {
        rawResolution = rawResolution?.Trim();

        if (string.IsNullOrWhiteSpace(rawResolution))
            return ItsmResolution.None;

        if (rawResolution.Equals("Done", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Done;

        if (rawResolution.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Resolved;

        if (rawResolution.Equals("Fixed", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Fixed;

        if (rawResolution.Equals("Workaround", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Workaround;

        if (rawResolution.Equals("Duplicate", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Duplicate;

        if (rawResolution.Equals("Won't Fix", StringComparison.OrdinalIgnoreCase) ||
            rawResolution.Equals("Wont Fix", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.WontFix;

        if (rawResolution.Equals("Not Reproducible", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.NotReproducible;

        if (rawResolution.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) ||
            rawResolution.Equals("Canceled", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Cancelled;

        if (rawResolution.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            return ItsmResolution.Rejected;

        return ItsmResolution.Unknown;
    }
}