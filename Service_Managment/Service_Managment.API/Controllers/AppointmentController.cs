using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;

namespace Service_Managment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _service;
    public AppointmentController(IAppointmentService service) => _service = service;

    [Authorize(Roles = "User")]
    [HttpPost("book")]
    public async Task<IActionResult> Book(BookAppointmentRequest request) => await Run(() => _service.BookAppointmentAsync(request, UserId()));

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/accept")]
    public async Task<IActionResult> Accept(int id) => await Run(() => _service.AcceptAppointmentAsync(id, UserId()));

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id) => await Run(() => _service.RejectAppointmentAsync(id, UserId()));

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/start")]
    public async Task<IActionResult> Start(int id, StartJobRequest request) => await Run(() => _service.StartJobAsync(id, UserId(), request.Otp));

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id) => await Run(() => _service.CompleteJobAsync(id, UserId()));

    [Authorize(Roles = "Worker")]
    [HttpPost("{id:int}/cannot-complete")]
    public async Task<IActionResult> CannotComplete(int id, FailureReasonRequest request) => await Run(() => _service.CannotCompleteJobAsync(id, UserId(), request.FailureReason));

    [Authorize(Roles = "User")]
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id) => await Run(() => _service.CancelAppointmentAsync(id, UserId()));

    [Authorize(Roles = "User")]
    [HttpGet("user")]
    public async Task<IActionResult> ForUser() => await Run(() => _service.GetUserAppointmentsAsync(UserId()));

    [Authorize(Roles = "Worker")]
    [HttpGet("worker")]
    public async Task<IActionResult> ForWorker() => await Run(() => _service.GetWorkerAppointmentsAsync(UserId()));

    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private static async Task<IActionResult> Run<T>(Func<Task<T>> action)
    {
        try { return new OkObjectResult(await action()); }
        catch (KeyNotFoundException e) { return new NotFoundObjectResult(new { message = e.Message }); }
        catch (UnauthorizedAccessException e) { return new ObjectResult(new { message = e.Message }) { StatusCode = 403 }; }
        catch (InvalidOperationException e) { return new ConflictObjectResult(new { message = e.Message }); }
    }
}
