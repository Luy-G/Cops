using FluentValidation;

public class SogilubVulnFindingDtoValidator : AbstractValidator<SogilubVulnFindingDto>
{
    public SogilubVulnFindingDtoValidator()
    {
        //obrigatório
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("FINDING_ID_REQUIRED");

        //obrigatório usado depois para os cálculos de risco
        RuleFor(x => x.Severity)
            .NotEmpty()
            .WithErrorCode("SEVERITY_REQUIRED")
            .Must(x => new[] { "CRITICAL", "HIGH", "MEDIUM", "LOW", "INFO" }
                .Contains(x?.Trim().ToUpperInvariant()))
            .WithErrorCode("SEVERITY_INVALID");

        //obrigatório identifica a vuln
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithErrorCode("TITLE_REQUIRED");

        // o CVSS se existir tem de ser um número entre 0.0 e 10.0
        RuleFor(x => x.Metadata!.Cvss)
            .Must(cvss =>
            {
                if (string.IsNullOrWhiteSpace(cvss)) return true;
                return decimal.TryParse(cvss, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var score)
                    && score >= 0m && score <= 10m;
            })
            .When(x => x.Metadata != null)
            .WithErrorCode("CVSS_INVALID");
    }
}
