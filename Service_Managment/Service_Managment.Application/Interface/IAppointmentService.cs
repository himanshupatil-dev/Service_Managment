using Service_Managment.Application.DTOs;

namespace Service_Managment.Application.Interface;

public interface IAppointmentService
{
    Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request, int userId);
    Task<AppointmentResponse> AcceptAppointmentAsync(int appointmentId, int userId);
    Task<string> RejectAppointmentAsync(int appointmentId, int userId);
    Task<AppointmentResponse> StartJobAsync(int appointmentId, int userId, string otpCode);
    Task<AppointmentResponse> CompleteJobAsync(int appointmentId, int userId);
    Task<AppointmentResponse> CannotCompleteJobAsync(int appointmentId, int userId, string reason);
    Task<AppointmentResponse> CancelAppointmentAsync(int appointmentId, int userId);
    Task<List<AppointmentResponse>> GetUserAppointmentsAsync(int userId);
    Task<List<AppointmentResponse>> GetWorkerAppointmentsAsync(int userId);
}
