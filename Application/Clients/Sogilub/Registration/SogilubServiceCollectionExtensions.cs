using CyberOps.Application.Clients.Sogilub.Itsm;
using CyberOps.Application.Clients.Sogilub.Vuln;
using CyberOps.Application.Ingestion.Shared;

namespace CyberOps.Application.Clients.Sogilub.Registration;

public static class SogilubServiceCollectionExtensions
{
    public static IServiceCollection AddSogilubIngestion(this IServiceCollection services)
    {
        services.AddScoped<IItsmIngestionAdapter, SogilubItsmAdapter>();
        services.AddScoped<IVulnIngestionAdapter, SogilubVulnAdapter>();
        return services;
    }
}
