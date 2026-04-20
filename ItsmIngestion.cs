namespace ControloQualidade.Common.Ingestion;
public class SogilubItsmIngestion
{
    public ItsmTicket Map(SogilubJsonDto dto, long clientId)
    {
        //SlaBreached vem como texto do jira passamos para boolean
        var firstResponseSlaBreached = ParseNullableBoolean(dto.FirstResponseSlaBreached);

        return new ItsmTicket
        {
            ClientId = clientId,

            SourceSystem = SourceSystem.Jira,

            TicketKey = dto.Key!,
            IssueId = Convert.ToInt64(dto.IssueId!.Value),

            Status = SogilubItsmMapper.MapStatus(dto.Status),
            TicketType = SogilubItsmMapper.MapTicketType(dto.IssueType),
            Priority = SogilubItsmMapper.MapPriority(dto.Priority),
            Resolution = SogilubItsmMapper.MapResolution(dto.Resolution),

            Title = dto.Summary!,
            Description = dto.Description,
            DescriptionHtml = dto.DescriptionHtml,

            //convertemos para UTC para consistencia
            CreatedAt = dto.Created!.Value.UtcDateTime,
            ResolvedAt = dto.Resolved?.UtcDateTime,
            UpdatedAt = dto.Updated?.UtcDateTime,

            CreatorName = dto.CreatorName,
            CreatorEmail = dto.CreatorEmail,
            CurrentAssigneeName = dto.AssigneeName,
            CurrentAssigneeEmail = dto.AssigneeEmail,
            ReporterName = dto.ReporterName,
            ReporterEmail = dto.ReporterEmail,

            //dados do SLA de primeira resposta
            FirstResponseDurationText = dto.FirstResponseDurationText,
            // jira envia em double guardamos em long em ms
            FirstResponseDurationMs = dto.FirstResponseDurationMs.HasValue ? (long)dto.FirstResponseDurationMs.Value : null,
            FirstResponseSlaStartAt = dto.FirstResponseSlaStartAt?.UtcDateTime,
            FirstResponseSlaCompleteAt = dto.FirstResponseSlaCompleteAt?.UtcDateTime,
            FirstResponseSlaBreached = firstResponseSlaBreached,

            // horas gastas no ticket(calcular o MTTR)
            TimeSpentHours = dto.TimeSpentHours
        };
    }

    // passar o texto "true"/"false" para boolean
    // da null se vier vazio(SLA ainda não foi avaliado)
    private static bool? ParseNullableBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().ToLowerInvariant() switch
        {
            "true"  => true,
            "false" => false,
            _       => null
        };
    }
}
