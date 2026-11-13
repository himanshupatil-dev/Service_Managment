using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;
using Service_Managment.Domain.Entities;
using Service_Managment.Domain.Enum;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;

namespace Service_Managment.Application.Services;

public class AppointmentService : IAppointmentService
{
    private const string CodeNotificationPrefix = "AppointmentStartCode:";
    private readonly IAppointmentRepository _appointments;
    private readonly IGenericRepository<Worker> _workers;
    private readonly IGenericRepository<OtpVerification> _codes;
    private readonly IGenericRepository<Notification> _notifications;
    private readonly int _expiryMinutes;
    private readonly int _maxAttempts;

    public AppointmentService(IAppointmentRepository appointments, IGenericRepository<Worker> workers,
        IGenericRepository<OtpVerification> codes, IGenericRepository<Notification> notifications,
        IConfiguration configuration)
    {
        _appointments = appointments;
        _workers = workers;
        _codes = codes;
        _notifications = notifications;
        _expiryMinutes = Math.Max(1, configuration.GetValue<int?>("Otp:ExpiryMinutes") ?? 30);
        _maxAttempts = Math.Max(1, configuration.GetValue<int?>("Otp:MaxAttempts") ?? 5);
    }

    public async Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request, int userId)
    {
        var worker = await _workers.GetByIdAsync(request.WorkerId) ?? throw new KeyNotFoundException("Worker not found.");
        if (worker.UserId == userId) throw new InvalidOperationException("You cannot book yourself.");
        if (worker.IsBlocked || worker.VerificationStatus != VerificationStatus.Verified)
            throw new InvalidOperationException("This worker is not approved for bookings.");
        if (!worker.IsAvailable || await _appointments.HasActiveAppointmentAsync(worker.WorkerId))
            throw new InvalidOperationException("This worker is currently unavailable.");

        var appointment = new Appointment
        {
            UserId = userId, WorkerId = worker.WorkerId, ProblemDescription = request.ProblemDescription.Trim(),
            PartsRequired = request.PartsRequired, Status = Status.Pending, CreatedAt = DateTime.UtcNow
        };
        await _appointments.AddAsync(appointment);
        await _appointments.SaveChangesAsync();
        var created = await _appointments.GetWithDetailsAsync(appointment.AppointmentId)
            ?? throw new InvalidOperationException("The appointment could not be loaded after saving.");
        return Map(created);
    }

    public async Task<AppointmentResponse> AcceptAppointmentAsync(int appointmentId, int userId)
    {
        var worker = await GetWorkerForUser(userId);
        if (!worker.IsAvailable || worker.IsBlocked || worker.VerificationStatus != VerificationStatus.Verified)
            throw new InvalidOperationException("Your worker profile is not available for bookings.");
        var appointment = await GetAppointment(appointmentId);
        if (appointment.WorkerId != worker.WorkerId) throw new UnauthorizedAccessException("This appointment belongs to another worker.");
        if (appointment.Status != Status.Pending) throw new InvalidOperationException("Only pending appointments can be accepted.");
        if (await _appointments.HasActiveAppointmentAsync(worker.WorkerId)) throw new InvalidOperationException("You already have an active appointment.");

        appointment.Status = Status.Accepted;
        worker.IsAvailable = false;
        var code = GenerateCode();
        await _codes.AddAsync(new OtpVerification
        {
            AppointmentId = appointment.AppointmentId, OtpHash = HashCode(code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_expiryMinutes), IsUsed = false, FailedAttempts = 0, CreatedAt = DateTime.UtcNow
        });
        await _notifications.AddAsync(new Notification
        {
            UserId = appointment.UserId, Type = CodeNotificationPrefix + appointment.AppointmentId,
            Message = code, Channel = NotificationChannel.Email, IsRead = false, SentAt = DateTime.UtcNow
        });
        await _appointments.SaveChangesAsync();
        var result = Map(appointment);
        result.StartCode = null;
        return result;
    }

    public async Task<string> RejectAppointmentAsync(int appointmentId, int userId)
    {
        var worker = await GetWorkerForUser(userId);
        var appointment = await GetAppointment(appointmentId);
        EnsureWorkerOwns(appointment, worker);
        if (appointment.Status != Status.Pending) throw new InvalidOperationException("Only pending appointments can be rejected.");
        appointment.Status = Status.Rejected;
        worker.IsAvailable = true;
        await _appointments.SaveChangesAsync();
        return "Appointment rejected.";
    }

    public async Task<AppointmentResponse> StartJobAsync(int appointmentId, int userId, string otpCode)
    {
        var worker = await GetWorkerForUser(userId);
        var appointment = await GetAppointment(appointmentId);
        EnsureWorkerOwns(appointment, worker);
        if (appointment.Status != Status.Accepted) throw new InvalidOperationException("Accept this appointment before starting it.");
        var records = await _codes.FindAsync(x => x.AppointmentId == appointmentId && !x.IsUsed);
        var code = records.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        if (code is null || code.ExpiresAt <= DateTime.UtcNow) throw new InvalidOperationException("The start code has expired. Ask the customer to cancel and rebook.");
        if (code.FailedAttempts >= _maxAttempts) throw new InvalidOperationException("Too many incorrect start codes.");
        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(code.OtpHash), Convert.FromHexString(HashCode(otpCode))))
        {
            code.FailedAttempts++;
            await _codes.SaveChangesAsync();
            throw new InvalidOperationException("The start code is incorrect.");
        }
        code.IsUsed = true;
        appointment.Status = Status.InProgress;
        await _appointments.SaveChangesAsync();
        return Map(appointment);
    }

    public async Task<AppointmentResponse> CompleteJobAsync(int appointmentId, int userId)
    {
        var worker = await GetWorkerForUser(userId);
        var appointment = await GetAppointment(appointmentId);
        EnsureWorkerOwns(appointment, worker);
        if (appointment.Status != Status.InProgress) throw new InvalidOperationException("Start the job before completing it.");
        appointment.Status = Status.AwaitingPayment;
        worker.IsAvailable = true;
        await _appointments.SaveChangesAsync();
        return Map(appointment);
    }

    public async Task<AppointmentResponse> CannotCompleteJobAsync(int appointmentId, int userId, string reason)
    {
        var worker = await GetWorkerForUser(userId);
        var appointment = await GetAppointment(appointmentId);
        EnsureWorkerOwns(appointment, worker);
        if (appointment.Status != Status.InProgress) throw new InvalidOperationException("Only an in-progress job can be marked unable to complete.");
        appointment.Status = Status.CannotComplete;
        appointment.FailureReason = reason.Trim();
        worker.IsAvailable = true;
        await _appointments.SaveChangesAsync();
        return Map(appointment);
    }

    public async Task<AppointmentResponse> CancelAppointmentAsync(int appointmentId, int userId)
    {
        var appointment = await GetAppointment(appointmentId);
        if (appointment.UserId != userId) throw new UnauthorizedAccessException("This appointment belongs to another customer.");
        if (appointment.Status is not (Status.Pending or Status.Accepted)) throw new InvalidOperationException("This appointment can no longer be cancelled.");
        appointment.Status = Status.Cancelled;
        var worker = appointment.Worker;
        if (worker is not null) worker.IsAvailable = true;
        await _appointments.SaveChangesAsync();
        return Map(appointment);
    }

    public async Task<List<AppointmentResponse>> GetUserAppointmentsAsync(int userId)
    {
        var appointments = await _appointments.GetUserAppointmentsAsync(userId);
        var notifications = await _notifications.FindAsync(n => n.UserId == userId && n.Type.StartsWith(CodeNotificationPrefix));
        var codesByAppointment = notifications.ToDictionary(n => n.Type[CodeNotificationPrefix.Length..], n => n.Message);
        return appointments.OrderByDescending(a => a.CreatedAt).Select(a =>
        {
            var result = Map(a);
            if (a.Status == Status.Accepted && codesByAppointment.TryGetValue(a.AppointmentId.ToString(), out var code)) result.StartCode = code;
            return result;
        }).ToList();
    }

    public async Task<List<AppointmentResponse>> GetWorkerAppointmentsAsync(int userId)
    {
        var worker = await GetWorkerForUser(userId);
        return (await _appointments.GetWorkerAppointmentAsync(worker.WorkerId)).OrderByDescending(a => a.CreatedAt).Select(Map).ToList();
    }

    private async Task<Worker> GetWorkerForUser(int userId) =>
        (await _workers.FindAsync(w => w.UserId == userId)).SingleOrDefault() ?? throw new KeyNotFoundException("Worker profile not found.");
    private async Task<Appointment> GetAppointment(int appointmentId) =>
        await _appointments.GetWithDetailsAsync(appointmentId) ?? throw new KeyNotFoundException("Appointment not found.");
    private static void EnsureWorkerOwns(Appointment appointment, Worker worker)
    {
        if (appointment.WorkerId != worker.WorkerId) throw new UnauthorizedAccessException("This appointment belongs to another worker.");
    }
    private static AppointmentResponse Map(Appointment a) => new()
    {
        AppointmentId = a.AppointmentId, UserId = a.UserId, UserName = a.User?.UserName ?? string.Empty,
        WorkerId = a.WorkerId, WorkerPrice = a.Worker?.Price ?? 0m, WorkerName = a.Worker?.User?.UserName ?? string.Empty,
        Status = a.Status.ToString(), PartsRequired = a.PartsRequired,
        ProblemDescription = a.ProblemDescription, CreatedAt = a.CreatedAt,
        FailureReason = string.IsNullOrWhiteSpace(a.FailureReason) ? null : a.FailureReason,
        Rating = a.RatingDetails?.Score,
                RatingComment = a.RatingDetails?.Review,
                PaymentMethod = a.Payments?.FirstOrDefault(p => p.Status == PaymentStatus.Success)?.Method.ToString(),
                PaymentStatus = a.Payments?.OrderByDescending(p => p.PaymentId).FirstOrDefault()?.Status.ToString(),
                PaidAt = a.Payments?.FirstOrDefault(p => p.Status == PaymentStatus.Success)?.PaidAt
    };
    private static string GenerateCode() => RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    private static string HashCode(string code) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
}
