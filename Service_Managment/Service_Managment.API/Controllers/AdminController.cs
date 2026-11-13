using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Managment.Domain.Enum;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;
using Service_Managment.Infrastructure.Persistence;

namespace Service_Managment.API.Controllers;

[ApiController]
[Route("api/Admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) => _db = db;

    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        var workers = await _db.Workers.AsNoTracking().Include(w => w.User).OrderBy(w => w.WorkerId)
            .Select(w => new { workerId = w.WorkerId, name = w.User.UserName, category = w.Category.ToString(), rating = Math.Round(w.Rating, 1), isBlocked = w.IsBlocked, verificationStatus = w.VerificationStatus.ToString() }).ToListAsync();
        var complaints = await _db.Complaints.AsNoTracking().OrderByDescending(c => c.CreatedAt)
            .Select(c => new { complaintId = c.ComplaintId, appointmentId = c.AppointmentId, text = c.Description, status = c.Status.ToString() }).ToListAsync();
        return Ok(new
        {
            totalUsers = await _db.Users.CountAsync(u => u.Role == UserRole.User), totalWorkers = workers.Count,
            activeJobs = await _db.Appointments.CountAsync(a => a.Status == Status.Accepted || a.Status == Status.InProgress),
            openComplaints = complaints.Count(c => c.status == ComplaintStatus.Open.ToString()), workers, complaints
        });
    }

    [HttpPost("workers/{workerId:int}/block")]
    public async Task<IActionResult> SetBlocked(int workerId, WorkerFlagRequest request)
    {
        var worker = await _db.Workers.SingleOrDefaultAsync(w => w.WorkerId == workerId);
        if (worker is null) return NotFound(new { message = "Worker not found." });
        if (request.Value && await _db.Appointments.AnyAsync(a => a.WorkerId == workerId && (a.Status == Status.Accepted || a.Status == Status.InProgress)))
            return Conflict(new { message = "The worker has an active appointment." });
        worker.IsBlocked = request.Value;
        worker.IsAvailable = !request.Value && worker.VerificationStatus == VerificationStatus.Verified;
        await _db.SaveChangesAsync();
        return Ok(new { isBlocked = worker.IsBlocked });
    }

    [HttpPost("workers/{workerId:int}/verify")]
    public async Task<IActionResult> SetVerified(int workerId, WorkerFlagRequest request)
    {
        var worker = await _db.Workers.SingleOrDefaultAsync(w => w.WorkerId == workerId);
        if (worker is null) return NotFound(new { message = "Worker not found." });
        worker.VerificationStatus = request.Value ? VerificationStatus.Verified : VerificationStatus.Pending;
        worker.IsAvailable = request.Value && !worker.IsBlocked;
        await _db.SaveChangesAsync();
        return Ok(new { verificationStatus = worker.VerificationStatus.ToString() });
    }
}

public sealed record WorkerFlagRequest(bool Value);
