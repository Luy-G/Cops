var dto = new SogilubJsonDto
{
    TicketKey = "SR-8205",
    IssueId = 74525.0,
    Status = "Closed",
    IssueType = "Incident",
    Priority = "P1 (Critical)",
    Resolution = "Done",
    Summary = "teste summary",
    Description = "teste descriçao.",
    CreatedAt = new DateTime(2026, 4, 10, 9, 30, 0),
    ResolvedAt = new DateTime(2026, 4, 10, 11, 30, 0),
    UpdatedAt = new DateTime(2026, 4, 10, 11, 35, 0),
    CreatorName = "teste teste creator",
    CreatorEmail = "teste@creator.com",
    AssigneeName = "Technology - Support",
    AssigneeEmail = "teste@assignee.com",
    ReporterName = "teste teste reporter",
    ReporterEmail = "testeo@reporter.com",
    FirstResponseDurationText = "59m",
    FirstResponseDurationMs = 3540000,
    FirstResponseSlaStartAt = new DateTime(2026, 4, 10, 9, 31, 0),
    FirstResponseSlaCompleteAt = new DateTime(2026, 4, 10, 10, 30, 0),
    FirstResponseSlaBreached = "false",
    TimeSpentHours = 2.0m
};

var ingestion = new ItsmIngestion();
var ticket = ingestion.BuildTicket(dto, 1);

Console.WriteLine(ticket.TicketKey);
Console.WriteLine(ticket.IssueId);
Console.WriteLine(ticket.Status);
Console.WriteLine(ticket.Priority);
Console.WriteLine(ticket.Resolution);
Console.WriteLine(ticket.Title);

var tickets = new List<ItsmTicket>
{
    ticket,
    new ItsmTicket
    {
        ClientId = 1,
        SourceSystem = SourceSystem.Jira,
        TicketKey = "SR-8206",
        IssueId = 74526,
        Status = ItsmStatus.Closed,
        IssueType = ItsmIssueType.Incident,
        Priority = PriorityLevel.High,
        Resolution = ItsmResolution.Done,
        Title = "Outro ticket",
        CreatedAt = new DateTime(2026, 4, 10, 12, 0, 0),
        TimeSpentHours = 4.0m,
        FirstResponseSlaBreached = true
    },
    new ItsmTicket
    {
        ClientId = 1,
        SourceSystem = SourceSystem.Jira,
        TicketKey = "SR-8207",
        IssueId = 74527,
        Status = ItsmStatus.InProgress,
        IssueType = ItsmIssueType.Incident,
        Priority = PriorityLevel.Medium,
        Resolution = ItsmResolution.None,
        Title = "Ticket aberto",
        CreatedAt = new DateTime(2026, 4, 11, 10, 0, 0)
    }
};

var threshold = new ClientItsmThreshold
{
    ClientId = 1,
    OpenTickets100Max = 10,
    OpenTickets70Max = 50,
    MttrTargetHours = 24
};


Console.WriteLine($"MTTR: {ItsmCalculations.CalculateMttr(tickets)}");
Console.WriteLine($"MTTR Score: {ItsmCalculations.CalculateMttrScore(tickets, threshold)}");
Console.WriteLine($"SLA Compliance: {ItsmCalculations.CalculateSlaCompliance(tickets)}");
Console.WriteLine($"Open Tickets: {ItsmCalculations.CalculateOpenTickets(tickets)}");
Console.WriteLine($"Open Tickets Score: {ItsmCalculations.CalculateOpenTicketsScore(tickets, threshold)}");
Console.WriteLine($"Open Tickets Weighted: {ItsmCalculations.CalculateOpenTicketsWeighted(tickets, threshold)}");
Console.WriteLine($"MTTR Weighted: {ItsmCalculations.CalculateMttrWeighted(tickets, threshold)}");
Console.WriteLine($"SLA Weighted: {ItsmCalculations.CalculateSlaComplianceWeighted(tickets)}");
Console.WriteLine($"Total: {ItsmCalculations.CalculateOperationalSecurityItsmTotal(tickets, threshold)}");