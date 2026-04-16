using FluentValidation;
public class SogilubJsonDtoValidator : AbstractValidator<SogilubJsonDto>
{
    public SogilubJsonDtoValidator()
    {
        RuleFor(x => x.TicketKey)
            .NotEmpty()
            .WithMessage("TicketKey is required.");

        RuleFor(x => x.IssueId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("IssueId is required.")
            .GreaterThan(0)
            .WithMessage("IssueId must be greater than 0.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.");

        RuleFor(x => x.IssueType)
            .NotEmpty()
            .WithMessage("IssueType is required.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Priority is required.");

        RuleFor(x => x.Resolution)
            .NotEmpty()
            .WithMessage("Resolution is required.");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithMessage("Summary is required.");

        RuleFor(x => x.CreatedAt)
            .NotNull()
            .WithMessage("CreatedAt is required.");

        RuleFor(x => x.TimeSpentHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TimeSpentHours.HasValue)
            .WithMessage("TimeSpentHours cannot be negative.");

        RuleFor(x => x.FirstResponseDurationMs)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FirstResponseDurationMs.HasValue)
            .WithMessage("FirstResponseDurationMs cannot be negative.");

        RuleFor(x => x.ResolvedAt)
            .GreaterThan(x => x.CreatedAt!.Value)
            .When(x => x.ResolvedAt.HasValue && x.CreatedAt.HasValue)
            .WithMessage("ResolvedAt cannot be earlier than CreatedAt.");

        RuleFor(x => x.UpdatedAt)
            .GreaterThanOrEqualTo(x => x.CreatedAt!.Value)
            .When(x => x.UpdatedAt.HasValue && x.CreatedAt.HasValue)
            .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");

        RuleFor(x => x.FirstResponseSlaBreached)
            .Must(x => x == null || x == "true" || x == "false")
            .WithMessage("FirstResponseSlaBreached must be 'true', 'false' or null.");
    }
}