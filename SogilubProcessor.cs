using System.Text.Json;

namespace ControloQualidade.Common.Ingestion;

// implementação do processador para dados Sogilub (Jira ITSM + scan de vulnerabilidades)
// segue o pipeline: fetch → validar → mapear → ingerir → calcular indicadores → calcular scores
public class SogilubProcessor : IProcessor
{
    private readonly SogilubItsmIngestion _itsmIngestion = new();
    private readonly SogilubVulnIngestion _vulnIngestion = new();
    private readonly SogilubJsonDtoValidator _itsmValidator = new();
    private readonly SogilubVulnFindingDtoValidator _vulnValidator = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ProcessorResult> ProcessAsync(ProcessorInput input)
    {
        var errors   = new List<string>();
        var tickets  = new List<Operationalsecitsm>();
        var findings = new List<VulnerabilityAttackSurface>();

        // 1. processar ITSM se vier JSON
        if (!string.IsNullOrWhiteSpace(input.ItsmJson))
        {
            var dtos = DeserializeItsmJson(input.ItsmJson, errors);

            foreach (var dto in dtos)
            {
                // 2. validar o DTO antes de mapear
                var validation = _itsmValidator.Validate(dto);
                if (!validation.IsValid)
                {
                    errors.AddRange(validation.Errors.Select(e => $"ITSM [{dto.Key}]: {e.ErrorMessage}"));
                    continue;
                }

                // 3. mapear DTO → entidade normalizada
                tickets.Add(_itsmIngestion.Map(dto, input.ClientId));
            }
        }

        // 1. processar vulnerabilidades se vier JSON
        if (!string.IsNullOrWhiteSpace(input.VulnJson))
        {
            var report = DeserializeVulnJson(input.VulnJson, errors);

            if (report is not null)
            {
                foreach (var dto in report.vulnerability_findings ?? [])
                {
                    // 2. validar o DTO
                    var validation = _vulnValidator.Validate(dto);
                    if (!validation.IsValid)
                    {
                        errors.AddRange(validation.Errors.Select(e => $"Vuln [{dto.Id}]: {e.ErrorMessage}"));
                        continue;
                    }

                    // 3. mapear DTO → entidade normalizada
                    findings.Add(_vulnIngestion.Map(dto, input.ClientId));
                }
            }
        }

        // 4. construir o contexto com os dados disponíveis
        // (persistência fica fora do processor — responsabilidade do repositório)
        var context = new DomainContext
        {
            ClientId    = input.ClientId,
            ItsmTickets = tickets,
            VulnFindings = findings,
            ItsmCalcs   = input.ItsmCalcs,
            VulnCalcs   = input.VulnCalcs
        };

        // 5. calcular o score de cada domínio ativo
        var domainScores = new Dictionary<DomainKey, decimal>();
        foreach (var domain in input.ActiveDomains)
            domainScores[domain.Key] = domain.Calculate(context);

        // 6. calcular o score composto ponderado
        var composite = CompositeScoreCalculator.Calculate(domainScores);

        return await Task.FromResult(new ProcessorResult
        {
            Success       = errors.Count == 0,
            Errors        = errors,
            ItsmTickets   = tickets,
            VulnFindings  = findings,
            DomainScores  = domainScores,
            CompositeScore = composite
        });
    }

    // tenta deserializar o JSON ITSM — regista o erro e devolve lista vazia se falhar
    private static List<SogilubJsonDto> DeserializeItsmJson(string json, List<string> errors)
    {
        try
        {
            return JsonSerializer.Deserialize<List<SogilubJsonDto>>(json, JsonOptions) ?? [];
        }
        catch (JsonException ex)
        {
            errors.Add($"ITSM JSON inválido: {ex.Message}");
            return [];
        }
    }

    // tenta deserializar o relatório de vulns — regista o erro e devolve null se falhar
    private static SogilubVulnReportDto? DeserializeVulnJson(string json, List<string> errors)
    {
        try
        {
            return JsonSerializer.Deserialize<SogilubVulnReportDto>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            errors.Add($"Vuln JSON inválido: {ex.Message}");
            return null;
        }
    }
}
