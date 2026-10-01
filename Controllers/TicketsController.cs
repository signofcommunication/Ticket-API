using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_API.Data;
using Ticket_API.Models;

namespace Ticket_API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
[Produces("application/json")]
public sealed class TicketsController : ControllerBase
{
    /// <summary>Submit tiket baru (wajib login, agent backoffice).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Ticket>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<Ticket>> Submit([FromBody] CreateTicketRequest request)
    {
        var ticket = InMemoryStore.AddTicket(
            title: request.Title.Trim(),
            name: request.Name.Trim(),
            email: request.Email.Trim().ToLowerInvariant(),
            description: request.Description.Trim(),
            issueType: request.IssueType.Trim());

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticket.Id },
            ApiResponse<Ticket>.Ok(ticket, "Tiket berhasil dibuat.", HttpContext.TraceIdentifier));
    }

    /// <summary>Ambil detail tiket by id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Ticket>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<Ticket>> GetById(string id)
    {
        var ticket = InMemoryStore.Tickets.FirstOrDefault(t =>
            string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

        if (ticket is null)
        {
            return NotFound(ApiResponse<Ticket>.Fail(
                "Tiket tidak ditemukan.",
                [new ApiError("not_found", $"Tiket '{id}' tidak ditemukan.")],
                HttpContext.TraceIdentifier));
        }

        return Ok(ApiResponse<Ticket>.Ok(ticket, "Tiket ditemukan.", HttpContext.TraceIdentifier));
    }
}
