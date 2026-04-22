using System.Text.Json;
using CyberOps.Application.Clients.Sogilub;
using CyberOps.Application.Ingestion.Shared;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Clients.Sogilub.Itsm;

public class SogilubItsmAdapter : IItsmIngestionAdapter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly SogilubItsmDtoValidator _validator = new();

    public bool CanHandle(long clientId) => clientId == SogilubClient.ClientId;

    public IngestionResult<Operationalsecitsm> Ingest(string json, long clientId)
    {
        var errors = new List<string>();
        var items = new List<Operationalsecitsm>();
        var dtos = Deserialize(json, errors);

        foreach (var dto in dtos)
        {
            var validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                errors.AddRange(validation.Errors.Select(e => $"ITSM [{dto.Key}]: {e.ErrorMessage}"));
                continue;
            }

            items.Add(Map(dto, clientId));
        }

        return new IngestionResult<Operationalsecitsm>
        {
            Items = items,
            Errors = errors
        };
    }

    private static List<SogilubItsmDto> Deserialize(string json, List<string> errors)
    {
        try
        {
            return JsonSerializer.Deserialize<List<SogilubItsmDto>>(json, JsonOptions) ?? [];
        }
        catch (JsonException ex)
        {
            errors.Add($"ITSM invalid JSON: {ex.Message}");
            return [];
        }
    }

    private static Operationalsecitsm Map(SogilubItsmDto dto, long clientId)
    {
        var firstResponseSlaBreached = ParseNullableBoolean(dto.FirstResponseSlaBreached);

        return new Operationalsecitsm
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
            CreatedAt = dto.Created!.Value.UtcDateTime,
            ResolvedAt = dto.Resolved?.UtcDateTime,
            UpdatedAt = dto.Updated?.UtcDateTime,
            CreatorName = dto.CreatorName,
            CreatorEmail = dto.CreatorEmail,
            CurrentAssigneeName = dto.AssigneeName,
            CurrentAssigneeEmail = dto.AssigneeEmail,
            ReporterName = dto.ReporterName,
            ReporterEmail = dto.ReporterEmail,
            FirstResponseDurationText = dto.FirstResponseDurationText,
            FirstResponseDurationMs = dto.FirstResponseDurationMs.HasValue ? (long)dto.FirstResponseDurationMs.Value : null,
            FirstResponseSlaStartAt = dto.FirstResponseSlaStartAt?.UtcDateTime,
            FirstResponseSlaCompleteAt = dto.FirstResponseSlaCompleteAt?.UtcDateTime,
            FirstResponseSlaBreached = firstResponseSlaBreached,
            TimeSpentHours = dto.TimeSpentHours
        };
    }

    private static bool? ParseNullableBoolean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "true" => true,
            "false" => false,
            _ => null
        };
    }
}
