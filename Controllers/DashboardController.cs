using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_API.Data;
using Ticket_API.Models;

namespace Ticket_API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[Produces("application/json")]
public sealed class DashboardController : ControllerBase
{
    /// <summary>Data dashboard backoffice (butuh Bearer token dari login).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<DashboardResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<DashboardResponse>> Get()
    {
        var tickets = InMemoryStore.Tickets;
        var name = User.Identity?.Name ?? "Nico";

        var data = new DashboardResponse(
            GreetingName: name,
            Date: "Tuesday, 22 September 2026",
            Summary: InMemoryStore.BuildSummary(),
            Tickets: tickets.ToList(),
            RecentTickets: tickets.Take(4).ToList(),
            ReplyQueue: tickets.Where(t => t.Status != "resolved").Take(3).ToList(),
            Sla: new SlaMetrics(FirstResponse: 92, ResolutionTarget: 78),
            TeamNotes:
            [
                "Prioritaskan tiket login dan akses finance sebelum jam 16.00.",
                "Template balasan sudah disiapkan untuk kasus akses dan email approval.",
                "Follow up ticket pending setelah ada approval owner.",
            ]);

        return Ok(ApiResponse<DashboardResponse>.Ok(
            data,
            "Dashboard berhasil dimuat.",
            HttpContext.TraceIdentifier));
    }
}
