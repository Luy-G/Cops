using System.Text.Json;

namespace CyberOps.Infrastructure.External.Cisa;

public class CisaKevEntry
{
    public string? cveID { get; set; }
}

public class CisaKevResponse
{
    public List<CisaKevEntry>? vulnerabilities { get; set; }
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

        return (catalog?.vulnerabilities ?? [])
            .Where(entry => !string.IsNullOrWhiteSpace(entry.cveID))
            .Select(entry => entry.cveID!.Trim().ToUpperInvariant())
            .ToHashSet();
    }
}
