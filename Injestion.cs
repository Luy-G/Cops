using FluentValidation;

public class ItsmIngestion
{
    private readonly SogilubJsonDtoValidator _validator = new();

    public ItsmTicket BuildTicket(SogilubJsonDto dto, long clientId)
    {
        var validationResult = _validator.Validate(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException();

        var issueId = Convert.ToInt64(dto.IssueId);

        bool? firstResponseSlaBreached = null;

        if (!string.IsNullOrWhiteSpace(dto.FirstResponseSlaBreached))
            firstResponseSlaBreached = bool.Parse(dto.FirstResponseSlaBreached);

        return new ItsmTicket
        {
            ClientId = clientId,
            SourceSystem = SourceSystem.Jira,

            TicketKey = dto.TicketKey,
            IssueId = issueId,

            Status = ItsmEnumMapper.MapStatus(dto.Status),
            IssueType = ItsmEnumMapper.MapIssueType(dto.IssueType),
            Priority = ItsmEnumMapper.MapPriority(dto.Priority),
            Resolution = ItsmEnumMapper.MapResolution(dto.Resolution),

            Title = dto.Summary,
            Description = dto.Description,
            DescriptionHtml = dto.DescriptionHtml,

            CreatedAt = dto.CreatedAt.UtcDateTime,
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
}