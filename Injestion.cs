public class ItsmIngestion
{
    public ItsmTicket BuildTicket(JsonSogilubDto dto, long clientId)
    {
        if (!ItsmDtoValidator.IsValid(dto))
            throw new ArgumentException("Invalid ITSM DTO.");

        return new ItsmTicket
        {
            ClientId = clientId,
            SourceSystem = "jira",

            TicketKey = dto.TicketKey ?? string.Empty,
            IssueId = dto.IssueId ?? string.Empty,

            Status = ItsmEnumMapper.MapStatus(dto.Status),
            IssueType = ItsmEnumMapper.MapIssueType(dto.IssueType),
            Priority = ItsmEnumMapper.MapPriority(dto.Priority),
            Resolution = ItsmEnumMapper.MapResolution(dto.Resolution),

            Title = dto.Summary,
            DescriptionText = dto.Description,
            DescriptionHtml = dto.DescriptionHtml,

            CreatedAt = dto.CreatedAt!.Value,
            ResolvedAt = dto.ResolvedAt,
            UpdatedAt = dto.UpdatedAt,

            CreatorName = dto.CreatorName,
            CreatorEmail = dto.CreatorEmail,

            CurrentAssigneeName = dto.AssigneeName,
            CurrentAssigneeEmail = dto.AssigneeEmail,

            ReporterName = dto.ReporterName,
            ReporterEmail = dto.ReporterEmail,

            FirstResponseDurationText = dto.FirstResponseDurationText,
            FirstResponseDurationMs = dto.FirstResponseDurationMs,
            FirstResponseSlaStartAt = dto.FirstResponseSlaStartAt,
            FirstResponseSlaCompleteAt = dto.FirstResponseSlaCompleteAt,
            FirstResponseSlaBreached = dto.FirstResponseSlaBreached,

            TimeSpentHours = dto.TimeSpentHours
        };
    }
}