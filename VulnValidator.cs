using FluentValidation;

public class SogilubVulnFindingDtoValidator : AbstractValidator<SogilubVulnFindingDto>
{
    public SogilubVulnFindingDtoValidator()
    {
        //obrigatório
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("FINDING_KEY_REQUIRED");

        //obrigatório — usado nos cálculos de risco
        RuleFor(x => x.Severity)
            .NotEmpty()
            .WithErrorCode("SEVERITY_REQUIRED")
            .Must(x => new[] { "CRITICAL", "HIGH", "MEDIUM", "LOW", "INFO" }
                .Contains(x?.Trim().ToUpperInvariant()))
            .WithErrorCode("SEVERITY_INVALID");

        //obrigatório — identifica a vuln
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithErrorCode("TITLE_REQUIRED");

        // se existir e não for "N/A", o CVSS tem de ser um número entre 0.0 e 10.0
        RuleFor(x => x.Metadata!.Cvss)
            .Must(cvss =>
            {
                if (string.IsNullOrWhiteSpace(cvss) || cvss.Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    return true;
                return decimal.TryParse(cvss, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var score)
                    && score >= 0m && score <= 10m;
            })
            .When(x => x.Metadata != null)
            .WithErrorCode("CVSS_INVALID");
    }
}
