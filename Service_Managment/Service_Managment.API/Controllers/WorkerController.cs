using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;
using Service_Managment.Infrastructure.Persistence;

namespace Service_Managment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkerController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public WorkerController(ApplicationDbContext db) => _db = db;

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? category, [FromQuery] string? city, [FromQuery] double minRating = 0)
    {
        var workers = _db.Workers.AsNoTracking().Include(w => w.User).Where(w =>
            !w.IsBlocked && w.VerificationStatus == VerificationStatus.Verified && w.Rating >= minRating);
        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!Enum.TryParse<WorkerCategory>(category, true, out var parsedCategory))
                return BadRequest(new { message = "Unknown service category." });
            workers = workers.Where(w => w.Category == parsedCategory);
        }
        if (!string.IsNullOrWhiteSpace(city))
            workers = workers.Where(w => w.User.Address.Contains(city));

        var result = await workers.OrderByDescending(w => w.Rating).Select(w => new
        {
            workerId = w.WorkerId,
            name = w.User.UserName,
            category = w.Category.ToString(),
            city = w.User.Address,
            rating = Math.Round(w.Rating, 1),
            jobs = _db.Appointments.Count(a => a.WorkerId == w.WorkerId && a.Status == Status.Completed),
            isAvailable = w.IsAvailable,
            bio = w.Qualification
        }).ToListAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Worker")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = CurrentUserId();
        var worker = await _db.Workers.AsNoTracking().Include(w => w.User).SingleOrDefaultAsync(w => w.UserId == userId);
        return worker is null ? NotFound() : Ok(new { workerId = worker.WorkerId, isAvailable = worker.IsAvailable && !worker.IsBlocked && worker.VerificationStatus == VerificationStatus.Verified });
    }

    [Authorize(Roles = "Worker")]
    [HttpPost("availability")]
    public async Task<IActionResult> SetAvailability(AvailabilityRequest request)
    {
        var userId = CurrentUserId();
        var worker = await _db.Workers.SingleOrDefaultAsync(w => w.UserId == userId);
        if (worker is null) return NotFound(new { message = "Worker profile not found." });
        if (worker.IsBlocked || worker.VerificationStatus != VerificationStatus.Verified)
            return Conflict(new { message = "Your profile is not approved for bookings." });
        var hasActive = await _db.Appointments.AnyAsync(a => a.WorkerId == worker.WorkerId &&
            (a.Status == Status.Accepted || a.Status == Status.InProgress));
        if (hasActive && request.IsAvailable)
            return Conflict(new { message = "Finish your active appointment before becoming available." });
        worker.IsAvailable = request.IsAvailable;
        await _db.SaveChangesAsync();
        return Ok(new { isAvailable = worker.IsAvailable });
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public sealed record AvailabilityRequest(bool IsAvailable);
