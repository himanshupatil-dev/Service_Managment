using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Managment.Application.DTOs;
using Service_Managment.Application.Interface;
using Service_Managment.Application.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service_Managment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _Service;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _Service = appointmentService;
        }

        [Authorize]
        [HttpPost("appointments/book")]
        public async Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request)
        {   
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _Service.BookAppointmentAsync(request, userId);
            return result;
        }

        [Authorize(Roles = "Worker")]
        [HttpPost("appointments/{appointmentId}/accept")]
        public async Task<AppointmentResponse> AcceptAppointmentAsync(int appointmentId)
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _Service.AcceptAppointmentAsync(appointmentId, workerId);
            return result;
        }

        [Authorize(Roles = "Worker")]
        [HttpPost("appointments/reject")]
        public async Task<string> RejectAppointmentAsync(int appointmentId, AppointmentResponse result)
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _Service.RejectAppointmentAsync(appointmentId, workerId);
            return "Appointment is rejected";
        }

        [HttpPost("appointments/start-job")]
        [Authorize(Roles = "Worker")]
        public async Task<AppointmentResponse> StartJobAsync(int appointmentId,string otpCode)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _Service.StartJobAsync(
                appointmentId,
                userId,
                otpCode
            );

            return result;
        }

        [HttpPost("appointments/complete")]
        [Authorize(Roles = "Worker")]
        public async Task<AppointmentResponse> CompleteJobAsync(int appointmentId)
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value); 
            var result = await _Service.CompleteJobAsync(appointmentId, workerId);
            return result;

        }


        [HttpPost("appointments/cannot-complete")]
        [Authorize(Roles = "Worker")]
        public async Task<AppointmentResponse> CannotCompleteJobAsync(int appointmentId, string reason)
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _Service.CannotCompleteJobAsync(appointmentId, workerId, reason);
            return result;
        }

        [HttpPost("appointments/cancel-appointment")]
        [Authorize(Roles = "User")]

        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _Service.CancelAppointmentAsync(appointmentId, userId);
            return Ok(result);
        }

        [HttpGet("appointments/get-user-appointment")]
        public async Task<List<AppointmentResponse>> GetUserAppointmentsAsync()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _Service.GetUserAppointmentsAsync(userId);

            return result;
        }


        [HttpGet("appointments/get-worker-appointment")]
        [AllowAnonymous]
        public async Task<List<AppointmentResponse>> GetWorkerAppointmentsAsync()
        {
            var workerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _Service.GetWorkerAppointmentsAsync(workerId);
            return result;
        }

        [Authorize(Roles = "Worker")]
        [HttpPost("{appointmentId}/otp")]
        public async Task<IActionResult> RequestOtp(int appointmentId)
        {
            var workerId = GetWorkerIdFromClaims();

            await _Service.GenerateOtpAsync(
                appointmentId,
                workerId);

            return Ok(new
            {
                message = "OTP has been sent to the customer."
            });
        }

        private int GetWorkerIdFromClaims()
        {
            var workerIdClaim = User.FindFirst("WorkerId")?.Value;

            if (string.IsNullOrEmpty(workerIdClaim))
                throw new UnauthorizedAccessException("Worker ID not found in token.");

            return int.Parse(workerIdClaim);
        }

        

        
}
}

