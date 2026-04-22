using System.Text.Json;
using CyberOps.Application.Clients.Sogilub;
using CyberOps.Application.Ingestion.Shared;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Clients.Sogilub.Vuln;

public class SogilubVulnAdapter : IVulnIngestionAdapter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly SogilubVulnFindingDtoValidator _validator = new();

    public bool CanHandle(long clientId) => clientId == SogilubClient.ClientId;

    public IngestionResult<VulnerabilityAttackSurface> Ingest(string json, long clientId)
    {
        var errors = new List<string>();
        var items = new List<VulnerabilityAttackSurface>();
        var report = Deserialize(json, errors);

        if (report is null)
        {
            return new IngestionResult<VulnerabilityAttackSurface>
            {
                Items = items,
                Errors = errors
            };
        }

        foreach (var dto in report.vulnerability_findings ?? [])
        {
            var validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                errors.AddRange(validation.Errors.Select(e => $"Vuln [{dto.Id}]: {e.ErrorMessage}"));
                continue;
            }

            items.Add(Map(dto, clientId));
        }

        return new IngestionResult<VulnerabilityAttackSurface>
        {
            Items = items,
            Errors = errors
        };
    }

    private static SogilubVulnReportDto? Deserialize(string json, List<string> errors)
    {
        try
        {
            return JsonSerializer.Deserialize<SogilubVulnReportDto>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            errors.Add($"Vuln invalid JSON: {ex.Message}");
            return null;
        }
    }

    private static VulnerabilityAttackSurface Map(SogilubVulnFindingDto dto, long clientId, IReadOnlySet<string>? kevCveIds = null)
    {
        var cve = dto.Metadata?.Cve?.Trim().ToUpperInvariant();
        var isInKev = cve is not null && kevCveIds is not null && kevCveIds.Contains(cve);

        return new VulnerabilityAttackSurface
        {
            ClientId = clientId,
            FindingKey = dto.Id!,
            SourceSystem = SourceSystem.Other,
            ScanEngine = ScanEngine.Unknown,
            Severity = SogilubVulnMapper.MapSeverity(dto.Severity),
            Title = dto.Title!,
            Cve = dto.Metadata?.Cve,
            Cvss = SogilubVulnMapper.ParseCvss(dto.Metadata?.Cvss),
            Host = dto.Metadata?.Host,
            Port = dto.Metadata?.Port,
            Evidence = dto.Metadata?.Status,
            Description = dto.Description,
            Impact = dto.Impact,
            Recommendation = dto.Recommendation,
            HasPublicExploit = false,
            IsInternetExposed = false,
            IsInKevCatalog = isInKev
        };
    }
}
