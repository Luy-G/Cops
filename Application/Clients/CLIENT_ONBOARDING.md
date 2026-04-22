# Client Onboarding Pattern

Use this structure for every new client integration:

1. Create `Application/Clients/<ClientName>/`.
2. Add `<ClientName>Client.cs` with a stable `ClientId` constant.
3. Add ITSM adapter files under `Itsm/`:
   - `<ClientName>ItsmDto.cs`
   - `<ClientName>ItsmValidator.cs`
   - `<ClientName>ItsmMapper.cs`
   - `<ClientName>ItsmAdapter.cs` implementing `IItsmIngestionAdapter`
4. Add vulnerability adapter files under `Vuln/`:
   - `<ClientName>VulnDto.cs`
   - `<ClientName>VulnValidator.cs`
   - `<ClientName>VulnMapper.cs`
   - `<ClientName>VulnAdapter.cs` implementing `IVulnIngestionAdapter`
5. Add `Registration/<ClientName>ServiceCollectionExtensions.cs` to register adapters in DI.
6. Call `builder.Services.Add<ClientName>Ingestion()` in API composition root.

Important rules:
- Keep normalized entities in `Domain/Entities` unchanged (`Operationalsecitsm`, `VulnerabilityAttackSurface`).
- Keep scoring logic in `Application/Scoring` client-agnostic.
- Put only client/source-specific JSON DTO/validation/mapping in client folders.
