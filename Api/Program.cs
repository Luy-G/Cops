using CyberOps.Application.Contracts.Processing;
using CyberOps.Application.Clients.Sogilub.Registration;
using CyberOps.Application.Processors;
using CyberOps.Application.Scoring;
using CyberOps.Infrastructure.External.Cisa;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProcessor, ClientDataProcessor>();
builder.Services.AddSogilubIngestion();
builder.Services.AddHttpClient<IKevCatalogService, CisaKevCatalogService>();
builder.Services.AddSingleton<IDomain, OperationalSecurityDomain>();
builder.Services.AddSingleton<IDomain, VulnerabilityAndAttackSurfaceDomain>();
builder.Services.AddSingleton<IDomain, ThreatLandscapeDomain>();
builder.Services.AddSingleton<IDomain, DetectionAndResponseDomain>();
builder.Services.AddSingleton<IDomain, IdentityAndAccessSecurityDomain>();
builder.Services.AddSingleton<IDomain, HumanRiskDomain>();
builder.Services.AddSingleton<IDomain, GovernanceAndResilienceDomain>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
