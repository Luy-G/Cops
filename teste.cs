using System;

var dto = new SogilubJsonDto
{
    TicketKey = "",
    IssueId = "",
    Status = "Closed",
    IssueType = "Incident",
    Priority = "P1",
    Resolution = "done",
    Summary = "Falha de rede",
    Description = "Sem conectividade.",
    CreatedAt = new DateTime(2026, 4, 10, 9, 30, 0),
    ResolvedAt = new DateTime(2026, 4, 10, 11, 30, 0),
    UpdatedAt = new DateTime(2026, 4, 10, 11, 35, 0),
    CreatorName = "Rodrigo Alves",
    CreatorEmail = "rodrigo@example.com",
    AssigneeName = "Technology - Support",
    AssigneeEmail = "support@example.com",
    ReporterName = "Rodrigo Alves",
    ReporterEmail = "rodrigo@example.com",
    FirstResponseDurationText = "59m",
    FirstResponseDurationMs = 3540000,
    FirstResponseSlaStartAt = new DateTime(2026, 4, 10, 9, 31, 0),
    FirstResponseSlaCompleteAt = new DateTime(2026, 4, 10, 10, 30, 0),
    FirstResponseSlaBreached = false,
    TimeSpentHours = 2.0m
};

Console.WriteLine(ItsmDtoValidator.IsValid(dto));

var ingestion = new ItsmIngestion();
var ticket = ingestion.BuildTicket(dto, 1);

Console.WriteLine(ticket.ClientId);
Console.WriteLine(ticket.SourceSystem);
Console.WriteLine(ticket.TicketKey);
Console.WriteLine(ticket.IssueId);
Console.WriteLine(ticket.Status);
Console.WriteLine(ticket.IssueType);
Console.WriteLine(ticket.Priority);
Console.WriteLine(ticket.Resolution);
Console.WriteLine(ticket.Title);
Console.WriteLine(ticket.CreatedAt);
Console.WriteLine(ticket.TimeSpentHours);
