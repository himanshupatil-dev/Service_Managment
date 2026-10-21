using Service_Managment.Application.DTOs;

namespace Service_Managment.Application.Interface
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> BookAppointmentAsync(BookAppointmentRequest request, int userId);
        Task<AppointmentResponse> AcceptAppointmentAsync(int appointmentId, int workerId);
        Task<string> RejectAppointmentAsync(int appointmentId, int workerId);
        Task<AppointmentResponse> StartJobAsync(int appointmentId, int workerId, string otpCode);
        Task<AppointmentResponse> CompleteJobAsync(int appointmentId, int workerId);
        Task<AppointmentResponse> CannotCompleteJobAsync(int appointmentId, int workerId, string reason);
        Task<AppointmentResponse> CancelAppointmentAsync(int appointmentId, int userId);
        Task<List<AppointmentResponse>> GetUserAppointmentsAsync(int userId);
        Task<List<AppointmentResponse>> GetWorkerAppointmentsAsync(int workerId);

        Task GenerateOtpAsync(int appointmentId, int workerId);
        // Task<AppointmentResponse> AcceptAppointmentAsync(int id);
        //Task<List<AppointmentResponse>> GetWorkerByCategory(int workerId);
    }
}