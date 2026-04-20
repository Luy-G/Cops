using System.Text.Json;
using System.Text.Json.Serialization;

public class CisaKevEntry
{
    [JsonPropertyName("cveID")]
    public string? CveId { get; set; }
}

public class CisaKevResponse
{
    [JsonPropertyName("vulnerabilities")]
    public List<CisaKevEntry>? Vulnerabilities { get; set; }
}

public interface IKevCatalogService
{
    Task<IReadOnlySet<string>> GetKevCveIdsAsync(CancellationToken ct = default);
}

public class CisaKevCatalogService : IKevCatalogService
{
    private const string KevUrl = "https://www.cisa.gov/sites/default/files/feeds/known_exploited_vulnerabilities.json";

    private readonly HttpClient _http;

    public CisaKevCatalogService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlySet<string>> GetKevCveIdsAsync(CancellationToken ct = default)
    {
        var response = await _http.GetStringAsync(KevUrl, ct);
        var catalog = JsonSerializer.Deserialize<CisaKevResponse>(response);

        return (catalog?.Vulnerabilities ?? [])
            .Where(e => !string.IsNullOrWhiteSpace(e.CveId))
            .Select(e => e.CveId!.Trim().ToUpperInvariant())
            .ToHashSet();
    }
}
