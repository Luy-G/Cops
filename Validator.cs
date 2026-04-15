using FluentValidation;

public class SogilubJsonDtoValidator : AbstractValidator<SogilubJsonDto>
{
    public SogilubJsonDtoValidator()
    {
        RuleFor(x => x.TicketKey).NotEmpty();
        RuleFor(x => x.IssueId).NotEmpty();

        RuleFor(x => x.Status).NotEmpty();
        RuleFor(x => x.IssueType).NotEmpty();
        RuleFor(x => x.Priority).NotEmpty();
        RuleFor(x => x.Resolution).NotEmpty();

        RuleFor(x => x.Summary).NotEmpty();

        RuleFor(x => x.CreatedAt).NotEmpty();

        RuleFor(x => x.IssueId)
            .GreaterThan(0);

        RuleFor(x => x.TimeSpentHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TimeSpentHours.HasValue);

        RuleFor(x => x.FirstResponseDurationMs)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FirstResponseDurationMs.HasValue);

        RuleFor(x => x.ResolvedAt)
            .GreaterThanOrEqualTo(x => x.CreatedAt)
            .When(x => x.ResolvedAt.HasValue);

        RuleFor(x => x.UpdatedAt)
            .GreaterThanOrEqualTo(x => x.CreatedAt)
            .When(x => x.UpdatedAt.HasValue);
    }
}