using FluentValidation;

public class ItsmIngestion
{
    private readonly SogilubJsonDtoValidator _validator = new();

    public ItsmTicket BuildTicket(SogilubJsonDto dto, long clientId)
    {
        var validationResult = _validator.Validate(dto);

        if (!validationResult.IsValid)
            throw new ArgumentException();

        var issueId = long.Parse(dto.IssueId);

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

            CreatedAt = dto.CreatedAt,
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