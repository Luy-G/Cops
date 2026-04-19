using FluentValidation;
public class SogilubJsonDtoValidator : AbstractValidator<SogilubJsonDto>
{
    public SogilubJsonDtoValidator()
    {
        //obrigatória
        RuleFor(x => x.TicketKey)
            .NotEmpty()
            .WithErrorCode("TICKET_KEY_REQUIRED");

        // obrigatório e tem de ser maior que zero

        RuleFor(x => x.IssueId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode("ISSUE_ID_REQUIRED")
            .GreaterThan(0)
            .WithErrorCode("ISSUE_ID_INVALID");

        //obrigatório
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithErrorCode("STATUS_REQUIRED");

        //obrigatório
        RuleFor(x => x.IssueType)
            .NotEmpty()
            .WithErrorCode("ISSUE_TYPE_REQUIRED");

        //obrigatória
        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithErrorCode("PRIORITY_REQUIRED");

        //obrigatório
        RuleFor(x => x.Summary)
            .NotEmpty()
            .WithErrorCode("SUMMARY_REQUIRED");

        //obrigatória (calcular o MTTR e ordenar tickets)
        RuleFor(x => x.CreatedAt)
            .NotNull()
            .WithErrorCode("CREATED_AT_REQUIRED");

        //se existirem tem de ser zero ou positivas
        RuleFor(x => x.TimeSpentHours)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TimeSpentHours.HasValue)
            .WithErrorCode("TIME_SPENT_HOURS_INVALID");

        // se existir tem de ser zero ou positivo
        RuleFor(x => x.FirstResponseDurationMs)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FirstResponseDurationMs.HasValue)
            .WithErrorCode("FIRST_RESPONSE_DURATION_MS_INVALID");

        //se existir tem de ser depois da data de criação
        RuleFor(x => x.ResolvedAt)
            .GreaterThan(x => x.CreatedAt!.Value)
            .When(x => x.ResolvedAt.HasValue && x.CreatedAt.HasValue)
            .WithErrorCode("RESOLVED_AT_INVALID");

        //se existirtem de ser igual ou depois da data de criação
        RuleFor(x => x.UpdatedAt)
            .GreaterThanOrEqualTo(x => x.CreatedAt!.Value)
            .When(x => x.UpdatedAt.HasValue && x.CreatedAt.HasValue)
            .WithErrorCode("UPDATED_AT_INVALID");

        //se existir só pode ser "true" ou "false" em texto
        RuleFor(x => x.FirstResponseSlaBreached)
            .Must(x => x == null || x == "true" || x == "false")
            .WithErrorCode("FIRST_RESPONSE_SLA_BREACHED_INVALID");
    }
}
