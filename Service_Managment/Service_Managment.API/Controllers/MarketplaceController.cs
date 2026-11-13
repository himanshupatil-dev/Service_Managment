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
[Route("api/Appointment")]
[Authorize]
public class MarketplaceController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public MarketplaceController(ApplicationDbContext db) => _db = db;

    [Authorize(Roles = "User")]
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, PayRequest request)
    {
        var userId = UserId();
        if (!Enum.TryParse<PaymentMethod>(request.Method, true, out var method))
            return BadRequest(new { message = "Choose UPI, Wallet, or Cash." });
        var appointment = await _db.Appointments.Include(a => a.Worker).ThenInclude(w => w.User)
            .SingleOrDefaultAsync(a => a.AppointmentId == id);
        if (appointment is null) return NotFound(new { message = "Appointment not found." });
        if (appointment.UserId != userId) return Forbid();
        if (appointment.Status is not (Status.AwaitingPayment or Status.Completed))
            return Conflict(new { message = "Payment is available after the worker finishes the job." });
        var existing = await _db.Payments.Where(p => p.AppointmentId == id)
            .OrderByDescending(p => p.PaymentId).FirstOrDefaultAsync();
        if (existing?.Status == PaymentStatus.Success)
        {
            if (appointment.Status != Status.Completed)
            {
                appointment.Status = Status.Completed;
                await _db.SaveChangesAsync();
            }
            return Ok(PaymentResult(existing));
        }
        if (appointment.Status == Status.Completed)
            return Conflict(new { message = "This appointment is already complete." });

        var payment = existing ?? new Payment { AppointmentId = id };
        payment.Method = method;
        payment.Amount = appointment.Worker.Price;
        payment.Status = PaymentStatus.Pending;
        payment.PaidAt = null;
        if (existing is null)
        {
            _db.Payments.Add(payment);
        }
        await _db.SaveChangesAsync();
        return Ok(PaymentResult(payment));
    }

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/payment/confirm")]
    public async Task<IActionResult> ConfirmPayment(int id)
    {
        var userId = UserId();
        var appointment = await _db.Appointments.Include(a => a.Worker)
            .SingleOrDefaultAsync(a => a.AppointmentId == id);
        if (appointment is null) return NotFound(new { message = "Appointment not found." });
        if (appointment.Worker.UserId != userId) return Forbid();

        var payment = await _db.Payments.Where(p => p.AppointmentId == id)
            .OrderByDescending(p => p.PaymentId).FirstOrDefaultAsync();
        if (payment is null || payment.Status != PaymentStatus.Pending)
            return Conflict(new { message = "There is no payment awaiting confirmation." });
        if (appointment.Status != Status.AwaitingPayment)
            return Conflict(new { message = "This appointment is not awaiting payment." });

        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;
        appointment.Status = Status.Completed;
        await _db.SaveChangesAsync();
        return Ok(PaymentResult(payment));
    }

    [Authorize(Roles = "User")]
    [HttpGet("{id:int}/payment")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var owns = await _db.Appointments.AnyAsync(a => a.AppointmentId == id && a.UserId == UserId());
        if (!owns) return NotFound();
        var payment = await _db.Payments.Where(p => p.AppointmentId == id)
            .OrderByDescending(p => p.PaymentId).FirstOrDefaultAsync();
        return payment is null ? NotFound() : Ok(PaymentResult(payment));
    }

    [Authorize(Roles = "User")]
    [HttpPost("{id:int}/rating")]
    public async Task<IActionResult> Rate(int id, RatingRequest request)
    {
        if (request.Stars is < 1 or > 5) return BadRequest(new { message = "Rating must be from 1 to 5 stars." });
        var userId = UserId();
        var appointment = await _db.Appointments.Include(a => a.Worker)
            .SingleOrDefaultAsync(a => a.AppointmentId == id);
        if (appointment is null || appointment.UserId != userId) return NotFound(new { message = "Appointment not found." });
        if (appointment.Status != Status.Completed) return Conflict(new { message = "You can rate a completed job." });
        if (!await _db.Payments.AnyAsync(p => p.AppointmentId == id && p.Status == PaymentStatus.Success))
            return Conflict(new { message = "Pay for the job before leaving a rating." });
        if (await _db.Ratings.AnyAsync(r => r.AppointmentId == id)) return Conflict(new { message = "This job has already been rated." });
        var rating = new Rating
        {
            AppointmentId = id, UserId = userId, WorkerId = appointment.WorkerId,
            Score = request.Stars, Review = request.Comment?.Trim() ?? string.Empty, CreatedAt = DateTime.UtcNow
        };
        var previousScores = await _db.Ratings.Where(r => r.WorkerId == appointment.WorkerId).Select(r => r.Score).ToListAsync();
        _db.Ratings.Add(rating);
        appointment.Worker.Rating = (previousScores.Sum() + request.Stars) / (previousScores.Count + 1d);
        await _db.SaveChangesAsync();
        return Ok(new { stars = rating.Score, comment = rating.Review, createdAt = rating.CreatedAt });
    }

    [Authorize(Roles = "User")]
    [HttpGet("{id:int}/rating")]
    public async Task<IActionResult> GetRating(int id)
    {
        var owns = await _db.Appointments.AnyAsync(a => a.AppointmentId == id && a.UserId == UserId());
        if (!owns) return NotFound();
        var rating = await _db.Ratings.FirstOrDefaultAsync(r => r.AppointmentId == id);
        return rating is null ? NotFound() : Ok(new { stars = rating.Score, comment = rating.Review, createdAt = rating.CreatedAt });
    }

    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private static object PaymentResult(Payment p) => new { method = p.Method.ToString(), amount = p.Amount, paidAt = p.PaidAt, status = p.Status.ToString() };
}

public sealed record PayRequest(string Method);
public sealed record RatingRequest(int Stars, string? Comment);


