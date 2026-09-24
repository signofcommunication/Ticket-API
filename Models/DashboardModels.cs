namespace Ticket_API.Models;

public sealed record TicketReply(
    string Id,
    string Author,
    string Message,
    DateTimeOffset SentAt,
    bool IsAgent);

public sealed record Ticket(
    string Id,
    string TicketNumber,
    string RequesterEmail,
    string Title,
    string Complaint,
    string Status,
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string AssignedTo,
    IReadOnlyList<TicketReply> Replies);

public sealed record TicketSummary(
    int Total,
    int Open,
    int Pending,
    int Replied,
    int Resolved,
    int Urgent);

public sealed record SlaMetrics(int FirstResponse, int ResolutionTarget);

public sealed record DashboardResponse(
    string GreetingName,
    string Date,
    TicketSummary Summary,
    IReadOnlyList<Ticket> Tickets,
    IReadOnlyList<Ticket> RecentTickets,
    IReadOnlyList<Ticket> ReplyQueue,
    SlaMetrics Sla,
    IReadOnlyList<string> TeamNotes);
