using FluentValidation;

namespace CyberOps.Application.Clients.Sogilub.Vuln;

public class SogilubVulnFindingDtoValidator : AbstractValidator<SogilubVulnFindingDto>
{
    public SogilubVulnFindingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode("FINDING_KEY_REQUIRED");

        RuleFor(x => x.Severity)
            .NotEmpty().WithErrorCode("SEVERITY_REQUIRED")
            .Must(x => new[] { "CRITICAL", "HIGH", "MEDIUM", "LOW", "INFO" }.Contains(x?.Trim().ToUpperInvariant()))
            .WithErrorCode("SEVERITY_INVALID");

        RuleFor(x => x.Title).NotEmpty().WithErrorCode("TITLE_REQUIRED");

        RuleFor(x => x.Metadata!.Cvss)
            .Must(cvss =>
            {
                if (string.IsNullOrWhiteSpace(cvss) || cvss.Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return decimal.TryParse(
                    cvss,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var score)
                    && score >= 0m
                    && score <= 10m;
            })
            .When(x => x.Metadata != null)
            .WithErrorCode("CVSS_INVALID");
    }
}
