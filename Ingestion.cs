namespace ControloQualidade.Common.Ingestion;

public class ItsmIngestion
{
    public ItsmTicket Map(SogilubJsonDto dto, long clientId)
    {
        var firstResponseSlaBreached = ParseNullableBoolean(dto.FirstResponseSlaBreached);

        return new ItsmTicket
        {
            ClientId = clientId,
            SourceSystem = SourceSystem.Jira,

            TicketKey =  dto.TicketKey!,
            IssueId = Convert.ToInt64(dto.IssueId!.Value),

            Status = SogilubItsmMapper.MapStatus(dto.Status),
            TicketType = SogilubItsmMapper.MapTicketType(dto.IssueType),
            Priority = SogilubItsmMapper.MapPriority(dto.Priority),
            Resolution = SogilubItsmMapper.MapResolution(dto.Resolution),

            Title = dto.Summary!,
            Description = dto.Description,
            DescriptionHtml = dto.DescriptionHtml,

            CreatedAt = dto.CreatedAt!.Value.UtcDateTime,
            ResolvedAt = dto.ResolvedAt?.UtcDateTime,
            UpdatedAt = dto.UpdatedAt?.UtcDateTime,

            CreatorName = dto.CreatorName,
            CreatorEmail = dto.CreatorEmail,

            CurrentAssigneeName = dto.AssigneeName,
            CurrentAssigneeEmail = dto.AssigneeEmail,

            ReporterName = dto.ReporterName,
            ReporterEmail = dto.ReporterEmail,

            FirstResponseDurationText = dto.FirstResponseDurationText,
            FirstResponseDurationMs = dto.FirstResponseDurationMs,
            FirstResponseSlaStartAt = dto.FirstResponseSlaStartAt?.UtcDateTime,
            FirstResponseSlaCompleteAt = dto.FirstResponseSlaCompleteAt?.UtcDateTime,
            FirstResponseSlaBreached = firstResponseSlaBreached,

            TimeSpentHours = dto.TimeSpentHours
        };
    }

    private static bool? ParseNullableBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().ToLowerInvariant() switch
        {
            "true" => true,
            "false" => false,
            _ => null
        };
    }
}